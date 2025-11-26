# MetaOxide Python API Reference

Complete API documentation for the Python package.

## Table of Contents

- [Installation](#installation)
- [Core Classes](#core-classes)
- [Extraction Methods](#extraction-methods)
- [Data Types](#data-types)
- [Exceptions](#exceptions)
- [Type Hints](#type-hints)

## Installation

```bash
pip install meta-oxide
```

## Core Classes

### `MetaOxide`

The main class for extracting metadata from HTML.

```python
class MetaOxide:
    """MetaOxide metadata extractor."""

    def __init__(self, html: str, base_url: str) -> None:
        """
        Initialize MetaOxide extractor.

        Args:
            html: HTML content to parse
            base_url: Base URL for resolving relative URLs

        Raises:
            MetaOxideError: If HTML parsing fails
            ValueError: If base_url is invalid

        Example:
            >>> from meta_oxide import MetaOxide
            >>> html = '<!DOCTYPE html>...'
            >>> extractor = MetaOxide(html, 'https://example.com')
        """
```

## Extraction Methods

### `extract_all()`

Extract all metadata formats at once.

```python
def extract_all(self) -> Dict[str, Any]:
    """
    Extract all metadata formats.

    Returns:
        Dictionary containing all extracted metadata

    Raises:
        MetaOxideError: If extraction fails

    Example:
        >>> metadata = extractor.extract_all()
        >>> print(metadata['title'])
        >>> print(metadata.get('description'))
    """
```

**Return Structure:**
```python
{
    'title': str,
    'description': str,
    'keywords': List[str],
    'author': str,
    'canonical': str,
    'og:title': str,
    'og:image': str,
    'twitter:card': str,
    # ... and more
}
```

### `extract_basic_meta()`

Extract basic HTML metadata.

```python
def extract_basic_meta(self) -> Dict[str, Any]:
    """
    Extract basic HTML metadata.

    Returns:
        Dictionary with title, description, keywords, etc.

    Example:
        >>> basic = extractor.extract_basic_meta()
        >>> print(basic['title'])
        >>> print(basic['description'])
    """
```

**Return Structure:**
```python
{
    'title': str,
    'description': str,
    'keywords': List[str],
    'author': str,
    'canonical': str,
    'charset': str,
    'viewport': str,
    'language': str,
    'robots': str
}
```

### `extract_opengraph()`

Extract Open Graph metadata.

```python
def extract_opengraph(self) -> Optional[Dict[str, Any]]:
    """
    Extract Open Graph metadata.

    Returns:
        Dictionary with Open Graph data or None

    Example:
        >>> og = extractor.extract_opengraph()
        >>> if og:
        ...     print(og['title'])
        ...     print(og['image'])
    """
```

**Return Structure:**
```python
{
    'title': str,
    'type': str,
    'image': str,
    'url': str,
    'description': str,
    'site_name': str,
    'locale': str,
    'audio': str,
    'video': str
}
```

### `extract_twitter_card()`

Extract Twitter Card metadata.

```python
def extract_twitter_card(self) -> Optional[Dict[str, Any]]:
    """
    Extract Twitter Card metadata.

    Returns:
        Dictionary with Twitter Card data or None

    Example:
        >>> twitter = extractor.extract_twitter_card()
        >>> if twitter:
        ...     print(twitter['card'])
        ...     print(twitter['title'])
    """
```

**Return Structure:**
```python
{
    'card': str,
    'site': str,
    'creator': str,
    'title': str,
    'description': str,
    'image': str,
    'image:alt': str
}
```

### `extract_jsonld()`

Extract JSON-LD structured data.

```python
def extract_jsonld(self) -> List[Dict[str, Any]]:
    """
    Extract JSON-LD structured data.

    Returns:
        List of JSON-LD objects

    Example:
        >>> jsonld = extractor.extract_jsonld()
        >>> for item in jsonld:
        ...     print(item.get('@type'))
        ...     print(item.get('name'))
    """
```

### `extract_microdata()`

Extract Microdata (schema.org).

```python
def extract_microdata(self) -> List[Dict[str, Any]]:
    """
    Extract Microdata items.

    Returns:
        List of Microdata items

    Example:
        >>> microdata = extractor.extract_microdata()
        >>> for item in microdata:
        ...     print(item['type'])
        ...     print(item['properties'])
    """
```

**Item Structure:**
```python
{
    'type': List[str],
    'properties': Dict[str, List[Any]],
    'id': Optional[str]
}
```

### `extract_microformats()`

Extract Microformats data.

```python
def extract_microformats(self) -> Dict[str, List[Dict[str, Any]]]:
    """
    Extract all Microformats.

    Returns:
        Dictionary mapping format types to items

    Example:
        >>> mf = extractor.extract_microformats()
        >>> if 'h-card' in mf:
        ...     for card in mf['h-card']:
        ...         print(card['name'])
    """
```

**Return Structure:**
```python
{
    'h-card': List[Dict],
    'h-entry': List[Dict],
    'h-event': List[Dict],
    'h-review': List[Dict],
    'h-recipe': List[Dict],
    'h-product': List[Dict]
}
```

### `extract_dublin_core()`

Extract Dublin Core metadata.

```python
def extract_dublin_core(self) -> Optional[Dict[str, str]]:
    """
    Extract Dublin Core metadata.

    Returns:
        Dictionary with Dublin Core data or None

    Example:
        >>> dc = extractor.extract_dublin_core()
        >>> if dc:
        ...     print(dc['title'])
        ...     print(dc['creator'])
    """
```

### `extract_rdfa()`

Extract RDFa triples.

```python
def extract_rdfa(self) -> List[Dict[str, str]]:
    """
    Extract RDFa triples.

    Returns:
        List of RDFa triple dictionaries

    Example:
        >>> rdfa = extractor.extract_rdfa()
        >>> for triple in rdfa:
        ...     print(f"{triple['subject']} {triple['predicate']} {triple['object']}")
    """
```

### `extract_rel_links()`

Extract link relations.

```python
def extract_rel_links(self) -> Dict[str, List[Dict[str, str]]]:
    """
    Extract link relations.

    Returns:
        Dictionary mapping rel types to links

    Example:
        >>> links = extractor.extract_rel_links()
        >>> if 'canonical' in links:
        ...     print(links['canonical'][0]['href'])
    """
```

### `extract_manifest()`

Extract web app manifest.

```python
def extract_manifest(self) -> Optional[Dict[str, Any]]:
    """
    Extract web app manifest data.

    Returns:
        Manifest dictionary or None

    Example:
        >>> manifest = extractor.extract_manifest()
        >>> if manifest:
        ...     print(manifest['name'])
        ...     print(manifest['icons'])
    """
```

## Data Types

### Microformats Types

#### h-card (Person/Organization)

```python
{
    'name': str,
    'url': str,
    'photo': str,
    'email': str,
    'tel': str,
    'org': str,
    'adr': Dict[str, str]
}
```

#### h-entry (Blog Post/Article)

```python
{
    'name': str,
    'author': Dict,  # h-card
    'published': str,
    'updated': str,
    'content': str,
    'summary': str,
    'url': str,
    'category': List[str],
    'syndication': List[str]
}
```

#### h-event (Event)

```python
{
    'name': str,
    'start': str,
    'end': str,
    'duration': str,
    'summary': str,
    'description': str,
    'url': str,
    'location': str,
    'category': List[str]
}
```

## Exceptions

### `MetaOxideError`

Base exception class for MetaOxide errors.

```python
class MetaOxideError(Exception):
    """Base exception for MetaOxide errors."""
    pass
```

### `ParseError`

Raised when HTML parsing fails.

```python
class ParseError(MetaOxideError):
    """HTML parsing error."""
    pass
```

### `ExtractionError`

Raised when metadata extraction fails.

```python
class ExtractionError(MetaOxideError):
    """Metadata extraction error."""
    pass
```

### `UrlError`

Raised when URL parsing or resolution fails.

```python
class UrlError(MetaOxideError):
    """URL parsing error."""
    pass
```

### Exception Handling Example

```python
from meta_oxide import MetaOxide, MetaOxideError, ParseError

try:
    extractor = MetaOxide(html, url)
    metadata = extractor.extract_all()
except ParseError as e:
    print(f"HTML parsing failed: {e}")
except MetaOxideError as e:
    print(f"Extraction error: {e}")
except Exception as e:
    print(f"Unexpected error: {e}")
```

## Type Hints

Full type hint support for better IDE experience:

```python
from typing import Dict, List, Optional, Any
from meta_oxide import MetaOxide

def extract_metadata(html: str, url: str) -> Dict[str, Any]:
    """Extract metadata with type hints."""
    extractor: MetaOxide = MetaOxide(html, url)
    metadata: Dict[str, Any] = extractor.extract_all()
    return metadata

def extract_opengraph(html: str, url: str) -> Optional[Dict[str, Any]]:
    """Extract Open Graph with type hints."""
    extractor: MetaOxide = MetaOxide(html, url)
    og_data: Optional[Dict[str, Any]] = extractor.extract_opengraph()
    return og_data
```

## Async Support

MetaOxide works seamlessly with async Python:

```python
import asyncio
import aiohttp
from meta_oxide import MetaOxide

async def extract_from_url(url: str) -> Dict[str, Any]:
    """Async extraction from URL."""
    async with aiohttp.ClientSession() as session:
        async with session.get(url) as response:
            html = await response.text()
            extractor = MetaOxide(html, url)
            return extractor.extract_all()

async def main():
    metadata = await extract_from_url('https://example.com')
    print(metadata)

asyncio.run(main())
```

## Performance Tips

### Batch Processing

```python
from concurrent.futures import ThreadPoolExecutor
from meta_oxide import MetaOxide

def extract_metadata(html: str, url: str) -> Dict[str, Any]:
    extractor = MetaOxide(html, url)
    return extractor.extract_all()

# Process multiple documents in parallel
urls = ['url1', 'url2', 'url3']
htmls = [fetch_html(url) for url in urls]

with ThreadPoolExecutor(max_workers=4) as executor:
    results = list(executor.map(extract_metadata, htmls, urls))
```

### Selective Extraction

Extract only what you need for better performance:

```python
# Instead of extract_all()
metadata = extractor.extract_all()

# Extract only specific formats
og = extractor.extract_opengraph()
twitter = extractor.extract_twitter_card()
```

### Caching

```python
from functools import lru_cache

@lru_cache(maxsize=128)
def extract_metadata_cached(url: str) -> Dict[str, Any]:
    html = fetch_html(url)
    extractor = MetaOxide(html, url)
    return extractor.extract_all()
```

## Context Manager Support

```python
# Future API (if implemented)
with MetaOxide(html, url) as extractor:
    metadata = extractor.extract_all()
# Automatic cleanup
```

## See Also

- [Getting Started Guide](/docs/getting-started/getting-started-python.md)
- [Flask Integration](/docs/integrations/flask-integration.md)
- [Django Integration](/docs/integrations/django-integration.md)
- [Examples](/examples/real-world/python-flask-api/)
