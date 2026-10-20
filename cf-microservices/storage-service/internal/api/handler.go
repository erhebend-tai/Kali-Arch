package api

import (
	"encoding/json"
	"net/http"

	"yourplatform/storage-service/internal/application"
)

type Handler struct {
	upload *application.UploadHandler
	list   *application.ListHandler
}

func NewHandler(
	upload *application.UploadHandler,
	list *application.ListHandler,
) *Handler {
	return &Handler{upload: upload, list: list}
}

func (h *Handler) Upload(w http.ResponseWriter, r *http.Request) {
	key := r.PathValue("key")
	if key == "" {
		http.Error(w, "missing key", http.StatusBadRequest)
		return
	}

	contentType := r.Header.Get("Content-Type")
	if contentType == "" {
		contentType = "application/octet-stream"
	}

	if err := h.upload.Handle(key, r.Body, contentType); err != nil {
		http.Error(w, "upload failed", http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusCreated)
}

func (h *Handler) List(w http.ResponseWriter, r *http.Request) {
	objects, err := h.list.Handle()
	if err != nil {
		http.Error(w, "list failed", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(objects)
}
