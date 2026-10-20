package application

import (
	"context"

	"yourplatform/vm-service/internal/domain"
)

// ListVMsHandler — query side of CQRS.
type ListVMsHandler struct {
	repo domain.VirtualMachineRepository
}

func NewListVMsHandler(repo domain.VirtualMachineRepository) *ListVMsHandler {
	return &ListVMsHandler{repo: repo}
}

func (h *ListVMsHandler) Handle(
	ctx context.Context,
) ([]*domain.VirtualMachine, error) {
	return h.repo.List(ctx)
}
