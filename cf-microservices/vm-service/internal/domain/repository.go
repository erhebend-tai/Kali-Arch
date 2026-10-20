package domain

import "context"

// VirtualMachineRepository — the persistence contract.
// Infrastructure implements; application depends on this interface.
type VirtualMachineRepository interface {
	GetByID(ctx context.Context, id string) (*VirtualMachine, error)
	List(ctx context.Context) ([]*VirtualMachine, error)
	Add(ctx context.Context, vm *VirtualMachine) error
	Update(ctx context.Context, vm *VirtualMachine) error
}
