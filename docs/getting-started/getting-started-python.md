# Getting Started with MetaOxide (Python)

Welcome to MetaOxide! This guide will help you get started with the Python bindings for MetaOxide in just 5 minutes.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Basic Extraction](#basic-extraction)
- [Async Usage](#async-usage)
- [Next Steps](#next-steps)

## Installation

Install MetaOxide via pip:

```bash
pip install meta-oxide
```

Or with poetry:

```bash
poetry add meta-oxide
```

### Requirements

- Python 3.7+
- Works on Linux, macOS, and Windows

## Quick Start

Here's a minimal example to extract metadata from HTML:

```python
from meta_oxide import MetaOxide

html = """
<!DOCTYPE html>
<html>
<head>
    <title>My Page</title>
    <meta name="description" content="A great page">
    <meta property="og:title" content="My Page">
</head>
<body>Hello World</body>
</html>
"""

# Create extractor
extractor = MetaOxide(html, "https://example.com")

# Extract all metadata
metadata = extractor.extract_all()

print(f"Title: {metadata.get('title')}")
print(f"Description: {metadata.get('description')}")
```

## Basic Extraction

MetaOxide supports 13 metadata formats. Here's how to extract specific formats:

### Extract Open Graph Data

```python
from meta_oxide import MetaOxide

def extract_opengraph(html: str) -> dict:
    extractor = MetaOxide(html, "https://example.com")
    og_data = extractor.extract_opengraph()

    if og_data:
        print(f"OG Title: {og_data.get('title')}")
        print(f"OG Type: {og_data.get('type')}")
        print(f"OG Image: {og_data.get('image')}")

    return og_data or {}
```

### Extract Twitter Cards

```python
from meta_oxide import MetaOxide

def extract_twitter(html: str) -> dict:
    extractor = MetaOxide(html, "https://example.com")
    twitter_data = extractor.extract_twitter_card()

    if twitter_data:
        print(f"Card Type: {twitter_data.get('card')}")
        print(f"Title: {twitter_data.get('title')}")

    return twitter_data or {}
```

### Extract JSON-LD Structured Data

```python
from meta_oxide import MetaOxide
import json

def extract_jsonld(html: str) -> list:
    extractor = MetaOxide(html, "https://example.com")
    jsonld_data = extractor.extract_jsonld()

    if jsonld_data:
        print(json.dumps(jsonld_data, indent=2))

    return jsonld_data or []
```

### Extract from URL

```python
import requests
from meta_oxide import MetaOxide

def extract_from_url(url: str) -> dict:
    response = requests.get(url)
    response.raise_for_status()

    extractor = MetaOxide(response.text, url)
    return extractor.extract_all()

# Usage
metadata = extract_from_url("https://example.com")
```

## Async Usage

MetaOxide works seamlessly with async Python:

```python
import aiohttp
from meta_oxide import MetaOxide

async def extract_async(url: str) -> dict:
    async with aiohttp.ClientSession() as session:
        async with session.get(url) as response:
            html = await response.text()
            extractor = MetaOxide(html, url)
            return extractor.extract_all()

# Usage with asyncio
import asyncio

async def main():
    urls = [
        "https://example.com",
        "https://example.org",
        "https://example.net"
    ]

    tasks = [extract_async(url) for url in urls]
    results = await asyncio.gather(*tasks)

    for url, metadata in zip(urls, results):
        print(f"{url}: {metadata.get('title')}")

asyncio.run(main())
```

## Error Handling

Handle errors gracefully:

```python
from meta_oxide import MetaOxide, MetaOxideError

def safe_extraction(html: str, url: str) -> dict:
    try:
        extractor = MetaOxide(html, url)
        metadata = extractor.extract_all()
        print(f"Extracted {len(metadata)} fields")
        return metadata
    except MetaOxideError as e:
        print(f"Extraction failed: {e}")
        return {}
    except Exception as e:
        print(f"Unexpected error: {e}")
        return {}
```

Common errors:
- `ParseError`: Invalid HTML structure
- `UrlParseError`: Invalid base URL
- `ExtractionError`: Failed to extract specific metadata format

## Next Steps

Now that you've got the basics, explore more:

1. **[Complete API Reference](/docs/api/api-reference-python.md)** - Learn about all available methods
2. **[Real-World Examples](/examples/real-world/python-flask-api/)** - See a complete Flask API implementation
3. **[Integration Guides](/docs/integrations/flask-integration.md)** - Use with Flask, Django, FastAPI

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

### Type Hints

MetaOxide includes full type hints for better IDE support:

```python
from typing import Dict, List, Optional
from meta_oxide import MetaOxide

def extract_metadata(html: str, url: str) -> Dict[str, any]:
    extractor: MetaOxide = MetaOxide(html, url)
    metadata: Dict[str, any] = extractor.extract_all()
    return metadata
```

### Learn More

- [Flask Integration](/docs/integrations/flask-integration.md)
- [Django Integration](/docs/integrations/django-integration.md)
- [FastAPI Integration](/docs/integrations/fastapi-integration.md)
- [Performance Tuning](/docs/performance/performance-tuning-guide.md)

Happy extracting! 🚀
