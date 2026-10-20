package api

import (
	"encoding/json"
	"net/http"

	"yourplatform/identity-service/internal/application"
	"yourplatform/identity-service/internal/domain"
)

type Handler struct {
	auth *application.AuthenticateHandler
}

func NewHandler(auth *application.AuthenticateHandler) *Handler {
	return &Handler{auth: auth}
}

type loginRequest struct {
	Email    string `json:"email"`
	Password string `json:"password"`
}

type loginResponse struct {
	Token string `json:"token"`
}

func (h *Handler) Login(w http.ResponseWriter, r *http.Request) {
	var req loginRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "invalid body", http.StatusBadRequest)
		return
	}

	token, err := h.auth.Handle(req.Email, req.Password)
	if err != nil {
		if err == domain.ErrInvalidCredentials {
			http.Error(w, "invalid credentials", http.StatusUnauthorized)
			return
		}
		http.Error(w, "internal error", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(loginResponse{Token: token})
}
