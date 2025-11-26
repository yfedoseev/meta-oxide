# MetaOxide Rust CLI Tool

A command-line tool demonstrating how to build a metadata extraction application using MetaOxide in Rust.

## What This Example Shows

This CLI tool demonstrates:
- Using MetaOxide as a library in Rust applications
- Parsing command-line arguments
- Reading HTML from files and URLs
- Extracting all 13 metadata formats
- Formatting and displaying results
- Error handling
- Performance considerations

## Prerequisites

- Rust 1.70+ ([Install](https://rustup.rs/))
- Cargo (comes with Rust)

## Installation & Setup

```bash
# Build the tool
cargo build --release

# The binary will be at target/release/metaoxide-cli
```

## Usage

### Basic Usage

```bash
# Extract metadata from a file
./target/release/metaoxide-cli path/to/file.html

# Extract metadata from a URL (requires file to be HTML)
./target/release/metaoxide-cli --file path/to/page.html

# Show only specific format
./target/release/metaoxide-cli --format open-graph path/to/file.html

# Output as JSON
./target/release/metaoxide-cli --json path/to/file.html

# Extract from multiple files
./target/release/metaoxide-cli *.html
```

### Available Formats

Use `--format` or `-f` to extract specific metadata:
- `meta` - HTML Meta Tags
- `open-graph` - Open Graph properties
- `twitter` - Twitter Card metadata
- `json-ld` - JSON-LD structured data
- `microdata` - HTML Microdata
- `microformats` - Microformats (all types)
- `rdfa` - RDFa markup
- `dublin-core` - Dublin Core metadata
- `manifest` - Web App Manifest
- `oembed` - oEmbed data
- `rel-links` - Link relations

### Examples

#### Extract all metadata from a file
```bash
./target/release/metaoxide-cli my-page.html
```

Output:
```
═══════════════════════════════════════════
  MetaOxide - Metadata Extraction Tool
═══════════════════════════════════════════

File: my-page.html
Base URL: file:///path/to/my-page.html

───────────────────────────────────────────
  Meta Tags (HTML)
───────────────────────────────────────────
title: My Page Title
description: This is a description

───────────────────────────────────────────
  Open Graph
───────────────────────────────────────────
og:title: My Page Title
og:description: Page description
og:image: https://example.com/image.jpg

[... more formats ...]
```

#### Extract only Open Graph metadata
```bash
./target/release/metaoxide-cli -f open-graph my-page.html
```

#### Output as JSON
```bash
./target/release/metaoxide-cli --json my-page.html > metadata.json
```

#### Process multiple files
```bash
./target/release/metaoxide-cli *.html

# Or with specific format
./target/release/metaoxide-cli -f json-ld *.html
```

## How It Works

### Main Components

1. **Argument Parsing** (`main.rs`)
   - Uses `clap` crate for CLI argument handling
   - Supports file paths, URL handling, format filtering
   - Provides helpful error messages

2. **File Reading**
   - Reads HTML content from local files
   - Sets appropriate base URL for relative resolution
   - Handles file encoding (UTF-8)

3. **Extraction**
   - Creates `MetaOxide` instance with HTML and base URL
   - Calls extraction methods for requested formats
   - Handles errors gracefully

4. **Output Formatting**
   - Pretty-prints results in table format
   - Optional JSON output for programmatic use
   - Summary statistics

### Key Code Sections

```rust
// Load HTML file
let html = std::fs::read_to_string(&file_path)?;

// Create extractor
let extractor = MetaOxide::new(&html, &base_url);

// Extract all metadata
let meta = extractor.extract_all()?;

// Display results
for (key, value) in meta.meta {
    println!("  {}: {}", key, value);
}
```

## Performance

Typical performance on modern hardware:

- **Single page extraction**: <10ms
- **Small batch (10 files)**: <100ms
- **Large batch (1000 files)**: <10 seconds

For large-scale processing, consider parallel processing:
```bash
# Using GNU parallel (install with: apt-get install parallel)
parallel "./metaoxide-cli {} > {.}.json" ::: *.html
```

## Output Examples

### Standard Output
```
═══════════════════════════════════════════
  Meta Tags
═══════════════════════════════════════════
title: GitHub
description: GitHub is where over 100 million developers shape the future
viewport: width=device-width, initial-scale=1
...
```

### JSON Output
```json
{
  "meta": {
    "title": "GitHub",
    "description": "GitHub is where over 100 million developers..."
  },
  "openGraph": {
    "og:title": "GitHub",
    "og:type": "website",
    "og:url": "https://github.com"
  },
  ...
}
```

## Learning Resources

- [MetaOxide Getting Started](../../../docs/getting-started/getting-started-rust.md)
- [Rust API Reference](../../../docs/api/api-reference-rust.md)
- [Metadata Format Documentation](../../../docs/FORMATS.md)
- [Rust Book](https://doc.rust-lang.org/book/)

## Extending This Example

### Add URL Fetching
To extract from remote URLs, you can add:

```bash
# Add to Cargo.toml
reqwest = { version = "0.11", features = ["json"] }
tokio = { version = "1", features = ["full"] }
```

Then modify the main function to handle URLs:
```rust
let html = if path.starts_with("http") {
    reqwest::blocking::get(&path)?.text()?
} else {
    std::fs::read_to_string(&path)?
};
```

### Add CSV Export
Export results in CSV format for spreadsheet analysis:

```rust
// Would add CSV export capability
// Uses csv crate
```

### Add Interactive Mode
Build an interactive CLI where users can specify formats and options interactively.

## Troubleshooting

### "File not found"
```bash
# Make sure the file path is correct and readable
ls -la my-page.html
```

### "Invalid HTML"
```bash
# Some malformed HTML is still extracted, but check:
# - File encoding is UTF-8
# - HTML is properly formed
```

### Memory Issues with Large Files
```bash
# For very large HTML files (>10MB), consider processing in streaming mode
# or splitting the file
```

## License

This example is licensed under the same dual license as MetaOxide: **MIT OR Apache-2.0**

---

**Questions?** Check the main [MetaOxide documentation](../../../README.md) or open an issue on GitHub.
