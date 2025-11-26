# Examples

This document provides practical examples of using MetaOxide in various scenarios.

## Table of Contents

- [Basic Examples](#basic-examples)
- [Python Examples](#python-examples)
- [Rust Examples](#rust-examples)
- [Real-World Use Cases](#real-world-use-cases)
- [Advanced Patterns](#advanced-patterns)

---

## Basic Examples

### Extract Contact Information (h-card)

**HTML:**
```html
<div class="h-card">
    <img class="u-photo" src="https://example.com/photo.jpg" alt="">
    <a class="p-name u-url" href="https://example.com">Jane Doe</a>
    <a class="u-email" href="mailto:jane@example.com">jane@example.com</a>
    <span class="p-tel">+1-555-0123</span>
    <p class="p-note">Software engineer and open source enthusiast.</p>
</div>
```

**Python:**
```python
import meta_oxide

html = """..."""  # HTML from above

cards = meta_oxide.extract_hcard(html)
for card in cards:
    print(f"Name: {card['name']}")
    print(f"Email: {card['email']}")
    print(f"Phone: {card['tel']}")
    print(f"Photo: {card['photo']}")
    print(f"Bio: {card['note']}")
```

**Output:**
```
Name: Jane Doe
Email: jane@example.com
Phone: +1-555-0123
Photo: https://example.com/photo.jpg
Bio: Software engineer and open source enthusiast.
```

---

### Extract Blog Posts (h-entry)

**HTML:**
```html
<article class="h-entry">
    <h1 class="p-name">Getting Started with Rust</h1>
    <time class="dt-published" datetime="2024-01-15">January 15, 2024</time>
    <div class="p-author h-card">
        <span class="p-name">John Smith</span>
    </div>
    <div class="e-content">
        <p>Rust is a systems programming language...</p>
    </div>
    <a class="p-category" href="/tag/rust">Rust</a>
    <a class="p-category" href="/tag/programming">Programming</a>
</article>
```

**Python:**
```python
import meta_oxide

entries = meta_oxide.extract_hentry(html)
for entry in entries:
    print(f"Title: {entry['name']}")
    print(f"Published: {entry['published']}")
    print(f"Author: {entry['author']['name']}")
    print(f"Categories: {', '.join(entry['category'])}")
    print(f"Content: {entry['content'][:100]}...")
```

---

### Extract Events (h-event)

**HTML:**
```html
<div class="h-event">
    <h1 class="p-name">RustConf 2024</h1>
    <time class="dt-start" datetime="2024-09-10T09:00:00-07:00">
        September 10, 2024 at 9:00 AM PDT
    </time>
    <time class="dt-end" datetime="2024-09-12T17:00:00-07:00">
        September 12, 2024 at 5:00 PM PDT
    </time>
    <p class="p-location">Montreal, Canada</p>
    <p class="p-summary">The official Rust programming language conference</p>
</div>
```

**Python:**
```python
import meta_oxide

events = meta_oxide.extract_hevent(html)
for event in events:
    print(f"Event: {event['name']}")
    print(f"Start: {event['start']}")
    print(f"End: {event['end']}")
    print(f"Location: {event['location']}")
    print(f"Summary: {event['summary']}")
```

---

## Python Examples

### Extract All Microformats at Once

```python
import meta_oxide

html = """
<html>
<body>
    <div class="h-card">
        <span class="p-name">Alice</span>
    </div>
    <article class="h-entry">
        <h1 class="p-name">My Post</h1>
    </article>
    <div class="h-event">
        <h1 class="p-name">Meetup</h1>
    </div>
</body>
</html>
"""

# Extract all microformats in one call
all_mf = meta_oxide.extract_microformats(html)

print(f"Found {len(all_mf['h-card'])} h-cards")
print(f"Found {len(all_mf['h-entry'])} h-entries")
print(f"Found {len(all_mf['h-event'])} h-events")
```

---

### Parse Web Page with URL Resolution

```python
import meta_oxide
import requests

# Fetch a web page
url = "https://example.com/blog"
response = requests.get(url)
html = response.text

# Extract h-entries with URL resolution
entries = meta_oxide.extract_hentry(html, base_url=url)

for entry in entries:
    # Relative URLs are now absolute
    print(f"Permalink: {entry.get('url')}")
```

---

### Handle Missing Properties

```python
import meta_oxide

html = """
<div class="h-card">
    <span class="p-name">Bob</span>
    <!-- No email or URL -->
</div>
"""

cards = meta_oxide.extract_hcard(html)
card = cards[0]

# Safely access optional properties
name = card.get('name', 'Unknown')
email = card.get('email', 'No email provided')
url = card.get('url')

print(f"Name: {name}")
print(f"Email: {email}")
if url:
    print(f"URL: {url}")
```

---

### Batch Processing

```python
import meta_oxide
import glob

def process_html_file(filepath):
    with open(filepath, 'r') as f:
        html = f.read()

    return meta_oxide.extract_hcard(html)

# Process multiple files
all_cards = []
for filepath in glob.glob('data/*.html'):
    cards = process_html_file(filepath)
    all_cards.extend(cards)

print(f"Total cards found: {len(all_cards)}")
```

---

### Convert to JSON

```python
import meta_oxide
import json

html = """..."""
cards = meta_oxide.extract_hcard(html)

# Convert to JSON
json_output = json.dumps(cards, indent=2)
print(json_output)

# Save to file
with open('cards.json', 'w') as f:
    json.dump(cards, f, indent=2)
```

---

## Rust Examples

### Basic Usage

```rust
use meta_oxide::extractors::extract_hcard;

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let html = r#"
        <div class="h-card">
            <span class="p-name">Alice</span>
            <a class="u-url" href="https://alice.example">Website</a>
        </div>
    "#;

    let cards = extract_hcard(html, None)?;

    for card in cards {
        if let Some(name) = card.name {
            println!("Name: {}", name);
        }
        if let Some(url) = card.url {
            println!("URL: {}", url);
        }
    }

    Ok(())
}
```

---

### Error Handling

```rust
use meta_oxide::extractors::extract_hcard;
use meta_oxide::MicroformatError;

fn process_html(html: &str) -> Result<usize, MicroformatError> {
    let cards = extract_hcard(html, None)?;
    Ok(cards.len())
}

fn main() {
    let html = "<div class='h-card'><span class='p-name'>Bob</span></div>";

    match process_html(html) {
        Ok(count) => println!("Found {} h-cards", count),
        Err(MicroformatError::ParseError(msg)) => {
            eprintln!("Failed to parse HTML: {}", msg);
        }
        Err(e) => eprintln!("Error: {}", e),
    }
}
```

---

### With URL Resolution

```rust
use meta_oxide::extractors::extract_hentry;

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let html = r#"
        <article class="h-entry">
            <h1 class="p-name">My Post</h1>
            <a class="u-url" href="/posts/123">Permalink</a>
        </article>
    "#;

    let base_url = "https://example.com";
    let entries = extract_hentry(html, Some(base_url))?;

    for entry in entries {
        if let Some(url) = entry.url {
            // URL is resolved: https://example.com/posts/123
            println!("Permalink: {}", url);
        }
    }

    Ok(())
}
```

---

### Parallel Processing

```rust
use meta_oxide::extractors::extract_hcard;
use rayon::prelude::*;
use std::fs;

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let files = vec![
        "page1.html",
        "page2.html",
        "page3.html",
    ];

    let results: Vec<_> = files
        .par_iter()
        .map(|file| {
            let html = fs::read_to_string(file).unwrap();
            extract_hcard(&html, None).unwrap()
        })
        .flatten()
        .collect();

    println!("Total h-cards: {}", results.len());

    Ok(())
}
```

---

### Serialize to JSON

```rust
use meta_oxide::extractors::extract_hcard;
use serde_json;

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let html = r#"
        <div class="h-card">
            <span class="p-name">Charlie</span>
        </div>
    "#;

    let cards = extract_hcard(html, None)?;

    // Serialize to JSON
    let json = serde_json::to_string_pretty(&cards)?;
    println!("{}", json);

    Ok(())
}
```

---

## Real-World Use Cases

### Social Media Profile Scraper

```python
import meta_oxide
import requests

def scrape_profile(url):
    """Extract contact info from a personal website."""
    response = requests.get(url)
    cards = meta_oxide.extract_hcard(response.text, base_url=url)

    if not cards:
        return None

    profile = cards[0]
    return {
        'name': profile.get('name'),
        'website': profile.get('url'),
        'email': profile.get('email'),
        'photo': profile.get('photo'),
        'bio': profile.get('note'),
    }

# Usage
profile = scrape_profile('https://someone.example.com')
if profile:
    print(f"Found profile for {profile['name']}")
```

---

### Blog Aggregator

```python
import meta_oxide
import requests
from datetime import datetime

def fetch_blog_posts(blog_url):
    """Fetch all h-entry posts from a blog."""
    response = requests.get(blog_url)
    entries = meta_oxide.extract_hentry(response.text, base_url=blog_url)

    posts = []
    for entry in entries:
        posts.append({
            'title': entry.get('name'),
            'url': entry.get('url'),
            'published': entry.get('published'),
            'author': entry.get('author', {}).get('name'),
            'categories': entry.get('category', []),
            'summary': entry.get('summary'),
        })

    return posts

# Aggregate from multiple blogs
blogs = [
    'https://blog1.example.com',
    'https://blog2.example.com',
]

all_posts = []
for blog in blogs:
    posts = fetch_blog_posts(blog)
    all_posts.extend(posts)

# Sort by date
all_posts.sort(key=lambda p: p['published'], reverse=True)

for post in all_posts[:10]:
    print(f"{post['title']} - {post['author']}")
```

---

### Event Calendar Importer

```python
import meta_oxide
import requests
from icalendar import Calendar, Event as ICalEvent
from datetime import datetime

def import_events_to_ical(url):
    """Import h-events and convert to iCalendar format."""
    response = requests.get(url)
    events = meta_oxide.extract_hevent(response.text, base_url=url)

    cal = Calendar()
    cal.add('prodid', '-//MetaOxide Event Importer//EN')
    cal.add('version', '2.0')

    for event in events:
        ical_event = ICalEvent()
        ical_event.add('summary', event.get('name'))
        ical_event.add('dtstart', datetime.fromisoformat(event.get('start')))

        if event.get('end'):
            ical_event.add('dtend', datetime.fromisoformat(event.get('end')))

        if event.get('location'):
            ical_event.add('location', event.get('location'))

        if event.get('description'):
            ical_event.add('description', event.get('description'))

        cal.add_component(ical_event)

    return cal.to_ical()

# Usage
ical_data = import_events_to_ical('https://events.example.com')
with open('events.ics', 'wb') as f:
    f.write(ical_data)
```

---

### IndieWeb Comment System

```python
import meta_oxide
import requests

def fetch_webmentions(article_url):
    """Fetch and parse webmentions as h-entries."""
    # Discover webmention endpoint
    response = requests.get(article_url)
    # ... webmention discovery logic ...

    # Parse incoming webmentions
    entries = meta_oxide.extract_hentry(response.text, base_url=article_url)

    comments = []
    for entry in entries:
        # Check if it's a reply
        if 'in-reply-to' in entry.get('additional_properties', {}):
            comments.append({
                'author': entry.get('author'),
                'content': entry.get('content'),
                'published': entry.get('published'),
                'url': entry.get('url'),
            })

    return comments
```

---

## Advanced Patterns

### Custom Property Extraction

```python
import meta_oxide

html = """
<div class="h-card">
    <span class="p-name">Dana</span>
    <data class="p-skill" value="Python">Python Developer</data>
    <data class="p-skill" value="Rust">Rust Developer</data>
</div>
"""

cards = meta_oxide.extract_hcard(html)
card = cards[0]

# Access additional properties not in standard schema
skills = card.get('additional_properties', {}).get('skill', [])
print(f"Skills: {', '.join(skills)}")
```

---

### Nested Microformats

```python
import meta_oxide

html = """
<article class="h-entry">
    <h1 class="p-name">Article Title</h1>
    <div class="p-author h-card">
        <span class="p-name">Author Name</span>
        <a class="u-url" href="https://author.example.com">Website</a>
        <div class="p-org h-card">
            <span class="p-name">Company Name</span>
        </div>
    </div>
</article>
"""

entries = meta_oxide.extract_hentry(html)
entry = entries[0]

# Access nested h-card
author = entry['author']
print(f"Author: {author['name']}")
print(f"Author URL: {author['url']}")

# Access nested organization (if extracted)
if 'org' in author:
    print(f"Organization: {author['org']}")
```

---

### Integration with Flask

```python
from flask import Flask, request, jsonify
import meta_oxide

app = Flask(__name__)

@app.route('/extract/hcard', methods=['POST'])
def extract_hcard_api():
    data = request.get_json()
    html = data.get('html')
    base_url = data.get('base_url')

    try:
        cards = meta_oxide.extract_hcard(html, base_url)
        return jsonify({'success': True, 'data': cards})
    except ValueError as e:
        return jsonify({'success': False, 'error': str(e)}), 400

if __name__ == '__main__':
    app.run()
```

---

### Async Processing (Python)

```python
import asyncio
import aiohttp
import meta_oxide

async def fetch_and_extract(session, url):
    async with session.get(url) as response:
        html = await response.text()
        return meta_oxide.extract_hcard(html, base_url=url)

async def main():
    urls = [
        'https://person1.example.com',
        'https://person2.example.com',
        'https://person3.example.com',
    ]

    async with aiohttp.ClientSession() as session:
        tasks = [fetch_and_extract(session, url) for url in urls]
        results = await asyncio.gather(*tasks)

    all_cards = [card for cards in results for card in cards]
    print(f"Found {len(all_cards)} total cards")

asyncio.run(main())
```

---

## RDFa Examples

### Basic RDFa Extraction

```python
import meta_oxide

html = """
<div vocab="https://schema.org/" typeof="Person">
    <span property="name">Dr. Jane Smith</span>
    <span property="jobTitle">Software Engineer</span>
    <a property="url" href="https://janesmith.example">Website</a>
</div>
"""

rdfa_items = meta_oxide.extract_rdfa(html)
for item in rdfa_items:
    print(f"Type: {item['type']}")
    print(f"Vocab: {item['vocab']}")
    for prop, values in item['properties'].items():
        print(f"{prop}: {values}")
```

### RDFa with Nested Items

```python
import meta_oxide

html = """
<div vocab="https://schema.org/" typeof="Product">
    <span property="name">Deluxe Widget</span>
    <span property="brand">ACME Corp</span>
    <div property="offers" typeof="Offer">
        <span property="price" content="29.99">$29.99</span>
        <span property="priceCurrency" content="USD">USD</span>
    </div>
    <div property="aggregateRating" typeof="AggregateRating">
        <span property="ratingValue">4.5</span>
        <span property="reviewCount">128</span>
    </div>
</div>
"""

rdfa_items = meta_oxide.extract_rdfa(html, "https://example.com")

product = rdfa_items[0]
print(f"Product: {product['properties']['name'][0]}")
print(f"Price: {product['properties']['offers'][0]['properties']['price'][0]}")
print(f"Rating: {product['properties']['aggregateRating'][0]['properties']['ratingValue'][0]}")
```

### RDFa with CURIE Prefix Expansion

```python
import meta_oxide

# RDFa supports compact URIs (CURIEs) with prefix expansion
html = """
<div vocab="https://schema.org/" prefix="foaf: http://xmlns.com/foaf/0.1/">
    <div typeof="Person">
        <span property="name">Alice Wonder</span>
        <span property="foaf:nick">alice</span>
    </div>
</div>
"""

rdfa_items = meta_oxide.extract_rdfa(html)
# The 'foaf:nick' property is expanded to full URI
print(rdfa_items[0]['properties'])
```

### RDFa with Content Override and Datatypes

```python
import meta_oxide

html = """
<div vocab="https://schema.org/" typeof="Event">
    <span property="name">RustConf 2024</span>
    <time property="startDate" content="2024-09-10T09:00:00-07:00" datatype="xsd:dateTime">
        September 10, 2024
    </time>
    <span property="offers" typeof="Offer">
        <span property="price" content="299" datatype="xsd:decimal">$299</span>
    </span>
</div>
"""

rdfa_items = meta_oxide.extract_rdfa(html)
event = rdfa_items[0]
# The 'content' attribute overrides visible text for machine-readable values
print(f"Event: {event['properties']['name'][0]}")
print(f"Start: {event['properties']['startDate'][0]}")
```

### Real-World RDFa: Article Markup

```python
import meta_oxide

html = """
<article vocab="https://schema.org/" typeof="NewsArticle">
    <h1 property="headline">Breaking: New Discovery in Science</h1>
    <div property="author" typeof="Person">
        <span property="name">Dr. John Scientist</span>
    </div>
    <time property="datePublished" content="2024-11-25T10:00:00Z">
        November 25, 2024
    </time>
    <div property="articleBody">
        <p>Scientists have made a groundbreaking discovery...</p>
    </div>
</article>
"""

rdfa_items = meta_oxide.extract_rdfa(html, "https://news.example.com")
article = rdfa_items[0]

print(f"Headline: {article['properties']['headline'][0]}")
print(f"Author: {article['properties']['author'][0]['properties']['name'][0]}")
print(f"Published: {article['properties']['datePublished'][0]}")
```

## Web App Manifest Examples

### Discover Manifest Link from HTML

```python
import meta_oxide

html = """
<html>
<head>
    <link rel="manifest" href="/app.webmanifest">
    <link rel="manifest" href="/manifest.json">
</head>
</html>
"""

# Extract manifest link (returns first found)
discovery = meta_oxide.extract_manifest(html, "https://myapp.example.com")
print(f"Manifest URL: {discovery['href']}")
# Output: https://myapp.example.com/app.webmanifest
```

### Parse Complete Web App Manifest

```python
import meta_oxide
import json

manifest_json = """
{
    "name": "My Progressive Web App",
    "short_name": "MyPWA",
    "description": "An awesome progressive web application",
    "start_url": "/",
    "display": "standalone",
    "orientation": "portrait",
    "theme_color": "#2196F3",
    "background_color": "#FFFFFF",
    "icons": [
        {
            "src": "icons/icon-72.png",
            "sizes": "72x72",
            "type": "image/png"
        },
        {
            "src": "icons/icon-192.png",
            "sizes": "192x192",
            "type": "image/png",
            "purpose": "any maskable"
        },
        {
            "src": "icons/icon-512.png",
            "sizes": "512x512",
            "type": "image/png"
        }
    ]
}
"""

manifest = meta_oxide.parse_manifest(manifest_json, "https://myapp.example.com")

print(f"App Name: {manifest['name']}")
print(f"Short Name: {manifest['short_name']}")
print(f"Display Mode: {manifest['display']}")
print(f"Theme Color: {manifest['theme_color']}")

# Icons are automatically resolved to absolute URLs
for icon in manifest.get('icons', []):
    print(f"Icon: {icon['src']} ({icon['sizes']})")
# Output: https://myapp.example.com/icons/icon-72.png (72x72)
```

### Manifest with Shortcuts and Screenshots

```python
import meta_oxide

manifest_json = """
{
    "name": "Task Manager App",
    "short_name": "Tasks",
    "start_url": "/",
    "display": "standalone",
    "shortcuts": [
        {
            "name": "New Task",
            "short_name": "New",
            "description": "Create a new task",
            "url": "/new",
            "icons": [{"src": "icons/new.png", "sizes": "96x96"}]
        },
        {
            "name": "View All Tasks",
            "url": "/tasks"
        }
    ],
    "screenshots": [
        {
            "src": "screenshots/home.png",
            "sizes": "540x720",
            "type": "image/png"
        }
    ]
}
"""

manifest = meta_oxide.parse_manifest(manifest_json, "https://tasks.example.com")

print(f"App: {manifest['name']}")
print(f"Shortcuts: {len(manifest.get('shortcuts', []))}")
for shortcut in manifest.get('shortcuts', []):
    print(f"  - {shortcut['name']}: {shortcut['url']}")
    # URLs are resolved: https://tasks.example.com/new
```

### Complete PWA Discovery and Parsing

```python
import meta_oxide
import requests

def discover_and_parse_pwa_manifest(page_url):
    """Fetch a page, discover its manifest, and parse it."""
    # Fetch the HTML page
    response = requests.get(page_url)
    html = response.text

    # Discover manifest link
    discovery = meta_oxide.extract_manifest(html, page_url)

    if not discovery.get('href'):
        print("No manifest found")
        return None

    manifest_url = discovery['href']
    print(f"Found manifest: {manifest_url}")

    # Fetch manifest JSON
    manifest_response = requests.get(manifest_url)
    manifest_json = manifest_response.text

    # Parse manifest with URL resolution
    manifest = meta_oxide.parse_manifest(manifest_json, manifest_url)

    return manifest

# Usage
pwa_info = discover_and_parse_pwa_manifest("https://app.example.com")
if pwa_info:
    print(f"PWA Name: {pwa_info['name']}")
    print(f"Installable: {pwa_info.get('display') == 'standalone'}")
```

## Combined Format Examples

### Extract RDFa and JSON-LD from Same Page

```python
import meta_oxide

html = """
<html>
<head>
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Article",
        "headline": "Understanding Structured Data"
    }
    </script>
</head>
<body>
    <article vocab="https://schema.org/" typeof="BlogPosting">
        <h1 property="headline">Understanding Structured Data</h1>
        <div property="author" typeof="Person">
            <span property="name">Jane Blogger</span>
        </div>
    </article>
</body>
</html>
"""

# Extract both formats
jsonld = meta_oxide.extract_jsonld(html)
rdfa = meta_oxide.extract_rdfa(html)

print("JSON-LD:")
for item in jsonld:
    print(f"  Type: {item.get('@type')}")
    print(f"  Headline: {item.get('headline')}")

print("\nRDFa:")
for item in rdfa:
    print(f"  Type: {item['type']}")
    print(f"  Headline: {item['properties'].get('headline', ['N/A'])[0]}")
```

### Extract All Metadata Including Manifest

```python
import meta_oxide

html = """
<html>
<head>
    <title>My Progressive Web App</title>
    <meta name="description" content="An awesome PWA">
    <meta property="og:title" content="My PWA">
    <link rel="manifest" href="/manifest.json">
</head>
<body>
    <div vocab="https://schema.org/" typeof="WebApplication">
        <span property="name">My PWA</span>
    </div>
</body>
</html>
"""

# Extract everything at once
all_data = meta_oxide.extract_all(html, "https://myapp.example.com")

print(f"Title: {all_data['meta']['title']}")
print(f"OG Title: {all_data['opengraph'].get('title')}")
print(f"Manifest: {all_data.get('manifest', {}).get('href')}")
print(f"RDFa Items: {len(all_data.get('rdfa', []))}")
```

## Tips and Best Practices

1. **Always provide base_url**: When parsing web content, provide the base URL for proper link resolution

2. **Handle missing properties**: All formats are flexible; not all properties will be present

3. **Validate extracted data**: Add validation logic for critical fields

4. **Cache parsed results**: Parsing is fast but caching can help for frequently accessed pages

5. **Use type hints** (Python):
   ```python
   from typing import List, Dict, Any

   def extract_cards(html: str) -> List[Dict[str, Any]]:
       return meta_oxide.extract_hcard(html)
   ```

6. **Log extraction failures**: Keep track of pages that fail to parse

7. **Test with real-world HTML**: Test with actual websites, not just ideal examples

8. **For RDFa**: Remember that property values can be nested items (check type)

9. **For Manifests**: Always resolve relative URLs using the manifest URL, not the page URL

10. **Use extract_all()**: For most use cases, `extract_all()` is the simplest approach to get all metadata at once
