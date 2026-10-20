package infrastructure

import (
	"bytes"
	"io"
	"sync"
	"time"

	"yourplatform/storage-service/internal/domain"
)

// FsObjectStore — in-memory dev implementation of ObjectStore.
type FsObjectStore struct {
	mu      sync.RWMutex
	objects map[string]*stored
}

type stored struct {
	meta *domain.Object
	data []byte
}

func NewFsObjectStore() *FsObjectStore {
	return &FsObjectStore{objects: map[string]*stored{}}
}

func (s *FsObjectStore) Put(
	key string, r io.Reader, contentType string,
) error {
	data, err := io.ReadAll(r)
	if err != nil {
		return err
	}

	s.mu.Lock()
	defer s.mu.Unlock()
	s.objects[key] = &stored{
		meta: &domain.Object{
			Key:         key,
			Size:        int64(len(data)),
			ContentType: contentType,
			UploadedAt:  time.Now().UTC(),
		},
		data: data,
	}
	return nil
}

func (s *FsObjectStore) Get(key string) (io.ReadCloser, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()
	obj, ok := s.objects[key]
	if !ok {
		return nil, domain.ErrObjectNotFound
	}
	return io.NopCloser(bytes.NewReader(obj.data)), nil
}

func (s *FsObjectStore) Stat(key string) (*domain.Object, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()
	obj, ok := s.objects[key]
	if !ok {
		return nil, domain.ErrObjectNotFound
	}
	return obj.meta, nil
}

func (s *FsObjectStore) Delete(key string) error {
	s.mu.Lock()
	defer s.mu.Unlock()
	delete(s.objects, key)
	return nil
}

func (s *FsObjectStore) List() ([]*domain.Object, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()
	out := make([]*domain.Object, 0, len(s.objects))
	for _, o := range s.objects {
		out = append(out, o.meta)
	}
	return out, nil
}
