// vm-service entry point — wires the layers together.
// The handler knows nothing about HTTP; the router knows
// nothing about the domain.
package main

import (
	"log"
	"log/slog"
	"net/http"
	"os"

	"github.com/google/uuid"

	"yourplatform/vm-service/internal/api"
	"yourplatform/vm-service/internal/application"
	"yourplatform/vm-service/internal/infrastructure"
)

func main() {
	logger := slog.New(slog.NewJSONHandler(os.Stdout, nil))

	// Infrastructure — swap these for provider implementations.
	repo := infrastructure.NewInMemoryVirtualMachineRepository()
	publisher := infrastructure.LogEventPublisher{}

	// Application — use cases.
	provision := application.NewProvisionVMHandler(
		repo, publisher, func() string { return uuid.NewString() })
	start := application.NewStartVMHandler(repo, publisher)
	list := application.NewListVMsHandler(repo)

	// API — entry point only.
	router := api.NewRouter(provision, start, list, logger)

	addr := ":8080"
	if v := os.Getenv("PORT"); v != "" {
		addr = ":" + v
	}

	logger.Info("vm-service listening", "addr", addr)
	if err := http.ListenAndServe(addr, router); err != nil {
		log.Fatal(err)
	}
}
