package application

import (
	"io"

	"yourplatform/storage-service/internal/domain"
)

// UploadHandler — the upload use case.
type UploadHandler struct {
	store domain.ObjectStore
}

func NewUploadHandler(store domain.ObjectStore) *UploadHandler {
	return &UploadHandler{store: store}
}

func (h *UploadHandler) Handle(
	key string, r io.Reader, contentType string,
) error {
	return h.store.Put(key, r, contentType)
}
