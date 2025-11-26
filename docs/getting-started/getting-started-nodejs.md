# Getting Started with MetaOxide (Node.js)

Welcome to MetaOxide! This guide will help you get started with the Node.js bindings for MetaOxide in just 5 minutes.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Basic Extraction](#basic-extraction)
- [TypeScript Usage](#typescript-usage)
- [Next Steps](#next-steps)

## Installation

Install MetaOxide via npm:

```bash
npm install meta-oxide
```

Or with yarn:

```bash
yarn add meta-oxide
```

Or with pnpm:

```bash
pnpm add meta-oxide
```

### Requirements

- Node.js 14+
- Works on Linux, macOS, and Windows

## Quick Start

Here's a minimal example to extract metadata from HTML:

```javascript
const { MetaOxide } = require('meta-oxide');

const html = `
<!DOCTYPE html>
<html>
<head>
    <title>My Page</title>
    <meta name="description" content="A great page">
    <meta property="og:title" content="My Page">
</head>
<body>Hello World</body>
</html>
`;

// Create extractor
const extractor = new MetaOxide(html, 'https://example.com');

// Extract all metadata
const metadata = extractor.extractAll();

console.log('Title:', metadata.title);
console.log('Description:', metadata.description);
```

### ES Modules

```javascript
import { MetaOxide } from 'meta-oxide';

const html = '...';
const extractor = new MetaOxide(html, 'https://example.com');
const metadata = extractor.extractAll();
```

## Basic Extraction

MetaOxide supports 13 metadata formats. Here's how to extract specific formats:

### Extract Open Graph Data

```javascript
const { MetaOxide } = require('meta-oxide');

function extractOpenGraph(html) {
    const extractor = new MetaOxide(html, 'https://example.com');
    const ogData = extractor.extractOpenGraph();

    if (ogData) {
        console.log('OG Title:', ogData.title);
        console.log('OG Type:', ogData.type);
        console.log('OG Image:', ogData.image);
    }

    return ogData;
}
```

### Extract Twitter Cards

```javascript
function extractTwitter(html) {
    const extractor = new MetaOxide(html, 'https://example.com');
    const twitterData = extractor.extractTwitterCard();

    if (twitterData) {
        console.log('Card Type:', twitterData.card);
        console.log('Title:', twitterData.title);
    }

    return twitterData;
}
```

### Extract JSON-LD Structured Data

```javascript
function extractJSONLD(html) {
    const extractor = new MetaOxide(html, 'https://example.com');
    const jsonldData = extractor.extractJSONLD();

    if (jsonldData) {
        console.log(JSON.stringify(jsonldData, null, 2));
    }

    return jsonldData;
}
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

// Usage
extractFromURL('https://example.com')
    .then(metadata => console.log(metadata))
    .catch(err => console.error(err));
```

### Async/Await Pattern

```javascript
async function extractMultipleURLs(urls) {
    const results = await Promise.all(
        urls.map(async (url) => {
            try {
                return await extractFromURL(url);
            } catch (error) {
                console.error(`Failed to extract from ${url}:`, error);
                return null;
            }
        })
    );

    return results.filter(Boolean);
}

// Usage
const urls = [
    'https://example.com',
    'https://example.org',
    'https://example.net'
];

extractMultipleURLs(urls)
    .then(results => {
        results.forEach((metadata, i) => {
            console.log(`${urls[i]}: ${metadata.title}`);
        });
    });
```

## TypeScript Usage

MetaOxide includes full TypeScript definitions:

```typescript
import { MetaOxide, Metadata, OpenGraphData, TwitterCardData } from 'meta-oxide';

const html: string = `
<!DOCTYPE html>
<html>
<head>
    <title>My Page</title>
    <meta property="og:title" content="My Page">
</head>
</html>
`;

const extractor: MetaOxide = new MetaOxide(html, 'https://example.com');

// Typed return values
const metadata: Metadata = extractor.extractAll();
const ogData: OpenGraphData | null = extractor.extractOpenGraph();
const twitterData: TwitterCardData | null = extractor.extractTwitterCard();

console.log(metadata.title);
```

### Type-Safe Extraction Function

```typescript
import axios from 'axios';
import { MetaOxide, Metadata } from 'meta-oxide';

async function extractFromURL(url: string): Promise<Metadata> {
    const response = await axios.get<string>(url);
    const extractor = new MetaOxide(response.data, url);
    return extractor.extractAll();
}

// Usage with type safety
const metadata: Metadata = await extractFromURL('https://example.com');
```

### Interface Definitions

```typescript
interface Metadata {
    title?: string;
    description?: string;
    keywords?: string[];
    author?: string;
    image?: string;
    url?: string;
    [key: string]: any;
}

interface OpenGraphData {
    title?: string;
    type?: string;
    image?: string;
    url?: string;
    description?: string;
    siteName?: string;
    [key: string]: any;
}

interface TwitterCardData {
    card?: string;
    site?: string;
    title?: string;
    description?: string;
    image?: string;
    [key: string]: any;
}
```

## Error Handling

Handle errors appropriately:

```javascript
const { MetaOxide, MetaOxideError } = require('meta-oxide');

function safeExtraction(html, url) {
    try {
        const extractor = new MetaOxide(html, url);
        const metadata = extractor.extractAll();
        console.log(`Extracted ${Object.keys(metadata).length} fields`);
        return metadata;
    } catch (error) {
        if (error instanceof MetaOxideError) {
            console.error('Extraction failed:', error.message);
        } else {
            console.error('Unexpected error:', error);
        }
        return {};
    }
}
```

## Next Steps

Now that you've got the basics, explore more:

1. **[Complete API Reference](/docs/api/api-reference-nodejs.md)** - Learn about all available methods
2. **[Real-World Examples](/examples/real-world/nodejs-express-server/)** - See a complete Express.js server
3. **[Integration Guides](/docs/integrations/express-integration.md)** - Use with Express, Next.js

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

### Express.js Middleware Example

```javascript
const express = require('express');
const { MetaOxide } = require('meta-oxide');
const axios = require('axios');

const app = express();

app.get('/extract', async (req, res) => {
    const { url } = req.query;

    if (!url) {
        return res.status(400).json({ error: 'URL parameter required' });
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

app.listen(3000, () => {
    console.log('Server running on port 3000');
});
```

### Learn More

- [Express Integration](/docs/integrations/express-integration.md)
- [Next.js Integration](/docs/integrations/nextjs-integration.md)
- [Performance Tuning](/docs/performance/performance-tuning-guide.md)

Happy extracting! 🚀
