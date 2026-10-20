// Package api — HTTP entry point. Translates requests into
// application commands/queries; nothing domain-aware lives here.
package api

import (
	"encoding/json"
	"errors"
	"log/slog"
	"net/http"

	"yourplatform/vm-service/internal/application"
	"yourplatform/vm-service/internal/domain"
)

// Handler — thin adapter over use cases.
type Handler struct {
	provision *application.ProvisionVMHandler
	start     *application.StartVMHandler
	list      *application.ListVMsHandler
	logger    *slog.Logger
}

func NewHandler(
	provision *application.ProvisionVMHandler,
	start *application.StartVMHandler,
	list *application.ListVMsHandler,
	logger *slog.Logger,
) *Handler {
	return &Handler{
		provision: provision,
		start:     start,
		list:      list,
		logger:    logger,
	}
}

type provisionRequest struct {
	Name          string `json:"name"`
	ResourceGroup string `json:"resourceGroup"`
	Region        string `json:"region"`
	OSType        string `json:"osType"`
	Size          string `json:"size"`
}

type provisionResponse struct {
	VMID   string `json:"vmId"`
	Status string `json:"status"`
}

type errorResponse struct {
	Code    string `json:"code"`
	Message string `json:"message"`
}

// ProvisionVM — POST /api/v1/vms
func (h *Handler) ProvisionVM(w http.ResponseWriter, r *http.Request) {
	var req provisionRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeError(w, http.StatusBadRequest, "invalid body")
		return
	}

	result, err := h.provision.Handle(r.Context(),
		application.ProvisionVMCommand{
			Name:          req.Name,
			ResourceGroup: req.ResourceGroup,
			Region:        req.Region,
			OSType:        req.OSType,
			Size:          req.Size,
		})

	if err != nil {
		h.writeDomainError(w, err)
		return
	}

	writeJSON(w, http.StatusCreated, provisionResponse{
		VMID:   result.VMID,
		Status: string(result.Status),
	})
}

// ListVMs — GET /api/v1/vms
func (h *Handler) ListVMs(w http.ResponseWriter, r *http.Request) {
	vms, err := h.list.Handle(r.Context())
	if err != nil {
		writeError(w, http.StatusInternalServerError, "internal error")
		return
	}

	writeJSON(w, http.StatusOK, vms)
}

// StartVM — POST /api/v1/vms/{id}/start
func (h *Handler) StartVM(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")
	if id == "" {
		writeError(w, http.StatusBadRequest, "missing id")
		return
	}

	if err := h.start.Handle(r.Context(), id); err != nil {
		h.writeDomainError(w, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}

func (h *Handler) writeDomainError(w http.ResponseWriter, err error) {
	switch {
	case errors.Is(err, domain.ErrInvalidState):
		writeError(w, http.StatusUnprocessableEntity, err.Error())
	default:
		h.logger.Error("unhandled", "error", err)
		writeError(w, http.StatusInternalServerError, "internal error")
	}
}

func writeJSON(w http.ResponseWriter, status int, body any) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	_ = json.NewEncoder(w).Encode(body)
}

func writeError(w http.ResponseWriter, status int, msg string) {
	writeJSON(w, status, errorResponse{
		Code:    http.StatusText(status),
		Message: msg,
	})
}
