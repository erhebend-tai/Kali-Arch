package application

import "yourplatform/storage-service/internal/domain"

// ListHandler — list objects use case.
type ListHandler struct {
	store domain.ObjectStore
}

func NewListHandler(store domain.ObjectStore) *ListHandler {
	return &ListHandler{store: store}
}

func (h *ListHandler) Handle() ([]*domain.Object, error) {
	return h.store.List()
}
