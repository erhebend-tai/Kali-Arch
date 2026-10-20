package application

import (
	"context"
	"fmt"

	"yourplatform/vm-service/internal/domain"
)

// StartVMHandler — the start use case.
type StartVMHandler struct {
	repo      domain.VirtualMachineRepository
	publisher EventPublisher
}

func NewStartVMHandler(
	repo domain.VirtualMachineRepository,
	publisher EventPublisher,
) *StartVMHandler {
	return &StartVMHandler{repo: repo, publisher: publisher}
}

func (h *StartVMHandler) Handle(
	ctx context.Context,
	vmID string,
) error {
	vm, err := h.repo.GetByID(ctx, vmID)
	if err != nil {
		return fmt.Errorf("start vm: lookup: %w", err)
	}
	if vm == nil {
		return fmt.Errorf("start vm: %s not found", vmID)
	}

	if err := vm.Start(); err != nil {
		return err
	}

	if err := h.repo.Update(ctx, vm); err != nil {
		return fmt.Errorf("start vm: persist: %w", err)
	}

	for _, evt := range vm.Events() {
		if err := h.publisher.Publish(ctx, evt); err != nil {
			return fmt.Errorf("start vm: publish: %w", err)
		}
	}
	vm.ClearEvents()

	return nil
}
