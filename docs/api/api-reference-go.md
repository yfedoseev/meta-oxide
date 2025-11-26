# MetaOxide Go API Reference

Complete API documentation for the Go package.

## Table of Contents

- [Installation](#installation)
- [Package Overview](#package-overview)
- [Core Types](#core-types)
- [Functions](#functions)
- [Error Handling](#error-handling)
- [Examples](#examples)

## Installation

```bash
go get github.com/yourusername/meta-oxide-go
```

## Package Overview

```go
package metaoxide

import (
    "C"
    "unsafe"
)
```

## Core Types

### `Extractor`

The main struct for extracting metadata from HTML.

```go
type Extractor struct {
    handle unsafe.Pointer
}
```

#### Constructor

##### `NewExtractor(html, baseURL string) (*Extractor, error)`

Creates a new Extractor instance.

**Parameters:**
- `html string` - The HTML content to parse
- `baseURL string` - Base URL for resolving relative URLs

**Returns:**
- `*Extractor` - The extractor instance
- `error` - Error if creation fails

**Example:**
```go
extractor, err := metaoxide.NewExtractor(html, "https://example.com")
if err != nil {
    log.Fatal(err)
}
defer extractor.Free()
```

**Important:** Always call `Free()` when done to prevent memory leaks.

### `Metadata`

Generic metadata map.

```go
type Metadata map[string]interface{}
```

### `OpenGraphData`

Open Graph metadata structure.

```go
type OpenGraphData struct {
    Title       string `json:"title,omitempty"`
    Type        string `json:"type,omitempty"`
    Image       string `json:"image,omitempty"`
    URL         string `json:"url,omitempty"`
    Description string `json:"description,omitempty"`
    SiteName    string `json:"site_name,omitempty"`
    Locale      string `json:"locale,omitempty"`
}
```

### `TwitterCardData`

Twitter Card metadata structure.

```go
type TwitterCardData struct {
    Card        string `json:"card,omitempty"`
    Site        string `json:"site,omitempty"`
    Creator     string `json:"creator,omitempty"`
    Title       string `json:"title,omitempty"`
    Description string `json:"description,omitempty"`
    Image       string `json:"image,omitempty"`
    ImageAlt    string `json:"image_alt,omitempty"`
}
```

### `JSONLDData`

JSON-LD structured data.

```go
type JSONLDData []map[string]interface{}
```

### `MicrodataItem`

Microdata item structure.

```go
type MicrodataItem struct {
    Type       []string                 `json:"type"`
    Properties map[string][]interface{} `json:"properties"`
    ID         string                   `json:"id,omitempty"`
}
```

### `Link`

Link relation structure.

```go
type Link struct {
    Href     string `json:"href"`
    Rel      string `json:"rel"`
    Media    string `json:"media,omitempty"`
    Title    string `json:"title,omitempty"`
    Type     string `json:"type,omitempty"`
    Hreflang string `json:"hreflang,omitempty"`
}
```

## Functions

### Extraction Methods

#### `ExtractAll() (Metadata, error)`

Extract all metadata formats.

```go
func (e *Extractor) ExtractAll() (Metadata, error)
```

**Returns:**
- `Metadata` - All extracted metadata
- `error` - Error if extraction fails

**Example:**
```go
metadata, err := extractor.ExtractAll()
if err != nil {
    return err
}
fmt.Printf("Title: %v\n", metadata["title"])
```

#### `ExtractBasicMeta() (Metadata, error)`

Extract basic HTML metadata.

```go
func (e *Extractor) ExtractBasicMeta() (Metadata, error)
```

**Returns:**
- `Metadata` - Basic metadata (title, description, etc.)
- `error` - Error if extraction fails

**Example:**
```go
basic, err := extractor.ExtractBasicMeta()
if err != nil {
    return err
}
fmt.Printf("Title: %v\n", basic["title"])
fmt.Printf("Description: %v\n", basic["description"])
```

#### `ExtractOpenGraph() (*OpenGraphData, error)`

Extract Open Graph metadata.

```go
func (e *Extractor) ExtractOpenGraph() (*OpenGraphData, error)
```

**Returns:**
- `*OpenGraphData` - Open Graph data or nil
- `error` - Error if extraction fails

**Example:**
```go
og, err := extractor.ExtractOpenGraph()
if err != nil {
    return err
}
if og != nil {
    fmt.Printf("OG Title: %s\n", og.Title)
    fmt.Printf("OG Image: %s\n", og.Image)
}
```

#### `ExtractTwitterCard() (*TwitterCardData, error)`

Extract Twitter Card metadata.

```go
func (e *Extractor) ExtractTwitterCard() (*TwitterCardData, error)
```

**Returns:**
- `*TwitterCardData` - Twitter Card data or nil
- `error` - Error if extraction fails

**Example:**
```go
twitter, err := extractor.ExtractTwitterCard()
if err != nil {
    return err
}
if twitter != nil {
    fmt.Printf("Card Type: %s\n", twitter.Card)
}
```

#### `ExtractJSONLD() (JSONLDData, error)`

Extract JSON-LD structured data.

```go
func (e *Extractor) ExtractJSONLD() (JSONLDData, error)
```

**Returns:**
- `JSONLDData` - Array of JSON-LD objects
- `error` - Error if extraction fails

**Example:**
```go
jsonld, err := extractor.ExtractJSONLD()
if err != nil {
    return err
}
for _, item := range jsonld {
    fmt.Printf("Type: %v\n", item["@type"])
}
```

#### `ExtractMicrodata() ([]MicrodataItem, error)`

Extract Microdata items.

```go
func (e *Extractor) ExtractMicrodata() ([]MicrodataItem, error)
```

**Returns:**
- `[]MicrodataItem` - Array of Microdata items
- `error` - Error if extraction fails

**Example:**
```go
microdata, err := extractor.ExtractMicrodata()
if err != nil {
    return err
}
for _, item := range microdata {
    fmt.Printf("Type: %v\n", item.Type)
}
```

#### `ExtractMicroformats() (map[string][]Metadata, error)`

Extract Microformats data.

```go
func (e *Extractor) ExtractMicroformats() (map[string][]Metadata, error)
```

**Returns:**
- `map[string][]Metadata` - Microformats grouped by type
- `error` - Error if extraction fails

**Example:**
```go
mf, err := extractor.ExtractMicroformats()
if err != nil {
    return err
}
if hcards, ok := mf["h-card"]; ok {
    for _, card := range hcards {
        fmt.Printf("Name: %v\n", card["name"])
    }
}
```

#### `ExtractRelLinks() (map[string][]Link, error)`

Extract link relations.

```go
func (e *Extractor) ExtractRelLinks() (map[string][]Link, error)
```

**Returns:**
- `map[string][]Link` - Links grouped by rel type
- `error` - Error if extraction fails

**Example:**
```go
links, err := extractor.ExtractRelLinks()
if err != nil {
    return err
}
if canonical, ok := links["canonical"]; ok {
    fmt.Printf("Canonical: %s\n", canonical[0].Href)
}
```

#### `Free()`

Free native resources.

```go
func (e *Extractor) Free()
```

**Important:** Always call this method when done with an Extractor to prevent memory leaks.

**Example:**
```go
extractor, err := metaoxide.NewExtractor(html, url)
if err != nil {
    return err
}
defer extractor.Free()

// Use extractor...
```

## Error Handling

### Error Types

Errors are returned as standard Go errors with descriptive messages.

```go
extractor, err := metaoxide.NewExtractor(html, url)
if err != nil {
    // Handle creation error
    return fmt.Errorf("failed to create extractor: %w", err)
}

metadata, err := extractor.ExtractAll()
if err != nil {
    // Handle extraction error
    return fmt.Errorf("failed to extract metadata: %w", err)
}
```

### Common Errors

- Parse errors: Invalid HTML structure
- URL errors: Invalid base URL
- Extraction errors: Failed to extract specific format

## Examples

### Basic Usage

```go
package main

import (
    "fmt"
    "log"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func main() {
    html := `<!DOCTYPE html>...`

    extractor, err := metaoxide.NewExtractor(html, "https://example.com")
    if err != nil {
        log.Fatal(err)
    }
    defer extractor.Free()

    metadata, err := extractor.ExtractAll()
    if err != nil {
        log.Fatal(err)
    }

    fmt.Printf("Metadata: %+v\n", metadata)
}
```

### Extract from URL

```go
package main

import (
    "fmt"
    "io"
    "log"
    "net/http"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func extractFromURL(url string) (metaoxide.Metadata, error) {
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

func main() {
    metadata, err := extractFromURL("https://example.com")
    if err != nil {
        log.Fatal(err)
    }

    fmt.Printf("Title: %v\n", metadata["title"])
}
```

### Concurrent Extraction

```go
package main

import (
    "fmt"
    "sync"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func extractConcurrent(urls []string) []metaoxide.Metadata {
    var wg sync.WaitGroup
    results := make([]metaoxide.Metadata, len(urls))

    for i, url := range urls {
        wg.Add(1)
        go func(index int, targetURL string) {
            defer wg.Done()

            metadata, err := extractFromURL(targetURL)
            if err != nil {
                fmt.Printf("Error: %v\n", err)
                return
            }

            results[index] = metadata
        }(i, url)
    }

    wg.Wait()
    return results
}
```

### JSON Output

```go
package main

import (
    "encoding/json"
    "fmt"
    "log"

    metaoxide "github.com/yourusername/meta-oxide-go"
)

func main() {
    extractor, err := metaoxide.NewExtractor(html, url)
    if err != nil {
        log.Fatal(err)
    }
    defer extractor.Free()

    metadata, err := extractor.ExtractAll()
    if err != nil {
        log.Fatal(err)
    }

    jsonData, err := json.MarshalIndent(metadata, "", "  ")
    if err != nil {
        log.Fatal(err)
    }

    fmt.Println(string(jsonData))
}
```

### Selective Extraction

```go
func extractSocialMetadata(html, url string) error {
    extractor, err := metaoxide.NewExtractor(html, url)
    if err != nil {
        return err
    }
    defer extractor.Free()

    // Only extract social media metadata
    og, err := extractor.ExtractOpenGraph()
    if err != nil {
        return err
    }

    twitter, err := extractor.ExtractTwitterCard()
    if err != nil {
        return err
    }

    fmt.Printf("OG: %+v\n", og)
    fmt.Printf("Twitter: %+v\n", twitter)

    return nil
}
```

## Thread Safety

The Extractor is thread-safe. Each goroutine should create its own Extractor instance.

```go
// Safe: Each goroutine has its own extractor
for _, url := range urls {
    go func(u string) {
        extractor, _ := metaoxide.NewExtractor(html, u)
        defer extractor.Free()
        // ...
    }(url)
}

// Unsafe: Sharing extractor across goroutines
extractor, _ := metaoxide.NewExtractor(html, url)
for i := 0; i < 10; i++ {
    go func() {
        extractor.ExtractAll() // Don't do this!
    }()
}
```

## Performance Tips

1. **Reuse HTTP Client**: Use a single `http.Client` for multiple requests
2. **Worker Pool**: Limit concurrent extractions with a worker pool
3. **Selective Extraction**: Extract only needed formats
4. **Proper Cleanup**: Always call `Free()` to prevent memory leaks

## See Also

- [Getting Started Guide](/docs/getting-started/getting-started-go.md)
- [Examples](/examples/real-world/go-grpc-service/)
- [Architecture](/docs/architecture/c-abi-design.md)
