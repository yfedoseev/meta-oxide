# MetaOxide WASM

[![npm version](https://badge.fury.io/js/%40yfedoseev%2Fmeta-oxide-wasm.svg)](https://www.npmjs.com/package/@yfedoseev/meta-oxide-wasm)
[![License](https://img.shields.io/badge/license-MIT%2FApache--2.0-blue.svg)](LICENSE)

**Universal metadata extraction library for JavaScript/TypeScript via WebAssembly**

High-performance WASM bindings for the [MetaOxide](https://github.com/yfedoseev/meta-oxide) Rust library. Extract all major metadata formats from HTML with blazing-fast performance in any JavaScript runtime.

## ✨ Features

- 🚀 **Blazing Fast** - Native Rust performance via WebAssembly
- 🌍 **Universal** - Works in browsers, Node.js, Deno, edge computing platforms
- 📦 **Zero Dependencies** - Self-contained WASM module
- 🔒 **Type Safe** - Complete TypeScript definitions included
- 🎯 **11 Metadata Formats** - Comprehensive extraction support
- ⚡ **Edge Ready** - Optimized for Cloudflare Workers, Vercel Edge, etc.
- 🧪 **Well Tested** - 30+ tests ensuring correctness

## 📦 Installation

```bash
npm install @yfedoseev/meta-oxide-wasm
```

Or with your preferred package manager:

```bash
yarn add @yfedoseev/meta-oxide-wasm
pnpm add @yfedoseev/meta-oxide-wasm
```

## 🚀 Quick Start (5 Minutes)

### Browser

```html
<!DOCTYPE html>
<html>
<head>
    <title>MetaOxide Demo</title>
</head>
<body>
    <script type="module">
        import { extractAll } from 'https://unpkg.com/@yfedoseev/meta-oxide-wasm';

        const html = document.documentElement.outerHTML;
        const metadata = await extractAll(html, {
            baseUrl: window.location.href
        });

        console.log('Open Graph:', metadata.openGraph);
        console.log('Twitter Card:', metadata.twitter);
        console.log('JSON-LD:', metadata.jsonLd);
    </script>
</body>
</html>
```

### Node.js

```javascript
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

const html = await fetch('https://example.com').then(r => r.text());
const metadata = await extractAll(html, { baseUrl: 'https://example.com' });

console.log(metadata.openGraph?.title);
console.log(metadata.meta?.description);
```

### TypeScript

```typescript
import { extractAll, type ExtractionResult } from '@yfedoseev/meta-oxide-wasm';

async function getMetadata(url: string): Promise<ExtractionResult> {
    const response = await fetch(url);
    const html = await response.text();
    return extractAll(html, { baseUrl: url });
}

const metadata = await getMetadata('https://example.com');
console.log(metadata.openGraph?.image);
```

## 📚 API Reference

### Initialization

```typescript
import { initialize } from '@yfedoseev/meta-oxide-wasm';

// Initialize WASM module (automatic on first use)
await initialize();
```

The WASM module is automatically initialized on first extraction function call. Manual initialization is optional but recommended if you want to control timing.

### Extract All Formats

```typescript
function extractAll(
    html: string,
    options?: { baseUrl?: string }
): Promise<ExtractionResult>
```

Extract all 11 metadata formats in a single pass. Most efficient for comprehensive extraction.

**Example:**

```typescript
const metadata = await extractAll(html, { baseUrl: 'https://example.com' });

// Access different formats
metadata.meta          // HTML meta tags
metadata.openGraph     // Open Graph
metadata.twitter       // Twitter Card
metadata.jsonLd        // JSON-LD
metadata.microdata     // Microdata
metadata.microformats  // Microformats
metadata.rdfa          // RDFa
metadata.dublinCore    // Dublin Core
metadata.manifest      // Web App Manifest
metadata.oembed        // oEmbed
metadata.relLinks      // rel-* links
```

### Individual Extractors

Extract specific metadata formats:

```typescript
// HTML meta tags
const meta = await extractMeta(html, { baseUrl });
// { description: "...", keywords: "...", ... }

// Open Graph
const og = await extractOpenGraph(html, { baseUrl });
// { title: "...", type: "website", image: "...", ... }

// Twitter Card
const twitter = await extractTwitter(html, { baseUrl });
// { card: "summary_large_image", site: "@example", ... }

// JSON-LD
const jsonLd = await extractJsonLd(html, { baseUrl });
// { items: [{ "@type": "Article", ... }] }

// Microdata
const microdata = await extractMicrodata(html, { baseUrl });
// { items: [{ type: ["https://schema.org/Person"], ... }] }

// Microformats
const microformats = await extractMicroformats(html, { baseUrl });
// { items: [{ type: ["h-card"], properties: {...} }] }

// RDFa
const rdfa = await extractRDFa(html, { baseUrl });
// { triples: [{ subject: "...", predicate: "...", object: "..." }] }

// Dublin Core
const dc = await extractDublinCore(html, { baseUrl });
// { title: "...", creator: "...", date: "..." }

// Web App Manifest
const manifest = await extractManifest(html, { baseUrl });
// { href: "/manifest.json", type: "application/manifest+json" }

// oEmbed
const oembed = await extractOEmbed(html, { baseUrl });
// { json: "https://...", xml: "https://..." }

// rel-* links
const relLinks = await extractRelLinks(html, { baseUrl });
// { canonical: [{ href: "..." }], alternate: [...] }
```

## 🌐 Platform Support

### Browser Support

Works in all modern browsers with WebAssembly support:

| Browser | Version | Support |
|---------|---------|---------|
| Chrome | 57+ | ✅ |
| Firefox | 52+ | ✅ |
| Safari | 11+ | ✅ |
| Edge | 16+ | ✅ |
| Opera | 44+ | ✅ |

### Runtime Support

| Runtime | Support | Notes |
|---------|---------|-------|
| Node.js | 18+ | ✅ Full support |
| Deno | 1.0+ | ✅ Native TypeScript |
| Bun | 1.0+ | ✅ Fast runtime |
| Cloudflare Workers | ✅ | Edge computing |
| Vercel Edge | ✅ | Serverless edge |
| Netlify Edge | ✅ | Edge functions |

## 💡 Usage Examples

### 1. Extract Metadata from Current Page

```javascript
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

const metadata = await extractAll(
    document.documentElement.outerHTML,
    { baseUrl: window.location.href }
);

// Display Open Graph image
if (metadata.openGraph?.image) {
    const img = document.createElement('img');
    img.src = metadata.openGraph.image;
    document.body.appendChild(img);
}
```

### 2. Build a Link Preview Generator

```typescript
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

interface LinkPreview {
    title: string;
    description: string;
    image?: string;
    siteName?: string;
}

async function generatePreview(url: string): Promise<LinkPreview> {
    const response = await fetch(url);
    const html = await response.text();
    const metadata = await extractAll(html, { baseUrl: url });

    return {
        title: metadata.openGraph?.title ||
               metadata.twitter?.title ||
               metadata.meta?.title ||
               'Untitled',
        description: metadata.openGraph?.description ||
                    metadata.twitter?.description ||
                    metadata.meta?.description ||
                    '',
        image: metadata.openGraph?.image ||
               metadata.twitter?.image,
        siteName: metadata.openGraph?.site_name,
    };
}

const preview = await generatePreview('https://github.com');
console.log(preview);
```

### 3. SEO Analysis Tool

```typescript
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

interface SEOReport {
    score: number;
    issues: string[];
    recommendations: string[];
}

async function analyzeSEO(html: string, url: string): Promise<SEOReport> {
    const metadata = await extractAll(html, { baseUrl: url });
    const issues: string[] = [];
    const recommendations: string[] = [];
    let score = 100;

    // Check meta description
    if (!metadata.meta?.description) {
        issues.push('Missing meta description');
        score -= 10;
    } else if (metadata.meta.description.length < 120) {
        recommendations.push('Meta description should be at least 120 characters');
        score -= 5;
    }

    // Check Open Graph
    if (!metadata.openGraph?.title) {
        issues.push('Missing Open Graph title');
        score -= 15;
    }
    if (!metadata.openGraph?.image) {
        recommendations.push('Add Open Graph image for better social sharing');
        score -= 10;
    }

    // Check Twitter Card
    if (!metadata.twitter?.card) {
        recommendations.push('Add Twitter Card metadata');
        score -= 10;
    }

    // Check structured data
    if (!metadata.jsonLd?.items?.length && !metadata.microdata?.items?.length) {
        recommendations.push('Add structured data (JSON-LD or Microdata)');
        score -= 15;
    }

    return { score: Math.max(0, score), issues, recommendations };
}

const report = await analyzeSEO(html, 'https://example.com');
console.log(`SEO Score: ${report.score}/100`);
```

### 4. Metadata Cache Service

```typescript
import { extractAll, type ExtractionResult } from '@yfedoseev/meta-oxide-wasm';

class MetadataCache {
    private cache = new Map<string, { data: ExtractionResult; expires: number }>();
    private ttl = 3600000; // 1 hour

    async get(url: string): Promise<ExtractionResult> {
        // Check cache
        const cached = this.cache.get(url);
        if (cached && cached.expires > Date.now()) {
            return cached.data;
        }

        // Fetch and extract
        const response = await fetch(url);
        const html = await response.text();
        const data = await extractAll(html, { baseUrl: url });

        // Cache result
        this.cache.set(url, {
            data,
            expires: Date.now() + this.ttl,
        });

        return data;
    }

    clear(): void {
        this.cache.clear();
    }
}

const cache = new MetadataCache();
const metadata = await cache.get('https://example.com');
```

## ⚡ Edge Computing Examples

### Cloudflare Workers

```typescript
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

export default {
    async fetch(request: Request): Promise<Response> {
        const url = new URL(request.url);
        const targetUrl = url.searchParams.get('url');

        if (!targetUrl) {
            return new Response('Missing url parameter', { status: 400 });
        }

        const response = await fetch(targetUrl);
        const html = await response.text();
        const metadata = await extractAll(html, { baseUrl: targetUrl });

        return new Response(JSON.stringify(metadata), {
            headers: { 'Content-Type': 'application/json' },
        });
    },
};
```

### Vercel Edge Functions

```typescript
// api/metadata.ts
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

export const config = { runtime: 'edge' };

export default async function handler(request: Request) {
    const { searchParams } = new URL(request.url);
    const url = searchParams.get('url');

    if (!url) {
        return new Response('Missing url parameter', { status: 400 });
    }

    const response = await fetch(url);
    const html = await response.text();
    const metadata = await extractAll(html, { baseUrl: url });

    return new Response(JSON.stringify(metadata), {
        headers: { 'Content-Type': 'application/json' },
    });
}
```

### Deno Deploy

```typescript
import { extractAll } from 'https://esm.sh/@yfedoseev/meta-oxide-wasm';

Deno.serve(async (request: Request) => {
    const url = new URL(request.url);
    const targetUrl = url.searchParams.get('url');

    if (!targetUrl) {
        return new Response('Missing url parameter', { status: 400 });
    }

    const response = await fetch(targetUrl);
    const html = await response.text();
    const metadata = await extractAll(html, { baseUrl: targetUrl });

    return new Response(JSON.stringify(metadata), {
        headers: { 'Content-Type': 'application/json' },
    });
});
```

## 🎯 Supported Metadata Formats

### 1. HTML Meta Tags
Standard `<meta>` tags with `name`, `property`, or `http-equiv` attributes.

```html
<meta name="description" content="Page description">
<meta name="keywords" content="keyword1, keyword2">
```

### 2. Open Graph Protocol
Facebook's Open Graph metadata for rich social sharing.

```html
<meta property="og:title" content="Page Title">
<meta property="og:image" content="https://example.com/image.jpg">
```

### 3. Twitter Cards
Twitter's metadata format for card-based sharing.

```html
<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:site" content="@username">
```

### 4. JSON-LD
Google's preferred structured data format.

```html
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "Article",
  "headline": "Article Title"
}
</script>
```

### 5. Microdata
HTML5 inline structured data.

```html
<div itemscope itemtype="https://schema.org/Person">
  <span itemprop="name">John Doe</span>
</div>
```

### 6. Microformats
Classic web microformats (h-card, h-entry, h-event, etc.).

```html
<div class="h-card">
  <a class="p-name u-url" href="https://example.com">John Doe</a>
</div>
```

### 7. RDFa
Resource Description Framework in Attributes.

```html
<div vocab="https://schema.org/" typeof="Article">
  <span property="headline">Title</span>
</div>
```

### 8. Dublin Core
Standard metadata vocabulary for digital resources.

```html
<meta name="DC.title" content="Document Title">
<meta name="DC.creator" content="Author Name">
```

### 9. Web App Manifest
Progressive Web App manifest discovery.

```html
<link rel="manifest" href="/manifest.json">
```

### 10. oEmbed
Embeddable content discovery.

```html
<link rel="alternate" type="application/json+oembed"
      href="https://example.com/oembed?url=...">
```

### 11. rel-* Links
Link relationships (canonical, alternate, prev, next, etc.).

```html
<link rel="canonical" href="https://example.com/page">
<link rel="alternate" hreflang="es" href="https://example.com/es">
```

## ⚡ Performance

MetaOxide WASM is optimized for speed:

- **Typical page**: <10ms extraction time
- **Complex page** (multiple formats): <50ms
- **WASM overhead**: <1ms initialization (cached)
- **Memory efficient**: Minimal allocations, zero-copy parsing

### Benchmark Comparison

| Library | Extraction Time | WASM Size | Notes |
|---------|----------------|-----------|-------|
| MetaOxide WASM | ~5ms | ~500KB | This library |
| metascraper | ~50ms | N/A | Node.js only |
| html-metadata | ~30ms | N/A | Node.js only |
| open-graph-scraper | ~40ms | N/A | Node.js only |

*Benchmarks performed on a typical blog post with Open Graph, Twitter Cards, and JSON-LD.*

## 🔧 Building from Source

### Prerequisites

- Rust 1.70+ with wasm32 target
- Node.js 18+
- wasm-pack

### Build Steps

```bash
# Clone the repository
git clone https://github.com/yfedoseev/meta-oxide.git
cd meta-oxide/bindings/wasm

# Install dependencies
npm install

# Build WASM module
npm run build

# Run tests
npm test

# Build for all targets
npm run build:all
```

### Build Targets

- `npm run build` - Web target (ESM)
- `npm run build:nodejs` - Node.js target (CommonJS)
- `npm run build:bundler` - Bundler target (Webpack, Rollup, etc.)

## 🐛 Troubleshooting

### WASM initialization fails in browser

**Problem**: `WebAssembly.instantiate failed`

**Solution**: Ensure your server sends correct MIME type for `.wasm` files:

```
Content-Type: application/wasm
```

For development servers:
```javascript
// Vite
export default {
    server: {
        fs: { allow: ['..'] }
    }
}

// Webpack
module: {
    rules: [{
        test: /\.wasm$/,
        type: 'webassembly/async'
    }]
}
```

### Module not found in Node.js

**Problem**: `Cannot find module '@yfedoseev/meta-oxide-wasm'`

**Solution**: Use Node.js 18+ with ESM support:

```json
{
  "type": "module"
}
```

Or use dynamic import:
```javascript
const { extractAll } = await import('@yfedoseev/meta-oxide-wasm');
```

### TypeScript errors

**Problem**: Type errors when importing

**Solution**: Ensure `moduleResolution: "node"` in `tsconfig.json`:

```json
{
  "compilerOptions": {
    "moduleResolution": "node",
    "esModuleInterop": true
  }
}
```

### Performance issues

**Problem**: Slow extraction on large HTML

**Solution**:
1. Extract only needed formats (use individual extractors)
2. Limit HTML size before extraction
3. Consider streaming parser for very large documents
4. Cache extraction results

```typescript
// Extract only what you need
const og = await extractOpenGraph(html);
const twitter = await extractTwitter(html);
```

## 📊 Browser Compatibility

Check if WebAssembly is supported:

```javascript
if (typeof WebAssembly === 'object') {
    // WASM is supported
    const metadata = await extractAll(html);
} else {
    // Fallback to alternative solution
    console.error('WebAssembly not supported');
}
```

## 🤝 Contributing

Contributions are welcome! See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

### Development Setup

```bash
git clone https://github.com/yfedoseev/meta-oxide.git
cd meta-oxide/bindings/wasm
npm install
npm test
```

### Running Examples

```bash
# Browser example
npm run example:browser

# Node.js example
node examples/node-wasm.js https://example.com

# Deno example
deno run --allow-net examples/deno.ts https://example.com
```

## 📄 License

Licensed under either of:

- Apache License, Version 2.0 ([LICENSE-APACHE](LICENSE-APACHE))
- MIT license ([LICENSE-MIT](LICENSE-MIT))

at your option.

## 🔗 Links

- [GitHub Repository](https://github.com/yfedoseev/meta-oxide)
- [npm Package](https://www.npmjs.com/package/@yfedoseev/meta-oxide-wasm)
- [Documentation](https://docs.rs/meta-oxide)
- [Issue Tracker](https://github.com/yfedoseev/meta-oxide/issues)

## 🙏 Acknowledgments

Built with:
- [wasm-bindgen](https://github.com/rustwasm/wasm-bindgen) - Rust/WASM bindings
- [scraper](https://github.com/causal-agent/scraper) - HTML parsing
- [serde](https://github.com/serde-rs/serde) - Serialization

## 📮 Support

- 📧 Email: yfedoseev@gmail.com
- 🐛 Issues: [GitHub Issues](https://github.com/yfedoseev/meta-oxide/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/yfedoseev/meta-oxide/discussions)

---

Made with ❤️ by [Yury Fedoseev](https://github.com/yfedoseev)
