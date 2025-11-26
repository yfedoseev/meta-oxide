# MetaOxide Node.js API Reference

Complete API documentation for the Node.js package.

## Table of Contents

- [Installation](#installation)
- [Classes](#classes)
- [Methods](#methods)
- [TypeScript](#typescript)
- [Error Handling](#error-handling)
- [Examples](#examples)

## Installation

```bash
npm install meta-oxide
```

## Classes

### `MetaOxide`

Main class for extracting metadata from HTML.

```javascript
const { MetaOxide } = require('meta-oxide');
```

```typescript
import { MetaOxide } from 'meta-oxide';
```

#### Constructor

```javascript
new MetaOxide(html: string, baseUrl: string): MetaOxide
```

**Parameters:**
- `html` (string) - HTML content to parse
- `baseUrl` (string) - Base URL for resolving relative URLs

**Throws:**
- `MetaOxideError` - If HTML parsing fails

**Example:**
```javascript
const extractor = new MetaOxide(html, 'https://example.com');
```

## Methods

### `extractAll()`

Extract all metadata formats.

```javascript
extractAll(): Metadata
```

**Returns:** `Metadata` - Object containing all extracted metadata

**Example:**
```javascript
const metadata = extractor.extractAll();
console.log(metadata.title);
console.log(metadata.description);
```

**Return Type:**
```typescript
interface Metadata {
    title?: string;
    description?: string;
    keywords?: string[];
    author?: string;
    canonical?: string;
    charset?: string;
    viewport?: string;
    language?: string;
    [key: string]: any;
}
```

### `extractBasicMeta()`

Extract basic HTML metadata.

```javascript
extractBasicMeta(): BasicMetadata
```

**Returns:** `BasicMetadata` - Basic HTML metadata

**Example:**
```javascript
const basic = extractor.extractBasicMeta();
console.log(basic.title);
console.log(basic.description);
```

### `extractOpenGraph()`

Extract Open Graph metadata.

```javascript
extractOpenGraph(): OpenGraphData | null
```

**Returns:** `OpenGraphData` or `null` if not present

**Example:**
```javascript
const og = extractor.extractOpenGraph();
if (og) {
    console.log(og.title);
    console.log(og.image);
}
```

**Return Type:**
```typescript
interface OpenGraphData {
    title?: string;
    type?: string;
    image?: string;
    url?: string;
    description?: string;
    siteName?: string;
    locale?: string;
}
```

### `extractTwitterCard()`

Extract Twitter Card metadata.

```javascript
extractTwitterCard(): TwitterCardData | null
```

**Returns:** `TwitterCardData` or `null` if not present

**Example:**
```javascript
const twitter = extractor.extractTwitterCard();
if (twitter) {
    console.log(twitter.card);
    console.log(twitter.title);
}
```

**Return Type:**
```typescript
interface TwitterCardData {
    card?: string;
    site?: string;
    creator?: string;
    title?: string;
    description?: string;
    image?: string;
    imageAlt?: string;
}
```

### `extractJSONLD()`

Extract JSON-LD structured data.

```javascript
extractJSONLD(): Array<any>
```

**Returns:** Array of JSON-LD objects

**Example:**
```javascript
const jsonld = extractor.extractJSONLD();
jsonld.forEach(item => {
    console.log(item['@type']);
    console.log(item.name);
});
```

### `extractMicrodata()`

Extract Microdata items.

```javascript
extractMicrodata(): Array<MicrodataItem>
```

**Returns:** Array of Microdata items

**Example:**
```javascript
const microdata = extractor.extractMicrodata();
microdata.forEach(item => {
    console.log(item.type);
    console.log(item.properties);
});
```

**Return Type:**
```typescript
interface MicrodataItem {
    type: string[];
    properties: { [key: string]: any[] };
    id?: string;
}
```

### `extractMicroformats()`

Extract Microformats data.

```javascript
extractMicroformats(): { [key: string]: Array<any> }
```

**Returns:** Object mapping format types to items

**Example:**
```javascript
const mf = extractor.extractMicroformats();
if (mf['h-card']) {
    mf['h-card'].forEach(card => {
        console.log(card.name);
    });
}
```

### `extractDublinCore()`

Extract Dublin Core metadata.

```javascript
extractDublinCore(): DublinCoreData | null
```

**Returns:** `DublinCoreData` or `null` if not present

**Example:**
```javascript
const dc = extractor.extractDublinCore();
if (dc) {
    console.log(dc.title);
    console.log(dc.creator);
}
```

### `extractRDFa()`

Extract RDFa triples.

```javascript
extractRDFa(): Array<RdfaTriple>
```

**Returns:** Array of RDFa triples

**Example:**
```javascript
const rdfa = extractor.extractRDFa();
rdfa.forEach(triple => {
    console.log(`${triple.subject} ${triple.predicate} ${triple.object}`);
});
```

### `extractRelLinks()`

Extract link relations.

```javascript
extractRelLinks(): { [rel: string]: Array<Link> }
```

**Returns:** Object mapping rel types to links

**Example:**
```javascript
const links = extractor.extractRelLinks();
if (links.canonical) {
    console.log(links.canonical[0].href);
}
```

**Return Type:**
```typescript
interface Link {
    href: string;
    rel: string;
    media?: string;
    title?: string;
    type?: string;
    hreflang?: string;
}
```

### `extractManifest()`

Extract web app manifest.

```javascript
extractManifest(): WebManifest | null
```

**Returns:** `WebManifest` or `null` if not present

**Example:**
```javascript
const manifest = extractor.extractManifest();
if (manifest) {
    console.log(manifest.name);
    console.log(manifest.icons);
}
```

## TypeScript

Full TypeScript definitions included.

### Import Types

```typescript
import {
    MetaOxide,
    Metadata,
    OpenGraphData,
    TwitterCardData,
    MicrodataItem,
    Link,
    WebManifest,
    MetaOxideError
} from 'meta-oxide';
```

### Type-Safe Usage

```typescript
const html: string = '<!DOCTYPE html>...';
const extractor: MetaOxide = new MetaOxide(html, 'https://example.com');

const metadata: Metadata = extractor.extractAll();
const og: OpenGraphData | null = extractor.extractOpenGraph();
const twitter: TwitterCardData | null = extractor.extractTwitterCard();
const jsonld: any[] = extractor.extractJSONLD();
```

### Async Function Example

```typescript
import axios from 'axios';
import { MetaOxide, Metadata } from 'meta-oxide';

async function extractFromURL(url: string): Promise<Metadata> {
    const response = await axios.get<string>(url);
    const extractor = new MetaOxide(response.data, url);
    return extractor.extractAll();
}

// Usage
const metadata = await extractFromURL('https://example.com');
console.log(metadata.title);
```

## Error Handling

### `MetaOxideError`

Custom error class for MetaOxide errors.

```javascript
const { MetaOxide, MetaOxideError } = require('meta-oxide');

try {
    const extractor = new MetaOxide(html, url);
    const metadata = extractor.extractAll();
} catch (error) {
    if (error instanceof MetaOxideError) {
        console.error('MetaOxide error:', error.message);
    } else {
        console.error('Unexpected error:', error);
    }
}
```

### TypeScript Error Handling

```typescript
import { MetaOxide, MetaOxideError } from 'meta-oxide';

try {
    const extractor = new MetaOxide(html, url);
    const metadata = extractor.extractAll();
} catch (error) {
    if (error instanceof MetaOxideError) {
        console.error('MetaOxide error:', error.message);
    } else if (error instanceof Error) {
        console.error('Error:', error.message);
    }
}
```

## Examples

### Basic Usage (CommonJS)

```javascript
const { MetaOxide } = require('meta-oxide');

const html = `<!DOCTYPE html>...`;
const extractor = new MetaOxide(html, 'https://example.com');
const metadata = extractor.extractAll();

console.log(metadata);
```

### ES Modules

```javascript
import { MetaOxide } from 'meta-oxide';

const html = `<!DOCTYPE html>...`;
const extractor = new MetaOxide(html, 'https://example.com');
const metadata = extractor.extractAll();

console.log(metadata);
```

### Extract from URL

```javascript
const axios = require('axios');
const { MetaOxide } = require('meta-oxide');

async function extractFromURL(url) {
    const response = await axios.get(url);
    const extractor = new MetaOxide(response.data, url);
    return extractor.extractAll();
}

extractFromURL('https://example.com')
    .then(metadata => console.log(metadata))
    .catch(err => console.error(err));
```

### Parallel Extraction

```javascript
async function extractMultiple(urls) {
    const results = await Promise.all(
        urls.map(async (url) => {
            try {
                return await extractFromURL(url);
            } catch (error) {
                console.error(`Failed: ${url}`, error);
                return null;
            }
        })
    );

    return results.filter(Boolean);
}

// Usage
const urls = ['https://example.com', 'https://example.org'];
const results = await extractMultiple(urls);
```

### Express Middleware

```javascript
const express = require('express');
const axios = require('axios');
const { MetaOxide } = require('meta-oxide');

const app = express();

app.get('/extract', async (req, res) => {
    const { url } = req.query;

    if (!url) {
        return res.status(400).json({ error: 'URL required' });
    }

    try {
        const response = await axios.get(url);
        const extractor = new MetaOxide(response.data, url);
        const metadata = extractor.extractAll();

        res.json({ success: true, metadata });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.listen(3000);
```

### TypeScript with Express

```typescript
import express, { Request, Response } from 'express';
import axios from 'axios';
import { MetaOxide, Metadata } from 'meta-oxide';

const app = express();

interface ExtractQuery {
    url: string;
}

app.get('/extract', async (req: Request<{}, {}, {}, ExtractQuery>, res: Response) => {
    const { url } = req.query;

    if (!url) {
        return res.status(400).json({ error: 'URL required' });
    }

    try {
        const response = await axios.get<string>(url);
        const extractor = new MetaOxide(response.data, url);
        const metadata: Metadata = extractor.extractAll();

        res.json({ success: true, metadata });
    } catch (error) {
        res.status(500).json({ error: (error as Error).message });
    }
});

app.listen(3000);
```

### Selective Extraction

```javascript
function extractSocialMeta(html, url) {
    const extractor = new MetaOxide(html, url);

    return {
        openGraph: extractor.extractOpenGraph(),
        twitter: extractor.extractTwitterCard()
    };
}
```

### JSON Output

```javascript
const metadata = extractor.extractAll();
console.log(JSON.stringify(metadata, null, 2));
```

## Performance Tips

1. **Reuse HTTP Client**: Use a single axios instance
2. **Promise.all**: Extract from multiple URLs in parallel
3. **Selective Extraction**: Extract only needed formats
4. **Error Handling**: Always handle errors to prevent crashes

## See Also

- [Getting Started Guide](/docs/getting-started/getting-started-nodejs.md)
- [Express Integration](/docs/integrations/express-integration.md)
- [Next.js Integration](/docs/integrations/nextjs-integration.md)
- [Examples](/examples/real-world/nodejs-express-server/)
