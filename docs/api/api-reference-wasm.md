# MetaOxide WebAssembly API Reference

Complete API documentation for the WebAssembly bindings.

## Table of Contents

- [Installation](#installation)
- [Initialization](#initialization)
- [Classes](#classes)
- [Methods](#methods)
- [TypeScript](#typescript)
- [Platform Support](#platform-support)
- [Examples](#examples)

## Installation

```bash
npm install meta-oxide-wasm
```

## Initialization

WebAssembly modules must be initialized before use.

### Browser

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

await init();  // Initialize WASM module
```

### Node.js

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

await init();  // Initialize WASM module
```

### With Custom WASM URL

```javascript
await init('path/to/meta_oxide_wasm_bg.wasm');
```

## Classes

### `MetaOxide`

Main class for extracting metadata from HTML.

#### Constructor

```javascript
new MetaOxide(html: string, baseUrl: string): MetaOxide
```

**Parameters:**
- `html` (string) - HTML content to parse
- `baseUrl` (string) - Base URL for resolving relative URLs

**Throws:**
- Error if HTML parsing fails

**Example:**
```javascript
await init();

const extractor = new MetaOxide(html, 'https://example.com');
```

## Methods

### `extractAll()`

Extract all metadata formats.

```javascript
extractAll(): object
```

**Returns:** Object containing all extracted metadata

**Example:**
```javascript
const metadata = extractor.extractAll();
console.log(metadata.title);
console.log(metadata.description);
```

### `extractBasicMeta()`

Extract basic HTML metadata.

```javascript
extractBasicMeta(): object
```

**Returns:** Object with basic metadata

**Example:**
```javascript
const basic = extractor.extractBasicMeta();
console.log(basic.title);
console.log(basic.description);
```

### `extractOpenGraph()`

Extract Open Graph metadata.

```javascript
extractOpenGraph(): object | null
```

**Returns:** Open Graph data or `null`

**Example:**
```javascript
const og = extractor.extractOpenGraph();
if (og) {
    console.log(og.title);
    console.log(og.image);
}
```

### `extractTwitterCard()`

Extract Twitter Card metadata.

```javascript
extractTwitterCard(): object | null
```

**Returns:** Twitter Card data or `null`

**Example:**
```javascript
const twitter = extractor.extractTwitterCard();
if (twitter) {
    console.log(twitter.card);
    console.log(twitter.title);
}
```

### `extractJSONLD()`

Extract JSON-LD structured data.

```javascript
extractJSONLD(): Array<object>
```

**Returns:** Array of JSON-LD objects

**Example:**
```javascript
const jsonld = extractor.extractJSONLD();
jsonld.forEach(item => {
    console.log(item['@type']);
});
```

### `extractMicrodata()`

Extract Microdata items.

```javascript
extractMicrodata(): Array<object>
```

**Returns:** Array of Microdata items

**Example:**
```javascript
const microdata = extractor.extractMicrodata();
microdata.forEach(item => {
    console.log(item.type);
});
```

### `extractMicroformats()`

Extract Microformats data.

```javascript
extractMicroformats(): object
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

### `extractRelLinks()`

Extract link relations.

```javascript
extractRelLinks(): object
```

**Returns:** Object mapping rel types to links

**Example:**
```javascript
const links = extractor.extractRelLinks();
if (links.canonical) {
    console.log(links.canonical[0].href);
}
```

### `free()`

Free WASM memory (optional, handled by GC).

```javascript
free(): void
```

**Note:** JavaScript garbage collector will handle cleanup automatically, but you can call this manually if needed.

## TypeScript

Full TypeScript definitions included.

### Type Definitions

```typescript
declare module 'meta-oxide-wasm' {
    export default function init(input?: string | URL): Promise<void>;

    export class MetaOxide {
        constructor(html: string, baseUrl: string);

        extractAll(): Metadata;
        extractBasicMeta(): BasicMetadata;
        extractOpenGraph(): OpenGraphData | null;
        extractTwitterCard(): TwitterCardData | null;
        extractJSONLD(): any[];
        extractMicrodata(): MicrodataItem[];
        extractMicroformats(): { [key: string]: any[] };
        extractRelLinks(): { [rel: string]: Link[] };
        free(): void;
    }

    export interface Metadata {
        title?: string;
        description?: string;
        keywords?: string[];
        [key: string]: any;
    }

    export interface OpenGraphData {
        title?: string;
        type?: string;
        image?: string;
        url?: string;
        description?: string;
        siteName?: string;
    }

    export interface TwitterCardData {
        card?: string;
        site?: string;
        creator?: string;
        title?: string;
        description?: string;
        image?: string;
    }

    export interface MicrodataItem {
        type: string[];
        properties: { [key: string]: any[] };
        id?: string;
    }

    export interface Link {
        href: string;
        rel: string;
        media?: string;
        title?: string;
        type?: string;
    }
}
```

### TypeScript Usage

```typescript
import init, { MetaOxide, Metadata } from 'meta-oxide-wasm';

async function extractMetadata(html: string, url: string): Promise<Metadata> {
    await init();

    const extractor = new MetaOxide(html, url);
    return extractor.extractAll();
}

// Usage
const metadata = await extractMetadata(html, 'https://example.com');
console.log(metadata.title);
```

## Platform Support

### Browser

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

// Initialize once
let initialized = false;

async function ensureInit() {
    if (!initialized) {
        await init();
        initialized = true;
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
```

### Deno

```typescript
import init, { MetaOxide } from 'npm:meta-oxide-wasm';

await init();

const html = await Deno.readTextFile('./index.html');
const extractor = new MetaOxide(html, 'https://example.com');
const metadata = extractor.extractAll();
```

### Webpack

```javascript
// webpack.config.js
module.exports = {
    experiments: {
        asyncWebAssembly: true
    }
};

// app.js
import init, { MetaOxide } from 'meta-oxide-wasm';

await init();
const extractor = new MetaOxide(html, url);
```

### Vite

```javascript
// vite.config.js
export default {
    optimizeDeps: {
        exclude: ['meta-oxide-wasm']
    }
};

// app.js
import init, { MetaOxide } from 'meta-oxide-wasm';

await init();
const extractor = new MetaOxide(html, url);
```

## Examples

### Basic Usage

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

async function main() {
    await init();

    const html = `
        <!DOCTYPE html>
        <html>
        <head>
            <title>My Page</title>
            <meta property="og:title" content="My Page">
        </head>
        </html>
    `;

    const extractor = new MetaOxide(html, 'https://example.com');
    const metadata = extractor.extractAll();

    console.log(metadata);
}

main();
```

### React Hook

```jsx
import { useState, useEffect } from 'react';
import init, { MetaOxide } from 'meta-oxide-wasm';

function useMetaOxide() {
    const [initialized, setInitialized] = useState(false);

    useEffect(() => {
        init().then(() => setInitialized(true));
    }, []);

    const extract = (html, url) => {
        if (!initialized) return null;

        const extractor = new MetaOxide(html, url);
        return extractor.extractAll();
    };

    return { extract, initialized };
}

// Usage
function MetadataViewer({ html, url }) {
    const { extract, initialized } = useMetaOxide();
    const [metadata, setMetadata] = useState(null);

    useEffect(() => {
        if (initialized && html && url) {
            setMetadata(extract(html, url));
        }
    }, [initialized, html, url]);

    if (!metadata) return <div>Loading...</div>;

    return <pre>{JSON.stringify(metadata, null, 2)}</pre>;
}
```

### Vue Composable

```javascript
import { ref, onMounted } from 'vue';
import init, { MetaOxide } from 'meta-oxide-wasm';

export function useMetaOxide() {
    const initialized = ref(false);

    onMounted(async () => {
        await init();
        initialized.value = true;
    });

    const extract = (html, url) => {
        if (!initialized.value) return null;

        const extractor = new MetaOxide(html, url);
        return extractor.extractAll();
    };

    return { extract, initialized };
}
```

### Next.js (Server-Side)

```javascript
// pages/api/extract.js
import init, { MetaOxide } from 'meta-oxide-wasm';

let initialized = false;

export default async function handler(req, res) {
    if (!initialized) {
        await init();
        initialized = true;
    }

    const { html, url } = req.body;

    try {
        const extractor = new MetaOxide(html, url);
        const metadata = extractor.extractAll();

        res.status(200).json({ metadata });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
}
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

### Worker Thread (Browser)

```javascript
// worker.js
import init, { MetaOxide } from 'meta-oxide-wasm';

let initialized = false;

self.onmessage = async (e) => {
    if (!initialized) {
        await init();
        initialized = true;
    }

    const { html, url } = e.data;

    try {
        const extractor = new MetaOxide(html, url);
        const metadata = extractor.extractAll();

        self.postMessage({ success: true, metadata });
    } catch (error) {
        self.postMessage({ success: false, error: error.message });
    }
};

// main.js
const worker = new Worker('worker.js', { type: 'module' });

worker.postMessage({ html, url });
worker.onmessage = (e) => {
    if (e.data.success) {
        console.log(e.data.metadata);
    } else {
        console.error(e.data.error);
    }
};
```

## Error Handling

```javascript
try {
    await init();

    const extractor = new MetaOxide(html, url);
    const metadata = extractor.extractAll();

    console.log(metadata);
} catch (error) {
    console.error('Extraction failed:', error.message);
}
```

## Performance Tips

1. **Initialize once**: Call `init()` only once per application
2. **Reuse instances**: Create new MetaOxide instances per extraction
3. **Web Workers**: Offload processing to worker threads
4. **Selective extraction**: Extract only needed formats
5. **Bundle optimization**: Use tree-shaking with modern bundlers

## Memory Management

WASM memory is managed automatically by JavaScript GC, but you can manually free:

```javascript
const extractor = new MetaOxide(html, url);
const metadata = extractor.extractAll();

// Optional manual cleanup
extractor.free();
```

## See Also

- [Getting Started Guide](/docs/getting-started/getting-started-wasm.md)
- [Next.js Integration](/docs/integrations/nextjs-integration.md)
- [Examples](/examples/real-world/wasm-nextjs-app/)
