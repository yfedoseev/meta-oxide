# Getting Started with MetaOxide (WebAssembly)

Welcome to MetaOxide! This guide will help you get started with the WebAssembly bindings for MetaOxide in just 5 minutes.

## Table of Contents

- [Installation](#installation)
- [Quick Start (Browser)](#quick-start-browser)
- [Basic Extraction](#basic-extraction)
- [Platform Support](#platform-support)
- [Next Steps](#next-steps)

## Installation

Install MetaOxide via npm:

```bash
npm install meta-oxide-wasm
```

Or with yarn:

```bash
yarn add meta-oxide-wasm
```

Or with pnpm:

```bash
pnpm add meta-oxide-wasm
```

### Requirements

- Modern browser with WebAssembly support
- Node.js 14+ (for bundling)
- Works in browser, Node.js, and Deno

## Quick Start (Browser)

Here's a minimal example to extract metadata from HTML in the browser:

### HTML File

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>MetaOxide WASM Demo</title>
</head>
<body>
    <h1>MetaOxide Demo</h1>
    <textarea id="html-input" rows="10" cols="50">
<!DOCTYPE html>
<html>
<head>
    <title>Sample Page</title>
    <meta name="description" content="A sample page">
    <meta property="og:title" content="Sample Page">
</head>
</html>
    </textarea>
    <button id="extract-btn">Extract Metadata</button>
    <pre id="output"></pre>

    <script type="module">
        import init, { MetaOxide } from './node_modules/meta-oxide-wasm/meta_oxide_wasm.js';

        async function run() {
            // Initialize WASM module
            await init();

            document.getElementById('extract-btn').addEventListener('click', () => {
                const html = document.getElementById('html-input').value;
                const extractor = new MetaOxide(html, 'https://example.com');
                const metadata = extractor.extractAll();

                document.getElementById('output').textContent =
                    JSON.stringify(metadata, null, 2);
            });
        }

        run();
    </script>
</body>
</html>
```

### With a Bundler (Webpack, Vite, etc.)

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

async function extractMetadata() {
    // Initialize WASM module
    await init();

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

    const extractor = new MetaOxide(html, 'https://example.com');
    const metadata = extractor.extractAll();

    console.log('Title:', metadata.title);
    console.log('Description:', metadata.description);
}

extractMetadata();
```

## Basic Extraction

MetaOxide supports 13 metadata formats. Here's how to extract specific formats:

### Extract Open Graph Data

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

async function extractOpenGraph(html) {
    await init();

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
async function extractTwitter(html) {
    await init();

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
async function extractJSONLD(html) {
    await init();

    const extractor = new MetaOxide(html, 'https://example.com');
    const jsonldData = extractor.extractJSONLD();

    if (jsonldData) {
        console.log(JSON.stringify(jsonldData, null, 2));
    }

    return jsonldData;
}
```

### Extract from Current Page (Browser)

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

async function extractCurrentPage() {
    await init();

    const html = document.documentElement.outerHTML;
    const url = window.location.href;

    const extractor = new MetaOxide(html, url);
    return extractor.extractAll();
}

// Usage
extractCurrentPage().then(metadata => {
    console.log('Current page metadata:', metadata);
});
```

### Fetch and Extract

```javascript
async function fetchAndExtract(url) {
    await init();

    const response = await fetch(url);
    const html = await response.text();

    const extractor = new MetaOxide(html, url);
    return extractor.extractAll();
}

// Usage
fetchAndExtract('https://example.com')
    .then(metadata => console.log(metadata))
    .catch(err => console.error(err));
```

## Platform Support

### Browser

Works in all modern browsers:

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

// Initialize once
let wasmInitialized = false;

async function ensureInit() {
    if (!wasmInitialized) {
        await init();
        wasmInitialized = true;
    }
}

async function extract(html, url) {
    await ensureInit();
    const extractor = new MetaOxide(html, url);
    return extractor.extractAll();
}
```

### Node.js

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';
import { readFileSync } from 'fs';

async function extractFromFile(filepath, baseUrl) {
    await init();

    const html = readFileSync(filepath, 'utf-8');
    const extractor = new MetaOxide(html, baseUrl);
    return extractor.extractAll();
}

// Usage
extractFromFile('./index.html', 'https://example.com')
    .then(metadata => console.log(metadata));
```

### Deno

```typescript
import init, { MetaOxide } from 'npm:meta-oxide-wasm';

async function extractMetadata(html: string, url: string) {
    await init();

    const extractor = new MetaOxide(html, url);
    return extractor.extractAll();
}

const html = await Deno.readTextFile('./index.html');
const metadata = await extractMetadata(html, 'https://example.com');
console.log(metadata);
```

### React Example

```jsx
import { useState, useEffect } from 'react';
import init, { MetaOxide } from 'meta-oxide-wasm';

function MetadataExtractor() {
    const [metadata, setMetadata] = useState(null);
    const [initialized, setInitialized] = useState(false);

    useEffect(() => {
        init().then(() => setInitialized(true));
    }, []);

    const extractMetadata = (html) => {
        if (!initialized) return;

        const extractor = new MetaOxide(html, 'https://example.com');
        const result = extractor.extractAll();
        setMetadata(result);
    };

    return (
        <div>
            <button onClick={() => extractMetadata(sampleHTML)}>
                Extract
            </button>
            {metadata && (
                <pre>{JSON.stringify(metadata, null, 2)}</pre>
            )}
        </div>
    );
}
```

### Vue Example

```vue
<template>
  <div>
    <button @click="extractMetadata">Extract</button>
    <pre v-if="metadata">{{ JSON.stringify(metadata, null, 2) }}</pre>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import init, { MetaOxide } from 'meta-oxide-wasm';

export default {
  setup() {
    const metadata = ref(null);
    const initialized = ref(false);

    onMounted(async () => {
      await init();
      initialized.value = true;
    });

    const extractMetadata = () => {
      if (!initialized.value) return;

      const html = '<!DOCTYPE html>...';
      const extractor = new MetaOxide(html, 'https://example.com');
      metadata.value = extractor.extractAll();
    };

    return { metadata, extractMetadata };
  }
};
</script>
```

## Error Handling

Handle errors appropriately:

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

async function safeExtraction(html, url) {
    try {
        await init();

        const extractor = new MetaOxide(html, url);
        const metadata = extractor.extractAll();

        console.log(`Extracted ${Object.keys(metadata).length} fields`);
        return metadata;
    } catch (error) {
        console.error('Extraction failed:', error.message);
        return {};
    }
}
```

## Next Steps

Now that you've got the basics, explore more:

1. **[Complete API Reference](/docs/api/api-reference-wasm.md)** - Learn about all available methods
2. **[Real-World Examples](/examples/real-world/wasm-nextjs-app/)** - See a complete Next.js application
3. **[Integration Guides](/docs/integrations/nextjs-integration.md)** - Use with Next.js, React, Vue

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

### TypeScript Support

```typescript
import init, { MetaOxide } from 'meta-oxide-wasm';

interface Metadata {
    title?: string;
    description?: string;
    [key: string]: any;
}

async function extractMetadata(html: string, url: string): Promise<Metadata> {
    await init();

    const extractor = new MetaOxide(html, url);
    return extractor.extractAll();
}
```

### Learn More

- [Next.js Integration](/docs/integrations/nextjs-integration.md)
- [Performance Tuning](/docs/performance/performance-tuning-guide.md)
- [Architecture Overview](/docs/architecture/architecture-overview.md)

Happy extracting! 🚀
