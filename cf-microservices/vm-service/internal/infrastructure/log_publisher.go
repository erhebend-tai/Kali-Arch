package infrastructure

import (
	"context"
	"log"

	"yourplatform/vm-service/internal/domain"
)

// LogEventPublisher — dev fallback. Replace with a real
// broker (Kafka, NATS, Dapr pubsub) implementing EventPublisher.
type LogEventPublisher struct{}

func (LogEventPublisher) Publish(
	_ context.Context, event domain.DomainEvent,
) error {
	log.Printf("event published: %s at %s",
		event.EventName(), event.OccurredAt())
	return nil
}
