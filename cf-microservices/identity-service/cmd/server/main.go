package main

import (
	"log"
	"net/http"
	"sync"

	"yourplatform/identity-service/internal/api"
	"yourplatform/identity-service/internal/application"
	"yourplatform/identity-service/internal/domain"
)

// In-memory repo + trivial token issuer — swap for real
// implementations behind the same interfaces.
type memRepo struct {
	mu    sync.Mutex
	users map[string]*domain.User
}

func (r *memRepo) ByEmail(email string) (*domain.User, error) {
	r.mu.Lock()
	defer r.mu.Unlock()
	return r.users[email], nil
}

func (r *memRepo) Add(u *domain.User) error {
	r.mu.Lock()
	defer r.mu.Unlock()
	r.users[u.Email] = u
	return nil
}

type devIssuer struct{}

func (devIssuer) Issue(userID string, _ []string) (string, error) {
	return "dev-token-" + userID, nil
}

func main() {
	repo := &memRepo{users: map[string]*domain.User{}}
	handler := api.NewHandler(
		application.NewAuthenticateHandler(repo, devIssuer{}))

	mux := http.NewServeMux()
	mux.HandleFunc("POST /api/v1/login", handler.Login)
	mux.HandleFunc("GET /healthz", func(w http.ResponseWriter, _ *http.Request) {
		w.WriteHeader(http.StatusOK)
	})

	log.Println("identity-service listening on :8080")
	log.Fatal(http.ListenAndServe(":8080", mux))
}
