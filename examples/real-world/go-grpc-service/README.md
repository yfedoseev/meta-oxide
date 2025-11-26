# MetaOxide gRPC Service

A gRPC service demonstrating how to build a metadata extraction service using MetaOxide in Go.

## What This Example Shows

This gRPC service demonstrates:
- Using MetaOxide Go bindings
- Building scalable gRPC services
- Protocol Buffer definitions
- Concurrent request handling
- Streaming responses
- Error handling in distributed systems
- Health checks
- Service deployment

## Prerequisites

- Go 1.18+ ([Install](https://golang.org/dl/))
- protoc (Protocol Buffer compiler) - [Install](https://grpc.io/docs/protoc-installation/)

## Installation & Setup

```bash
# Install MetaOxide Go module
go get github.com/yfedoseev/meta-oxide-go

# Install gRPC and protobuf dependencies
go get -u google.golang.org/grpc
go get -u google.golang.org/protobuf

# Build the service
go build -o metaoxide-grpc-service

# Build the client
go build -o metaoxide-grpc-client ./cmd/client
```

## Running the Service

```bash
# Start the gRPC server (listens on :50051)
./metaoxide-grpc-service

# Server running on localhost:50051
```

## Using the Service

### gRPC Client

```bash
# Use provided client
./metaoxide-grpc-client

# Or with grpcurl (install: `go install github.com/fullstorydev/grpcurl/cmd/grpcurl@latest`)
grpcurl -plaintext localhost:50051 list
```

### Client Code (Go)

```go
package main

import (
    "context"
    pb "path/to/proto"
    "google.golang.org/grpc"
)

func main() {
    conn, _ := grpc.Dial("localhost:50051", grpc.WithInsecure())
    defer conn.Close()

    client := pb.NewMetaOxideClient(conn)

    req := &pb.ExtractionRequest{
        Html:    "<html>...</html>",
        BaseUrl: "https://example.com",
    }

    resp, _ := client.ExtractAll(context.Background(), req)
    println(resp.Meta)
}
```

## Service Methods

### ExtractAll
Extract all metadata formats from HTML.

```protobuf
rpc ExtractAll(ExtractionRequest) returns (ExtractionResponse);
```

### ExtractFormat
Extract specific metadata format.

```protobuf
rpc ExtractFormat(ExtractFormatRequest) returns (ExtractionResponse);
```

### StreamExtract
Stream extraction results (for batch processing).

```protobuf
rpc StreamExtract(stream ExtractionRequest) returns (stream ExtractionResponse);
```

### Health Check
Standard gRPC health checking.

```bash
grpcurl -plaintext localhost:50051 grpc.health.v1.Health/Check
```

## Performance

Expected performance on standard hardware:

- **Single extraction**: <5ms
- **Concurrent requests (100)**: <100ms
- **Streaming batches**: Linear with batch size

**Load testing:**
```bash
# Install ghz
go install github.com/bojand/ghz@latest

# Benchmark the service
ghz --insecure \
  --proto ./proto/metaoxide.proto \
  --call metaoxide.MetaOxide/ExtractAll \
  -d '{"html":"<html>...</html>","base_url":"https://example.com"}' \
  -n 1000 \
  -c 10 \
  localhost:50051
```

## Architecture

### Main Components

1. **Proto Definition** (`proto/metaoxide.proto`)
   - Service definition
   - Request/Response messages
   - RPC methods

2. **Server** (`cmd/server/main.go`)
   - gRPC server setup
   - Service implementation
   - Health checks

3. **Client** (`cmd/client/main.go`)
   - Example client usage
   - Request building
   - Response handling

### Key Code Pattern

```go
type server struct{}

func (s *server) ExtractAll(ctx context.Context, req *pb.ExtractionRequest) (*pb.ExtractionResponse, error) {
    extractor := metaoxide.New(req.Html, req.BaseUrl)
    result, err := extractor.ExtractAll()
    if err != nil {
        return nil, status.Error(codes.Internal, err.Error())
    }

    return &pb.ExtractionResponse{
        Meta:         result.Meta,
        OpenGraph:    result.OpenGraph,
        // ... other fields
    }, nil
}
```

## Deployment

### Docker Deployment

```dockerfile
FROM golang:1.21 as builder
WORKDIR /app
COPY . .
RUN go build -o metaoxide-grpc-service

FROM scratch
COPY --from=builder /app/metaoxide-grpc-service /
EXPOSE 50051
ENTRYPOINT ["/metaoxide-grpc-service"]
```

Build and run:
```bash
docker build -t metaoxide-grpc .
docker run -p 50051:50051 metaoxide-grpc
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: metaoxide-grpc
spec:
  replicas: 3
  selector:
    matchLabels:
      app: metaoxide-grpc
  template:
    metadata:
      labels:
        app: metaoxide-grpc
    spec:
      containers:
      - name: metaoxide-grpc
        image: metaoxide-grpc:latest
        ports:
        - containerPort: 50051
        livenessProbe:
          exec:
            command: ["/bin/grpc_health_probe", "-addr=:50051"]
          initialDelaySeconds: 5
```

## Testing

Run tests:
```bash
go test ./...

# With coverage
go test -cover ./...

# With verbose output
go test -v ./...
```

## Concurrent Requests

The service handles concurrent requests efficiently:

```go
// Client can make concurrent requests safely
var wg sync.WaitGroup
for i := 0; i < 100; i++ {
    wg.Add(1)
    go func() {
        defer wg.Done()
        resp, _ := client.ExtractAll(ctx, req)
        // Process response
    }()
}
wg.Wait()
```

## Monitoring

### Prometheus Metrics

Add Prometheus middleware:

```bash
go get github.com/grpc-ecosystem/go-grpc-prometheus
```

```go
grpcMetrics := grpc_prometheus.NewServerMetrics()
grpcMetrics.Register(server)
```

### Logging

Use structured logging:

```bash
go get go.uber.org/zap
```

```go
logger, _ := zap.NewProduction()
logger.Info("Extraction request", zap.String("format", req.Format))
```

## Learning Resources

- [MetaOxide Getting Started (Go)](../../../docs/getting-started/getting-started-go.md)
- [Go API Reference](../../../docs/api/api-reference-go.md)
- [gRPC Documentation](https://grpc.io/docs/)
- [Protocol Buffers Guide](https://developers.google.com/protocol-buffers)

## Troubleshooting

### Port Already in Use
```bash
# Use different port
PORT=50052 ./metaoxide-grpc-service

# Or kill process
lsof -ti:50051 | xargs kill -9
```

### Proto Compilation Issues
```bash
# Regenerate proto files
protoc --go_out=. --go-grpc_out=. proto/*.proto
```

## Extensions

### Add TLS/SSL
Secure the gRPC connection:

```go
creds, _ := credentials.NewServerTLSFromFile("cert.pem", "key.pem")
listener, _ := net.Listen("tcp", ":50051")
server := grpc.NewServer(grpc.Creds(creds))
```

### Add Authentication
Use token-based authentication:

```go
type AuthInterceptor struct{}

func (ai *AuthInterceptor) Unary() grpc.UnaryServerInterceptor {
    return func(ctx context.Context, req interface{}, info *grpc.UnaryServerInfo, handler grpc.UnaryHandler) (interface{}, error) {
        // Check auth token
        return handler(ctx, req)
    }
}
```

## License

This example is licensed under the same dual license as MetaOxide: **MIT OR Apache-2.0**

---

**Questions?** Check the main [MetaOxide documentation](../../../README.md) or open an issue.
