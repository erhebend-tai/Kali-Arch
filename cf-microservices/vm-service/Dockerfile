# Build from repo root:
#   docker build -f cf-microservices/vm-service/Dockerfile -t cf-vm-service ./cf-microservices/vm-service
FROM golang:1.23-alpine AS build
WORKDIR /src
COPY . .
RUN go build -o /out/service ./cmd

FROM alpine:latest
COPY --from=build /out/service /service
ENTRYPOINT ["/service"]
