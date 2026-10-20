package main

import (
	"log"
	"net/http"

	"yourplatform/storage-service/internal/api"
	"yourplatform/storage-service/internal/application"
	"yourplatform/storage-service/internal/infrastructure"
)

func main() {
	store := infrastructure.NewFsObjectStore()

	handler := api.NewHandler(
		application.NewUploadHandler(store),
		application.NewListHandler(store))

	mux := http.NewServeMux()
	mux.HandleFunc("PUT /api/v1/objects/{key}", handler.Upload)
	mux.HandleFunc("GET /api/v1/objects", handler.List)
	mux.HandleFunc("GET /healthz", func(w http.ResponseWriter, _ *http.Request) {
		w.WriteHeader(http.StatusOK)
	})

	log.Println("storage-service listening on :8080")
	log.Fatal(http.ListenAndServe(":8080", mux))
}
