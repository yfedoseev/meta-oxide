# Getting Started with MetaOxide (Go)

Welcome to MetaOxide! This guide will help you get started with the Go bindings for MetaOxide in just 5 minutes.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Basic Extraction](#basic-extraction)
- [Concurrency](#concurrency)
- [Next Steps](#next-steps)

## Installation

Install MetaOxide using `go get`:

```bash
go get github.com/yourusername/meta-oxide-go
```

### Requirements

- Go 1.18+
- CGO enabled (for linking with Rust library)
- Works on Linux, macOS, and Windows

## Quick Start

Here's a minimal example to extract metadata from HTML:

```go
package main

import (
    "fmt"
    "log"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func main() {
    html := `
        <!DOCTYPE html>
        <html>
        <head>
            <title>My Page</title>
            <meta name="description" content="A great page">
            <meta property="og:title" content="My Page">
        </head>
        <body>Hello World</body>
        </html>
    `

    // Create extractor
    extractor, err := metaoxide.NewExtractor(html, "https://example.com")
    if err != nil {
        log.Fatal(err)
    }
    defer extractor.Free()

    // Extract all metadata
    metadata, err := extractor.ExtractAll()
    if err != nil {
        log.Fatal(err)
    }

    fmt.Printf("Title: %v\n", metadata["title"])
    fmt.Printf("Description: %v\n", metadata["description"])
}
```

**Important**: Always call `Free()` to release resources when done with an extractor.

## Basic Extraction

MetaOxide supports 13 metadata formats. Here's how to extract specific formats:

### Extract Open Graph Data

```go
package main

import (
    "fmt"
    "log"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func extractOpenGraph(html string) map[string]interface{} {
    extractor, err := metaoxide.NewExtractor(html, "https://example.com")
    if err != nil {
        log.Fatal(err)
    }
    defer extractor.Free()

    ogData, err := extractor.ExtractOpenGraph()
    if err != nil {
        log.Fatal(err)
    }

    if ogData != nil {
        fmt.Printf("OG Title: %v\n", ogData["title"])
        fmt.Printf("OG Type: %v\n", ogData["type"])
        fmt.Printf("OG Image: %v\n", ogData["image"])
    }

    return ogData
}
```

### Extract Twitter Cards

```go
func extractTwitter(html string) map[string]interface{} {
    extractor, err := metaoxide.NewExtractor(html, "https://example.com")
    if err != nil {
        log.Fatal(err)
    }
    defer extractor.Free()

    twitterData, err := extractor.ExtractTwitterCard()
    if err != nil {
        log.Fatal(err)
    }

    if twitterData != nil {
        fmt.Printf("Card Type: %v\n", twitterData["card"])
        fmt.Printf("Title: %v\n", twitterData["title"])
    }

    return twitterData
}
```

### Extract JSON-LD Structured Data

```go
import "encoding/json"

func extractJSONLD(html string) []interface{} {
    extractor, err := metaoxide.NewExtractor(html, "https://example.com")
    if err != nil {
        log.Fatal(err)
    }
    defer extractor.Free()

    jsonldData, err := extractor.ExtractJSONLD()
    if err != nil {
        log.Fatal(err)
    }

    if jsonldData != nil {
        jsonBytes, _ := json.MarshalIndent(jsonldData, "", "  ")
        fmt.Println(string(jsonBytes))
    }

    return jsonldData
}
```

### Extract from URL

```go
import "net/http"
import "io"

func extractFromURL(url string) (map[string]interface{}, error) {
    resp, err := http.Get(url)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()

    body, err := io.ReadAll(resp.Body)
    if err != nil {
        return nil, err
    }

    extractor, err := metaoxide.NewExtractor(string(body), url)
    if err != nil {
        return nil, err
    }
    defer extractor.Free()

    return extractor.ExtractAll()
}
```

## Concurrency

MetaOxide is thread-safe and works great with Go's concurrency primitives:

```go
package main

import (
    "fmt"
    "log"
    "sync"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func extractConcurrently(urls []string) []map[string]interface{} {
    var wg sync.WaitGroup
    results := make([]map[string]interface{}, len(urls))

    for i, url := range urls {
        wg.Add(1)
        go func(index int, targetURL string) {
            defer wg.Done()

            metadata, err := extractFromURL(targetURL)
            if err != nil {
                log.Printf("Failed to extract from %s: %v", targetURL, err)
                return
            }

            results[index] = metadata
        }(i, url)
    }

    wg.Wait()
    return results
}

func main() {
    urls := []string{
        "https://example.com",
        "https://example.org",
        "https://example.net",
    }

    results := extractConcurrently(urls)

    for i, metadata := range results {
        fmt.Printf("%s: %v\n", urls[i], metadata["title"])
    }
}
```

### Worker Pool Pattern

```go
func extractWithWorkerPool(urls []string, workers int) []map[string]interface{} {
    jobs := make(chan string, len(urls))
    results := make(chan map[string]interface{}, len(urls))

    // Start workers
    var wg sync.WaitGroup
    for w := 0; w < workers; w++ {
        wg.Add(1)
        go func() {
            defer wg.Done()
            for url := range jobs {
                metadata, err := extractFromURL(url)
                if err != nil {
                    log.Printf("Error: %v", err)
                    results <- nil
                } else {
                    results <- metadata
                }
            }
        }()
    }

    // Send jobs
    for _, url := range urls {
        jobs <- url
    }
    close(jobs)

    // Wait for completion
    wg.Wait()
    close(results)

    // Collect results
    var allResults []map[string]interface{}
    for result := range results {
        allResults = append(allResults, result)
    }

    return allResults
}
```

## Error Handling

Handle errors appropriately:

```go
func safeExtraction(html, url string) (map[string]interface{}, error) {
    extractor, err := metaoxide.NewExtractor(html, url)
    if err != nil {
        return nil, fmt.Errorf("failed to create extractor: %w", err)
    }
    defer extractor.Free()

    metadata, err := extractor.ExtractAll()
    if err != nil {
        return nil, fmt.Errorf("extraction failed: %w", err)
    }

    fmt.Printf("Extracted %d fields\n", len(metadata))
    return metadata, nil
}
```

## Next Steps

Now that you've got the basics, explore more:

1. **[Complete API Reference](/docs/api/api-reference-go.md)** - Learn about all available methods
2. **[Real-World Examples](/examples/real-world/go-grpc-service/)** - See a complete gRPC service implementation
3. **[Performance Tuning](/docs/performance/performance-tuning-guide.md)** - Optimize for high throughput

### All Supported Formats

MetaOxide extracts these 13 metadata formats:

- Basic HTML metadata (title, description, keywords)
- Open Graph (og:*)
- Twitter Cards (twitter:*)
- JSON-LD structured data
- Microdata (schema.org)
- Microformats (h-card, h-entry, h-event)
- Dublin Core
- RDFA
- HTML5 semantic elements
- Link relations
- Image metadata
- Author information

### Learn More

- [Architecture Overview](/docs/architecture/architecture-overview.md)
- [CGO Integration Details](/docs/architecture/language-binding-patterns.md)
- [Contributing Guide](/docs/architecture/contributing-guide.md)

Happy extracting! 🚀
