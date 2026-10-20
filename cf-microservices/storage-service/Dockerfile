# Build from repo root:
#   docker build -f cf-microservices/storage-service/Dockerfile -t cf-storage-service ./cf-microservices/storage-service
FROM golang:1.23-alpine AS build
WORKDIR /src
COPY . .
RUN go build -o /out/service ./cmd

FROM alpine:latest
COPY --from=build /out/service /service
ENTRYPOINT ["/service"]
