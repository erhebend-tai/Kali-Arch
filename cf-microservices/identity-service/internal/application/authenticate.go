package application

import (
	"fmt"

	"golang.org/x/crypto/bcrypt"

	"yourplatform/identity-service/internal/domain"
)

// TokenIssuer — infrastructure fulfills this (JWT, opaque tokens).
type TokenIssuer interface {
	Issue(userID string, roles []string) (string, error)
}

// AuthenticateHandler — the login use case.
type AuthenticateHandler struct {
	repo   domain.UserRepository
	issuer TokenIssuer
}

func NewAuthenticateHandler(
	repo domain.UserRepository,
	issuer TokenIssuer,
) *AuthenticateHandler {
	return &AuthenticateHandler{repo: repo, issuer: issuer}
}

func (h *AuthenticateHandler) Handle(
	email, password string,
) (string, error) {
	user, err := h.repo.ByEmail(email)
	if err != nil || user == nil {
		return "", domain.ErrInvalidCredentials
	}

	if err := bcrypt.CompareHashAndPassword(
		[]byte(user.PasswordHash), []byte(password)); err != nil {
		return "", fmt.Errorf("%w", domain.ErrInvalidCredentials)
	}

	return h.issuer.Issue(user.ID, user.Roles)
}
