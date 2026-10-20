// Package domain — the innermost layer. No imports beyond
// the standard library and the shared kernel.
package domain

import (
	"errors"
	"fmt"
	"time"
)

// Status — the VM lifecycle states.
type Status string

const (
	StatusStopped   Status = "Stopped"
	StatusRunning   Status = "Running"
	StatusDeallocated Status = "Deallocated"
)

// ErrInvalidState — domain rule violation.
var ErrInvalidState = errors.New("invalid domain state")

// VirtualMachine — aggregate root. State changes only via methods.
type VirtualMachine struct {
	ID            string
	Name          string
	Region        string
	Size          string
	OSType        string
	ResourceGroup string
	Status        Status
	CreatedAt     time.Time
	UpdatedAt     time.Time

	events []DomainEvent
}

// DomainEvent — raised inside the aggregate, dispatched after commit.
type DomainEvent interface {
	EventName() string
	OccurredAt() time.Time
}

// VMProvisionedEvent — raised by NewVirtualMachine.
type VMProvisionedEvent struct {
	VMID   string
	VMName string
	At     time.Time
}

func (e VMProvisionedEvent) EventName() string    { return "vm.provisioned" }
func (e VMProvisionedEvent) OccurredAt() time.Time { return e.At }

// VMStartedEvent — raised by Start.
type VMStartedEvent struct {
	VMID string
	At   time.Time
}

func (e VMStartedEvent) EventName() string    { return "vm.started" }
func (e VMStartedEvent) OccurredAt() time.Time { return e.At }

// NewVirtualMachine — validated creation via the factory path.
func NewVirtualMachine(id, name, region, size, osType, rg string) (*VirtualMachine, error) {
	if name == "" || len(name) > 63 {
		return nil, fmt.Errorf("%w: invalid name", ErrInvalidState)
	}
	if region == "" {
		return nil, fmt.Errorf("%w: region required", ErrInvalidState)
	}

	vm := &VirtualMachine{
		ID:            id,
		Name:          name,
		Region:        region,
		Size:          size,
		OSType:        osType,
		ResourceGroup: rg,
		Status:        StatusStopped,
		CreatedAt:     time.Now().UTC(),
	}

	vm.raise(VMProvisionedEvent{VMID: id, VMName: name, At: vm.CreatedAt})
	return vm, nil
}

// Start — Stopped|Deallocated → Running.
func (v *VirtualMachine) Start() error {
	if v.Status == StatusRunning {
		return fmt.Errorf("%w: already running", ErrInvalidState)
	}
	v.Status = StatusRunning
	v.UpdatedAt = time.Now().UTC()
	v.raise(VMStartedEvent{VMID: v.ID, At: v.UpdatedAt})
	return nil
}

// Stop — Running → Stopped.
func (v *VirtualMachine) Stop() error {
	if v.Status == StatusStopped {
		return fmt.Errorf("%w: already stopped", ErrInvalidState)
	}
	v.Status = StatusStopped
	v.UpdatedAt = time.Now().UTC()
	return nil
}

// Events — pending domain events (cleared after dispatch).
func (v *VirtualMachine) Events() []DomainEvent { return v.events }
func (v *VirtualMachine) ClearEvents()           { v.events = nil }

func (v *VirtualMachine) raise(e DomainEvent) {
	v.events = append(v.events, e)
}
