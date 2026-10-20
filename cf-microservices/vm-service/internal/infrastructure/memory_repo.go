// Package infrastructure — implementations of the
// domain/application contracts.
package infrastructure

import (
	"context"
	"sync"

	"yourplatform/vm-service/internal/domain"
)

// InMemoryVirtualMachineRepository — dev/test storage.
// Swap for a provider implementation (Cosmos, DynamoDB,
// Firestore, OCI NoSQL, R2/D1) behind the same interface.
type InMemoryVirtualMachineRepository struct {
	mu  sync.RWMutex
	vms map[string]*domain.VirtualMachine
}

func NewInMemoryVirtualMachineRepository() *InMemoryVirtualMachineRepository {
	return &InMemoryVirtualMachineRepository{
		vms: make(map[string]*domain.VirtualMachine),
	}
}

func (r *InMemoryVirtualMachineRepository) GetByID(
	_ context.Context, id string,
) (*domain.VirtualMachine, error) {
	r.mu.RLock()
	defer r.mu.RUnlock()
	return r.vms[id], nil
}

func (r *InMemoryVirtualMachineRepository) List(
	_ context.Context,
) ([]*domain.VirtualMachine, error) {
	r.mu.RLock()
	defer r.mu.RUnlock()

	out := make([]*domain.VirtualMachine, 0, len(r.vms))
	for _, vm := range r.vms {
		out = append(out, vm)
	}
	return out, nil
}

func (r *InMemoryVirtualMachineRepository) Add(
	_ context.Context, vm *domain.VirtualMachine,
) error {
	r.mu.Lock()
	defer r.mu.Unlock()
	r.vms[vm.ID] = vm
	return nil
}

func (r *InMemoryVirtualMachineRepository) Update(
	_ context.Context, vm *domain.VirtualMachine,
) error {
	r.mu.Lock()
	defer r.mu.Unlock()
	r.vms[vm.ID] = vm
	return nil
}
