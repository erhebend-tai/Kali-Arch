package api

import (
	"net/http"

	"yourplatform/vm-service/internal/application"
	"log/slog"
)

// NewRouter — composition root for the HTTP surface.
func NewRouter(
	provision *application.ProvisionVMHandler,
	start *application.StartVMHandler,
	list *application.ListVMsHandler,
	logger *slog.Logger,
) http.Handler {
	h := NewHandler(provision, start, list, logger)

	mux := http.NewServeMux()
	mux.HandleFunc("POST /api/v1/vms", h.ProvisionVM)
	mux.HandleFunc("GET /api/v1/vms", h.ListVMs)
	mux.HandleFunc("POST /api/v1/vms/{id}/start", h.StartVM)
	mux.HandleFunc("GET /healthz", func(w http.ResponseWriter, _ *http.Request) {
		w.WriteHeader(http.StatusOK)
	})

	return mux
}
