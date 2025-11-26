# MetaOxide C API Usage Guide

Complete guide for using the MetaOxide C library to extract metadata from HTML documents.

## Table of Contents

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Quick Start](#quick-start)
4. [API Reference](#api-reference)
5. [Memory Management](#memory-management)
6. [Error Handling](#error-handling)
7. [Thread Safety](#thread-safety)
8. [Examples](#examples)
9. [Performance Tips](#performance-tips)

## Introduction

MetaOxide provides a comprehensive C API for extracting 13 different types of metadata from HTML documents:

- **Standard HTML meta tags** (title, description, keywords, etc.)
- **Open Graph** (Facebook, LinkedIn rich previews)
- **Twitter Cards** (Twitter rich previews)
- **JSON-LD** (Schema.org structured data)
- **Microdata** (HTML5 structured data)
- **Microformats** (h-card, h-entry, h-event, h-review, h-recipe, h-product, h-feed, h-adr, h-geo)
- **RDFa** (W3C structured data standard)
- **Dublin Core** (Library/archive metadata)
- **Web App Manifest** (PWA configuration)
- **oEmbed** (Embeddable content discovery)
- **rel-* links** (HTML link relationships)

All extraction functions return JSON strings for easy integration with any system.

## Installation

### Building from Source

```bash
# Clone the repository
git clone https://github.com/yfedoseev/meta_oxide
cd meta_oxide

# Build the C library
cargo build --release

# The library will be in target/release/
# - Linux: libmeta_oxide.so
# - macOS: libmeta_oxide.dylib
# - Windows: meta_oxide.dll

# The C header is in include/meta_oxide.h
```

### Linking Against MetaOxide

**GCC/Clang:**
```bash
gcc -I/path/to/meta_oxide/include \
    -L/path/to/meta_oxide/target/release \
    -o myapp myapp.c \
    -lmeta_oxide -lpthread -ldl -lm
```

**CMakeLists.txt:**
```cmake
find_library(META_OXIDE_LIB meta_oxide HINTS /path/to/target/release)
include_directories(/path/to/include)
target_link_libraries(myapp ${META_OXIDE_LIB} pthread dl m)
```

**Makefile:**
```makefile
CFLAGS = -I/path/to/include
LDFLAGS = -L/path/to/target/release -lmeta_oxide -lpthread -ldl -lm

myapp: myapp.c
    $(CC) $(CFLAGS) -o myapp myapp.c $(LDFLAGS)
```

## Quick Start

```c
#include "meta_oxide.h"
#include <stdio.h>

int main() {
    const char* html =
        "<html>"
        "<head>"
        "  <title>Example Page</title>"
        "  <meta name=\"description\" content=\"A test page\">"
        "  <meta property=\"og:image\" content=\"https://example.com/image.jpg\">"
        "</head>"
        "</html>";

    // Extract all metadata at once
    MetaOxideResult* result = meta_oxide_extract_all(html, "https://example.com");

    if (result == NULL) {
        fprintf(stderr, "Error: %s\n", meta_oxide_error_message());
        return 1;
    }

    // Access individual fields
    if (result->meta != NULL) {
        printf("Meta tags: %s\n", result->meta);
    }

    if (result->open_graph != NULL) {
        printf("Open Graph: %s\n", result->open_graph);
    }

    // Clean up (essential to prevent memory leaks!)
    meta_oxide_result_free(result);
    return 0;
}
```

## API Reference

### Core Function: Extract All Metadata

```c
MetaOxideResult* meta_oxide_extract_all(
    const char* html,      // HTML content (required)
    const char* base_url   // Base URL for resolving relative URLs (optional, can be NULL)
);
```

Extracts all supported metadata formats in a single call. Returns a `MetaOxideResult` struct containing JSON strings for each format, or `NULL` on error.

**Result Structure:**
```c
typedef struct MetaOxideResult {
    char* meta;           // Standard HTML meta tags (JSON object)
    char* open_graph;     // Open Graph metadata (JSON object)
    char* twitter;        // Twitter Card metadata (JSON object)
    char* json_ld;        // JSON-LD structured data (JSON array)
    char* microdata;      // Microdata items (JSON array)
    char* microformats;   // Microformats data (JSON object)
    char* rdfa;           // RDFa structured data (JSON array)
    char* dublin_core;    // Dublin Core metadata (JSON object)
    char* manifest;       // Web App Manifest discovery (JSON object)
    char* oembed;         // oEmbed endpoint discovery (JSON object)
    char* rel_links;      // rel-* link relationships (JSON object)
} MetaOxideResult;
```

Each field is either a JSON string or `NULL` if no data was found.

### Individual Extractors

For performance-critical applications where you only need specific metadata types:

```c
// Standard meta tags
char* meta_oxide_extract_meta(const char* html, const char* base_url);

// Social media metadata
char* meta_oxide_extract_open_graph(const char* html, const char* base_url);
char* meta_oxide_extract_twitter(const char* html, const char* base_url);

// Structured data
char* meta_oxide_extract_json_ld(const char* html, const char* base_url);
char* meta_oxide_extract_microdata(const char* html, const char* base_url);
char* meta_oxide_extract_microformats(const char* html, const char* base_url);
char* meta_oxide_extract_rdfa(const char* html, const char* base_url);

// Other formats
char* meta_oxide_extract_dublin_core(const char* html);
char* meta_oxide_extract_manifest(const char* html, const char* base_url);
char* meta_oxide_extract_oembed(const char* html, const char* base_url);
char* meta_oxide_extract_rel_links(const char* html, const char* base_url);
```

All functions return a JSON string or `NULL` on error. Strings must be freed with `meta_oxide_string_free()`.

### Manifest Parsing

```c
// Parse manifest.json content
char* meta_oxide_parse_manifest(const char* json, const char* base_url);
```

Parses a Web App Manifest JSON file and resolves all relative URLs.

### Error Handling

```c
// Get the last error code (0 = no error)
int meta_oxide_last_error(void);

// Get a human-readable error message
const char* meta_oxide_error_message(void);
```

### Memory Management

```c
// Free a MetaOxideResult struct
void meta_oxide_result_free(MetaOxideResult* result);

// Free a string returned by any extractor function
void meta_oxide_string_free(char* s);

// Free a ManifestDiscovery struct
void meta_oxide_manifest_discovery_free(ManifestDiscovery* discovery);
```

### Utility Functions

```c
// Get the library version
const char* meta_oxide_version(void);
```

## Memory Management

**Critical Rules:**

1. **All returned pointers are owned by the caller** - you must free them
2. **Use the correct free function:**
   - `MetaOxideResult*` → `meta_oxide_result_free()`
   - `char*` (strings) → `meta_oxide_string_free()`
   - `ManifestDiscovery*` → `meta_oxide_manifest_discovery_free()`
3. **Never use `free()` directly** - always use MetaOxide's free functions
4. **NULL pointers are safe to free** - all free functions check for NULL

**Example:**
```c
// Good
MetaOxideResult* result = meta_oxide_extract_all(html, NULL);
if (result) {
    // Use result...
    meta_oxide_result_free(result);  // Correct!
}

// Bad - will leak memory!
MetaOxideResult* result = meta_oxide_extract_all(html, NULL);
// Missing meta_oxide_result_free(result);
```

**Valgrind Check:**
```bash
valgrind --leak-check=full ./your_program
# Should report: "All heap blocks were freed -- no leaks are possible"
```

## Error Handling

MetaOxide uses return codes and thread-local error state:

```c
char* meta = meta_oxide_extract_meta(html, NULL);
if (meta == NULL) {
    // Error occurred
    int error_code = meta_oxide_last_error();
    const char* error_msg = meta_oxide_error_message();
    fprintf(stderr, "Error %d: %s\n", error_code, error_msg);
    return 1;
}

// Success - use meta
printf("%s\n", meta);
meta_oxide_string_free(meta);
```

**Error Codes:**
- `0` - No error (META_OXIDE_OK)
- `1` - HTML parsing error
- `2` - Invalid URL format
- `3` - Invalid UTF-8 string
- `4` - Memory allocation error
- `5` - JSON serialization error
- `6` - NULL pointer passed as argument

**Best Practices:**
- Always check for NULL returns
- Check errors immediately after failed calls
- Error state is thread-local (safe for multithreading)
- Error messages are human-readable and suitable for logging

## Thread Safety

**All MetaOxide functions are thread-safe:**

- No global state (except thread-local error storage)
- All functions are stateless
- Safe to call from multiple threads simultaneously
- No locking required by the caller

**Example Multithreaded Usage:**
```c
#include <pthread.h>

void* worker_thread(void* arg) {
    const char* html = (const char*)arg;

    // Each thread has its own error state
    MetaOxideResult* result = meta_oxide_extract_all(html, NULL);
    if (result == NULL) {
        fprintf(stderr, "Thread error: %s\n", meta_oxide_error_message());
        return NULL;
    }

    // Process result...
    meta_oxide_result_free(result);
    return NULL;
}

int main() {
    pthread_t threads[4];
    const char* htmls[4] = { /* ... */ };

    // Safe to call from multiple threads
    for (int i = 0; i < 4; i++) {
        pthread_create(&threads[i], NULL, worker_thread, (void*)htmls[i]);
    }

    for (int i = 0; i < 4; i++) {
        pthread_join(threads[i], NULL);
    }
}
```

## Examples

### Example 1: News Article Metadata

```c
const char* html =
    "<html><head>"
    "<title>Breaking News: Example Event</title>"
    "<meta property=\"og:type\" content=\"article\">"
    "<meta property=\"og:image\" content=\"https://news.example/image.jpg\">"
    "<script type=\"application/ld+json\">"
    "{"
    "  \"@type\": \"NewsArticle\","
    "  \"headline\": \"Breaking News\","
    "  \"datePublished\": \"2025-01-15T10:00:00Z\","
    "  \"author\": {\"@type\": \"Person\", \"name\": \"Jane Reporter\"}"
    "}"
    "</script>"
    "</head></html>";

MetaOxideResult* result = meta_oxide_extract_all(html, "https://news.example");

// Parse the JSON-LD to get structured article data
if (result->json_ld) {
    // Use a JSON parser library to parse result->json_ld
    printf("JSON-LD: %s\n", result->json_ld);
}

meta_oxide_result_free(result);
```

### Example 2: Product Page

```c
const char* html =
    "<div itemscope itemtype=\"https://schema.org/Product\">"
    "  <h1 itemprop=\"name\">Awesome Product</h1>"
    "  <div itemprop=\"offers\" itemscope itemtype=\"https://schema.org/Offer\">"
    "    <span itemprop=\"price\">$29.99</span>"
    "    <span itemprop=\"priceCurrency\">USD</span>"
    "  </div>"
    "</div>";

char* microdata = meta_oxide_extract_microdata(html, "https://shop.example");
if (microdata) {
    printf("Product data: %s\n", microdata);
    meta_oxide_string_free(microdata);
}
```

### Example 3: Blog Post with Microformats

```c
const char* html =
    "<article class=\"h-entry\">"
    "  <h1 class=\"p-name\">My Blog Post</h1>"
    "  <time class=\"dt-published\" datetime=\"2025-01-15\">Jan 15, 2025</time>"
    "  <div class=\"p-author h-card\">"
    "    <img class=\"u-photo\" src=\"/photo.jpg\">"
    "    <a class=\"p-name u-url\" href=\"https://author.example\">Author Name</a>"
    "  </div>"
    "  <div class=\"e-content\">Post content...</div>"
    "</article>";

char* microformats = meta_oxide_extract_microformats(html, "https://blog.example");
if (microformats) {
    // Will contain h-entry with nested h-card for author
    printf("Microformats: %s\n", microformats);
    meta_oxide_string_free(microformats);
}
```

### Example 4: URL Resolution

```c
const char* html =
    "<head>"
    "  <link rel=\"canonical\" href=\"/articles/example\">"
    "  <meta property=\"og:image\" content=\"../images/featured.jpg\">"
    "</head>";

// Without base URL - URLs remain relative
char* links1 = meta_oxide_extract_rel_links(html, NULL);
// Result: {"/articles/example"}

// With base URL - URLs are resolved
char* links2 = meta_oxide_extract_rel_links(html, "https://example.com/blog/");
// Result: {"https://example.com/articles/example"}

meta_oxide_string_free(links1);
meta_oxide_string_free(links2);
```

### Example 5: Error Recovery

```c
const char* malformed_html =
    "<html><head>"
    "<script type=\"application/ld+json\">{BROKEN JSON}</script>"
    "</head></html>";

MetaOxideResult* result = meta_oxide_extract_all(malformed_html, NULL);

if (result) {
    // MetaOxide continues extracting other formats even if one fails
    // json_ld will be NULL, but meta, og, twitter, etc. will work

    if (result->json_ld == NULL) {
        printf("JSON-LD failed (expected with broken JSON)\n");
    }

    if (result->meta != NULL) {
        printf("But meta tags were still extracted!\n");
    }

    meta_oxide_result_free(result);
}
```

## Performance Tips

### 1. Use Individual Extractors When Possible

If you only need specific metadata types, use individual extractors instead of `extract_all`:

```c
// Fast - only extracts Open Graph
char* og = meta_oxide_extract_open_graph(html, base_url);

// Slower - extracts everything
MetaOxideResult* result = meta_oxide_extract_all(html, base_url);
```

### 2. Reuse Parsed HTML (Future API)

Currently each call parses the HTML. If you need multiple extractions, use `extract_all` once:

```c
// Good - parse once
MetaOxideResult* result = meta_oxide_extract_all(html, base_url);

// Bad - parses HTML 3 times!
char* meta = meta_oxide_extract_meta(html, base_url);
char* og = meta_oxide_extract_open_graph(html, base_url);
char* twitter = meta_oxide_extract_twitter(html, base_url);
```

### 3. Batch Processing

Process multiple documents in parallel using threads:

```c
#pragma omp parallel for
for (int i = 0; i < num_documents; i++) {
    MetaOxideResult* result = meta_oxide_extract_all(documents[i], NULL);
    // Process result...
    meta_oxide_result_free(result);
}
```

### 4. Memory Pool Pattern

For high-throughput scenarios, consider using a memory pool for JSON parsing:

```c
// Extract once
MetaOxideResult* result = meta_oxide_extract_all(html, base_url);

// Parse all JSON strings at once
parse_all_json(result);

// Free all at once
meta_oxide_result_free(result);
```

## Troubleshooting

### Linking Errors

```
undefined reference to `meta_oxide_extract_all`
```

**Solution:** Add `-lmeta_oxide` to linker flags and ensure library path is correct.

### Runtime Library Not Found

```
error while loading shared libraries: libmeta_oxide.so
```

**Solution:** Set `LD_LIBRARY_PATH`:
```bash
export LD_LIBRARY_PATH=/path/to/target/release:$LD_LIBRARY_PATH
./your_program
```

Or install the library system-wide:
```bash
sudo cp target/release/libmeta_oxide.so /usr/local/lib/
sudo ldconfig
```

### Memory Leaks

Use Valgrind to detect leaks:
```bash
valgrind --leak-check=full --show-leak-kinds=all ./your_program
```

Ensure all `_free()` functions are called.

### Crashes with NULL Pointers

```c
// Bad - crashes if result is NULL
MetaOxideResult* result = meta_oxide_extract_all(html, NULL);
printf("%s\n", result->meta);  // CRASH!

// Good - check for NULL
MetaOxideResult* result = meta_oxide_extract_all(html, NULL);
if (result && result->meta) {
    printf("%s\n", result->meta);
}
meta_oxide_result_free(result);
```

## Next Steps

- See [C Binding Guide](c-binding-guide.md) for creating language bindings
- Check [examples/c_api_example.c](../examples/c_api_example.c) for more examples
- Run [tests/c_api_test.c](../tests/c_api_test.c) to verify your installation

## Support

- GitHub Issues: https://github.com/yfedoseev/meta_oxide/issues
- Documentation: https://github.com/yfedoseev/meta_oxide/tree/main/docs

## License

MIT OR Apache-2.0
