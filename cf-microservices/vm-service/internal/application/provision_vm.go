// Package application — use cases. HTTP, gRPC, and message
// handlers all call into these; the layer knows nothing
// about the entry point.
package application

import (
	"context"
	"fmt"

	"yourplatform/vm-service/internal/domain"
)

// EventPublisher — infrastructure fulfills this.
type EventPublisher interface {
	Publish(ctx context.Context, event domain.DomainEvent) error
}

// ProvisionVMCommand — input for the provision use case.
type ProvisionVMCommand struct {
	Name          string
	ResourceGroup string
	Region        string
	OSType        string
	Size          string
}

// ProvisionVMResult — returned to the caller.
type ProvisionVMResult struct {
	VMID   string
	Status domain.Status
}

// ProvisionVMHandler — the use case.
type ProvisionVMHandler struct {
	repo      domain.VirtualMachineRepository
	publisher EventPublisher
	newID     func() string
}

func NewProvisionVMHandler(
	repo domain.VirtualMachineRepository,
	publisher EventPublisher,
	newID func() string,
) *ProvisionVMHandler {
	return &ProvisionVMHandler{repo: repo, publisher: publisher, newID: newID}
}

// Handle — orchestrates: create → persist → publish events.
func (h *ProvisionVMHandler) Handle(
	ctx context.Context,
	cmd ProvisionVMCommand,
) (*ProvisionVMResult, error) {
	vm, err := domain.NewVirtualMachine(
		h.newID(), cmd.Name, cmd.Region, cmd.Size, cmd.OSType, cmd.ResourceGroup)
	if err != nil {
		return nil, fmt.Errorf("provision vm: %w", err)
	}

	if err := h.repo.Add(ctx, vm); err != nil {
		return nil, fmt.Errorf("provision vm: persist: %w", err)
	}

	for _, evt := range vm.Events() {
		if err := h.publisher.Publish(ctx, evt); err != nil {
			// At-least-once: the outbox-style retry is infra's job.
			return nil, fmt.Errorf("provision vm: publish: %w", err)
		}
	}
	vm.ClearEvents()

	return &ProvisionVMResult{VMID: vm.ID, Status: vm.Status}, nil
}
