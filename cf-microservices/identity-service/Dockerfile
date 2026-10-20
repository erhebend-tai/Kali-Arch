# Build from repo root:
#   docker build -f cf-microservices/identity-service/Dockerfile -t cf-identity-service ./cf-microservices/identity-service
FROM golang:1.23-alpine AS build
WORKDIR /src
COPY . .
RUN go build -o /out/service ./cmd

FROM alpine:latest
COPY --from=build /out/service /service
ENTRYPOINT ["/service"]
