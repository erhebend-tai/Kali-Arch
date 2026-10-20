package domain

import (
	"errors"
	"time"
)

var ErrInvalidCredentials = errors.New("invalid credentials")

// User — identity aggregate.
type User struct {
	ID           string
	Email        string
	PasswordHash string
	Roles        []string
	CreatedAt    time.Time
}

// NewUser — validated creation.
func NewUser(id, email, passwordHash string) (*User, error) {
	if email == "" || passwordHash == "" {
		return nil, ErrInvalidCredentials
	}
	return &User{
		ID:           id,
		Email:        email,
		PasswordHash: passwordHash,
		Roles:        []string{"user"},
		CreatedAt:    time.Now().UTC(),
	}, nil
}

// UserRepository — persistence contract.
type UserRepository interface {
	ByEmail(email string) (*User, error)
	Add(u *User) error
}
