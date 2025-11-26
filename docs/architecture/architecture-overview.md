# MetaOxide Architecture Overview

Comprehensive overview of MetaOxide's architecture, design decisions, and system components.

## Table of Contents

- [System Architecture](#system-architecture)
- [Core Components](#core-components)
- [Language Bindings](#language-bindings)
- [Data Flow](#data-flow)
- [Design Decisions](#design-decisions)
- [Extension Points](#extension-points)

## System Architecture

### High-Level Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     Application Layer                            │
│  (Rust, Python, Go, Node.js, Java, C#, WebAssembly)            │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────────┐
│                  Language Bindings Layer                         │
│  ┌─────────┬─────────┬──────────┬─────────┬────────┬──────┐   │
│  │ PyO3    │ CGO     │ N-API    │ JNI     │ P/Invoke│ WASM │   │
│  └─────────┴─────────┴──────────┴─────────┴────────┴──────┘   │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────────┐
│                    C-ABI Layer                                   │
│  (Foreign Function Interface - Thread-Safe, Memory-Safe)        │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────────┐
│                    Rust Core                                     │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  HTML Parser (html5ever)                                  │  │
│  └──────────────────┬───────────────────────────────────────┘  │
│                     │                                            │
│  ┌──────────────────▼───────────────────────────────────────┐  │
│  │  Extractor Router                                         │  │
│  │  (Dispatch to specialized extractors)                     │  │
│  └──┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬──────┘  │
│     │   │   │   │   │   │   │   │   │   │   │   │   │         │
│  ┌──▼───▼───▼───▼───▼───▼───▼───▼───▼───▼───▼───▼───▼──────┐  │
│  │  Metadata Extractors (13 formats)                        │  │
│  │  • BasicMeta  • OpenGraph  • TwitterCard  • JSON-LD      │  │
│  │  • Microdata  • Microformats  • DublinCore  • RDFa       │  │
│  │  • RelLinks   • Manifest   • SEO   • Images  • Authors   │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Utilities                                                 │  │
│  │  • URL Resolution  • Text Normalization  • Date Parsing   │  │
│  └───────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. HTML Parser

**Technology**: html5ever (Mozilla's HTML5 parser)

**Responsibilities**:
- Parse HTML into DOM tree
- Handle malformed HTML gracefully
- Support HTML5 specification
- Provide fast, standards-compliant parsing

**Key Features**:
- Zero-copy parsing where possible
- Streaming support for large documents
- Error recovery for real-world HTML

### 2. Extractor Router

**Purpose**: Coordinate extraction across all metadata formats

**Design Pattern**: Strategy Pattern

```rust
pub trait MetadataExtractor {
    type Output;

    fn extract(&self, document: &Html) -> Result<Self::Output>;
    fn can_extract(&self, document: &Html) -> bool;
}
```

**Flow**:
1. Parse HTML once into DOM
2. Pass DOM to each extractor
3. Extractors scan for their specific patterns
4. Aggregate results into unified structure

### 3. Metadata Extractors

#### Basic Meta Extractor
- Extracts `<title>`, `<meta>` tags
- Handles canonical URLs
- Processes viewport, charset, language
- SEO metadata (keywords, description, robots)

#### Social Media Extractors
- **OpenGraph**: og:* properties
- **TwitterCard**: twitter:* meta tags
- Supports rich media (images, videos, audio)

#### Structured Data Extractors
- **JSON-LD**: Parses `<script type="application/ld+json">`
- **Microdata**: schema.org vocabulary
- **Microformats**: h-card, h-entry, h-event, etc.
- **RDFa**: Resource Description Framework attributes
- **Dublin Core**: DC metadata elements

#### Semantic Extractors
- **RelLinks**: link[@rel] elements
- **Manifest**: Web app manifest
- **Images**: Metadata from img tags
- **Authors**: Author information extraction

### 4. C-ABI Layer

**Purpose**: Provide stable interface for language bindings

**Design Principles**:
- **Opaque Pointers**: Hide Rust internals
- **Manual Memory Management**: Explicit alloc/free
- **Error Handling**: Integer error codes + error messages
- **Thread Safety**: No shared mutable state

**Example**:
```c
// C-ABI function signature
typedef void* MetaOxideHandle;

MetaOxideHandle meta_oxide_new(const char* html, const char* base_url, char** error);
char* meta_oxide_extract_all(MetaOxideHandle handle, char** error);
void meta_oxide_free(MetaOxideHandle handle);
void meta_oxide_free_string(char* str);
```

## Language Bindings

### Python (PyO3)

**Binding Type**: Native extension module

**Architecture**:
```
Python Code
     ↓
  PyO3 Bindings (Rust)
     ↓
  Rust Core
```

**Key Features**:
- Zero-copy data transfer where possible
- Automatic memory management via Python GC
- Native Python exceptions
- Type hints support

**Performance**: Near-native Rust performance

### Go (CGO)

**Binding Type**: CGO foreign function interface

**Architecture**:
```
Go Code
     ↓
  CGO Wrapper
     ↓
  C-ABI Layer
     ↓
  Rust Core
```

**Key Features**:
- Manual memory management (Free() required)
- Thread-safe concurrent access
- Idiomatic Go error handling
- JSON serialization support

**Performance**: Minimal CGO overhead (~5%)

### Node.js (N-API)

**Binding Type**: Native addon via N-API

**Architecture**:
```
JavaScript/TypeScript
     ↓
  N-API Bindings (Rust)
     ↓
  Rust Core
```

**Key Features**:
- Node-API stability
- Automatic garbage collection
- Promise-based async support (future)
- TypeScript definitions included

**Performance**: 10-15% overhead vs Rust

### Java (JNI)

**Binding Type**: Java Native Interface

**Architecture**:
```
Java Code
     ↓
  JNI Wrapper
     ↓
  C-ABI Layer
     ↓
  Rust Core
```

**Key Features**:
- AutoCloseable for resource management
- Exception mapping
- Works on Android
- Thread-safe

**Performance**: JNI overhead (~15-20%)

### C# (P/Invoke)

**Binding Type**: Platform Invoke

**Architecture**:
```
C# Code
     ↓
  P/Invoke Marshaling
     ↓
  C-ABI Layer
     ↓
  Rust Core
```

**Key Features**:
- IDisposable pattern
- .NET Framework & .NET Core support
- Async/await compatible
- LINQ-friendly

**Performance**: Minimal P/Invoke overhead (~5-10%)

### WebAssembly (wasm-bindgen)

**Binding Type**: WebAssembly module

**Architecture**:
```
JavaScript/TypeScript
     ↓
  wasm-bindgen Glue Code
     ↓
  WASM Module (Rust compiled to WASM)
```

**Key Features**:
- Browser and Node.js support
- TypeScript definitions
- Memory managed by JS GC
- Small binary size (350KB gzipped)

**Performance**: 20-30% overhead vs native

## Data Flow

### Extraction Flow

```
1. Input: HTML string + base URL
         ↓
2. Parse HTML (html5ever)
         ↓
3. Build DOM tree
         ↓
4. Pass to extractors in parallel/sequential
         ↓
5. Each extractor scans DOM
         ↓
6. Aggregate results
         ↓
7. Serialize to language-native format
         ↓
8. Return to caller
```

### Memory Flow

```
[Rust Heap]                     [Language Heap]
     │                                │
     │  1. Parse HTML                 │
     │  2. Create DOM                 │
     │     (owned by Rust)            │
     │                                │
     │  3. Extract metadata           │
     │     (Rust allocations)         │
     │                                │
     │  4. Serialize ────────────────>│ 5. Deserialize
     │     (JSON/msgpack)             │    (language objects)
     │                                │
     │  6. Free Rust memory           │ 7. Manage via GC
     │     (manual or automatic)      │    (or manual)
     │                                │
```

## Design Decisions

### Why Rust for Core?

1. **Performance**: Zero-cost abstractions, no GC pauses
2. **Safety**: Memory safety without runtime overhead
3. **Concurrency**: Fearless concurrency
4. **FFI**: Excellent C-ABI support for bindings
5. **Ecosystem**: Mature HTML parsing (html5ever)

### Why C-ABI Layer?

1. **Stability**: C ABI is stable across compiler versions
2. **Portability**: All languages can call C functions
3. **Simplicity**: Easier than language-specific interfaces
4. **Versioning**: Clear ABI versioning boundaries

### Why Separate Extractors?

1. **Modularity**: Each format is independent
2. **Testability**: Easy to test each extractor in isolation
3. **Extensibility**: Add new formats without modifying core
4. **Performance**: Can parallelize extraction
5. **Maintenance**: Clear separation of concerns

### Memory Management Strategy

**Rust Core**:
- Ownership system ensures memory safety
- No garbage collection overhead
- Predictable deallocation

**Language Bindings**:
- **Python/Node.js/Java/WASM**: Leverage language GC
- **Go/C#**: Manual Free()/Dispose() required
- **All**: No memory leaks even with exceptions

### Error Handling Philosophy

**Rust**: Result<T, E> for recoverable errors, panic for unrecoverable

**Bindings**:
- **Python**: Exceptions
- **Go**: (result, error) tuples
- **Node.js**: Exceptions
- **Java**: Checked exceptions
- **C#**: Exceptions
- **WASM**: Exceptions

## Extension Points

### Adding New Metadata Format

1. Create new extractor module:
```rust
// src/extractors/my_format/mod.rs
pub struct MyFormatExtractor;

impl MetadataExtractor for MyFormatExtractor {
    type Output = MyFormatData;

    fn extract(&self, document: &Html) -> Result<Self::Output> {
        // Extraction logic
    }
}
```

2. Register in extractor router:
```rust
// src/extractors/mod.rs
pub mod my_format;

pub fn extract_all(document: &Html) -> AllMetadata {
    // ...
    my_format: MyFormatExtractor.extract(document)?,
    // ...
}
```

3. Update C-ABI (if needed):
```rust
// src/ffi.rs
#[no_mangle]
pub extern "C" fn meta_oxide_extract_my_format(
    handle: *mut MetaOxideHandle,
    error: *mut *mut c_char
) -> *mut c_char {
    // ...
}
```

4. Update language bindings

### Custom Extractor Example

```rust
use meta_oxide::extractors::MetadataExtractor;
use scraper::{Html, Selector};

pub struct PriceExtractor;

impl MetadataExtractor for PriceExtractor {
    type Output = Option<f64>;

    fn extract(&self, document: &Html) -> Result<Self::Output> {
        let selector = Selector::parse("[itemprop='price']").unwrap();

        for element in document.select(&selector) {
            if let Some(content) = element.value().attr("content") {
                if let Ok(price) = content.parse::<f64>() {
                    return Ok(Some(price));
                }
            }
        }

        Ok(None)
    }

    fn can_extract(&self, document: &Html) -> bool {
        document.select(&Selector::parse("[itemprop='price']").unwrap())
            .next()
            .is_some()
    }
}
```

## Thread Safety

### Rust Core

- **Immutable by default**: DOM tree is read-only after parsing
- **No shared mutable state**: Each extraction is independent
- **Send + Sync**: All types are thread-safe
- **Parallel extraction**: Can use rayon for parallel processing

### Language Bindings

- **Python**: Thread-safe (GIL-protected)
- **Go**: Thread-safe (goroutine-safe)
- **Node.js**: Single-threaded event loop (thread-safe within Node)
- **Java**: Thread-safe (synchronized access)
- **C#**: Thread-safe (lock-free where possible)
- **WASM**: Single-threaded in browser, thread-safe in Node

## Performance Characteristics

### Time Complexity

- **Parsing**: O(n) where n = HTML length
- **Extraction**: O(n × m) where m = number of extractors
- **Total**: O(n × m) but typically m is constant (13)

### Space Complexity

- **DOM tree**: O(n) where n = number of HTML nodes
- **Metadata**: O(k) where k = extracted metadata size
- **Peak memory**: O(n + k), typically k << n

### Optimization Strategies

1. **Lazy extraction**: Only extract requested formats
2. **Selector caching**: Reuse compiled CSS selectors
3. **String interning**: Deduplicate common strings
4. **Zero-copy**: Minimize allocations

## Versioning Strategy

### Semantic Versioning

- **Major**: Breaking API changes
- **Minor**: New features, backward compatible
- **Patch**: Bug fixes, backward compatible

### ABI Stability

- C-ABI remains stable within major version
- Language bindings may evolve independently
- Clear upgrade path documented

## See Also

- [C-ABI Design](/docs/architecture/c-abi-design.md)
- [Language Binding Patterns](/docs/architecture/language-binding-patterns.md)
- [Testing Strategy](/docs/architecture/testing-strategy.md)
- [Contributing Guide](/docs/architecture/contributing-guide.md)
