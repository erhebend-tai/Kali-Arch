package domain

import (
	"errors"
	"io"
	"time"
)

var ErrObjectNotFound = errors.New("object not found")

// Object — stored blob metadata.
type Object struct {
	Key         string
	Size        int64
	ContentType string
	UploadedAt  time.Time
}

// ObjectStore — persistence contract; implemented by
// S3 / GCS / OCI Object Storage / R2 / filesystem.
type ObjectStore interface {
	Put(key string, r io.Reader, contentType string) error
	Get(key string) (io.ReadCloser, error)
	Stat(key string) (*Object, error)
	Delete(key string) error
	List() ([]*Object, error)
}
