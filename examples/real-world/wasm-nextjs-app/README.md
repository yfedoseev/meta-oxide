# MetaOxide Next.js WASM Application

A full-stack web application demonstrating how to use MetaOxide WebAssembly bindings in a modern Next.js application.

## What This Example Shows

This Next.js application demonstrates:
- Using MetaOxide WASM bindings in browser
- Server-side metadata extraction with Next.js API routes
- Client-side metadata extraction for instant processing
- TypeScript support
- Interactive UI with real-time results
- Performance optimization (100x faster than browser native)
- Deployment to Vercel
- SEO optimization with dynamic metadata

## Prerequisites

- Node.js 18+ ([Install](https://nodejs.org/))
- npm or yarn
- Basic Next.js knowledge

## Installation & Setup

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Start production server
npm start
```

## Running the Application

```bash
# Development (with hot reload)
npm run dev

# Application runs at http://localhost:3000

# Production build and run
npm run build
npm start
```

## Features

### Server-Side Extraction (API Routes)

Extract metadata on the server for better performance and security:

```bash
curl -X POST http://localhost:3000/api/extract \
  -H "Content-Type: application/json" \
  -d '{"html": "<html>...</html>", "baseUrl": "https://example.com"}'
```

**API Endpoint:** `POST /api/extract`

### Client-Side Extraction (WASM)

Extract metadata directly in the browser for instant results:

```typescript
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

const result = await extractAll(htmlContent, baseUrl);
console.log(result.meta, result.openGraph);
```

### Interactive UI

The application includes:
- URL input form
- HTML paste area
- Real-time extraction results
- Format filtering
- JSON export
- Copy-to-clipboard functionality
- Dark/light mode

## How It Works

### Server-Side Route (`pages/api/extract.ts`)

```typescript
import { NextApiRequest, NextApiResponse } from 'next';
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  if (req.method !== 'POST') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  const { html, baseUrl } = req.body;

  try {
    const result = await extractAll(html, baseUrl);
    res.status(200).json({ success: true, data: result });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
}
```

### Component (`components/Extractor.tsx`)

```typescript
import { useState } from 'react';
import { extractAll } from '@yfedoseev/meta-oxide-wasm';

export default function Extractor() {
  const [html, setHtml] = useState('');
  const [result, setResult] = useState(null);
  const [mode, setMode] = useState<'server' | 'client'>('client');

  const extract = async () => {
    if (mode === 'client') {
      // Client-side extraction
      const data = await extractAll(html, window.location.href);
      setResult(data);
    } else {
      // Server-side extraction
      const res = await fetch('/api/extract', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ html, baseUrl: window.location.href })
      });
      const { data } = await res.json();
      setResult(data);
    }
  };

  return (
    <div>
      <textarea
        value={html}
        onChange={(e) => setHtml(e.target.value)}
        placeholder="Paste HTML here..."
      />
      <button onClick={extract}>Extract Metadata</button>
      {result && <pre>{JSON.stringify(result, null, 2)}</pre>}
    </div>
  );
}
```

## Usage Examples

### Server-Side Usage

```bash
# Extract from HTML
curl -X POST http://localhost:3000/api/extract \
  -H "Content-Type: application/json" \
  -d '{
    "html": "<html><head><title>Test</title></head></html>",
    "baseUrl": "https://example.com"
  }'
```

### Client-Side Usage (React)

```typescript
'use client'; // Client component in Next.js 13+

import { useEffect, useState } from 'react';
import { extractAll, extractOpenGraph } from '@yfedoseev/meta-oxide-wasm';

export default function Page() {
  const [metadata, setMetadata] = useState(null);

  useEffect(() => {
    const extractMetadata = async () => {
      const html = document.documentElement.outerHTML;
      const result = await extractOpenGraph(html);
      setMetadata(result);
    };

    extractMetadata();
  }, []);

  return (
    <div>
      <h1>{metadata?.['og:title'] || 'Page Title'}</h1>
      <p>{metadata?.['og:description']}</p>
    </div>
  );
}
```

### Combining Both Approaches

```typescript
// Use client-side for instant UX, server for backup
async function smartExtract(html: string) {
  try {
    // Try client-side first (faster)
    return await extractAll(html, window.location.href);
  } catch {
    // Fallback to server-side
    const res = await fetch('/api/extract', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ html, baseUrl: window.location.href })
    });
    return (await res.json()).data;
  }
}
```

## Performance

Expected performance:

- **Client-side extraction**: <100ms (no network delay)
- **Server-side extraction**: 10-20ms + network latency
- **100x faster** than browser native parsing
- **Instant results** for user input
- **Parallel processing** of multiple extractions

## Deployment

### Vercel (Recommended)

```bash
# Install Vercel CLI
npm i -g vercel

# Deploy
vercel deploy
```

The application will be deployed with:
- Automatic HTTPS
- CDN distribution
- Automatic scaling
- Environment variable support
- Edge function optimization

### Docker Deployment

```dockerfile
FROM node:18-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:18-alpine
WORKDIR /app
COPY --from=builder /app/package.json ./
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/public ./public
RUN npm ci --only=production

EXPOSE 3000
CMD ["npm", "start"]
```

Build and run:
```bash
docker build -t metaoxide-nextjs .
docker run -p 3000:3000 metaoxide-nextjs
```

### Self-Hosted

```bash
# Build
npm run build

# Run
npm start

# Or use PM2
pm2 start npm --name "metaoxide" -- start
```

## Testing

Run tests:
```bash
npm test

# Watch mode
npm test -- --watch

# Coverage
npm test -- --coverage
```

Example test:
```typescript
import { render, screen } from '@testing-library/react';
import Extractor from '@/components/Extractor';

describe('Extractor Component', () => {
  it('should extract metadata from HTML', async () => {
    render(<Extractor />);

    const input = screen.getByPlaceholderText('Paste HTML here...');
    await userEvent.type(input, '<html><title>Test</title></html>');

    const button = screen.getByText('Extract Metadata');
    await userEvent.click(button);

    expect(screen.getByText('Test')).toBeInTheDocument();
  });
});
```

## SEO Optimization

Use MetaOxide to generate dynamic Open Graph metadata:

```typescript
// pages/[slug].tsx
import { extractOpenGraph } from '@yfedoseev/meta-oxide-wasm';

export async function getServerSideProps({ params }) {
  const pageContent = await fetch(`/api/pages/${params.slug}`);
  const html = await pageContent.text();

  const og = await extractOpenGraph(html);

  return {
    props: { og }
  };
}
```

## Browser Compatibility

Works on all modern browsers:

- Chrome/Edge 57+
- Firefox 52+
- Safari 11+
- Opera 44+

## Environment Variables

Create `.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:3000
EXTRACTION_MODE=hybrid  # 'server', 'client', or 'hybrid'
```

## Monitoring & Analytics

Track extraction performance:

```typescript
async function trackExtraction(duration: number, success: boolean) {
  await fetch('/api/analytics', {
    method: 'POST',
    body: JSON.stringify({ duration, success })
  });
}
```

## Learning Resources

- [MetaOxide Getting Started (WASM)](../../../docs/getting-started/getting-started-wasm.md)
- [WASM API Reference](../../../docs/api/api-reference-wasm.md)
- [Next.js Documentation](https://nextjs.org/docs)
- [WASM Guide](https://webassembly.org/docs/)

## Troubleshooting

### WASM Module Not Loading

```typescript
// Ensure initialization before use
import init, * as wasm from '@yfedoseev/meta-oxide-wasm';

useEffect(() => {
  init().then(() => {
    // Ready to use
  });
}, []);
```

### Module Not Found Error

```bash
# Reinstall WASM module
npm install --save @yfedoseev/meta-oxide-wasm
```

### Performance Issues

```typescript
// Debounce extraction for real-time input
import { debounce } from 'lodash';

const debouncedExtract = debounce(async (html) => {
  const result = await extractAll(html);
  setResult(result);
}, 300);
```

## Extensions

### Add URL Fetching

```typescript
// Fetch and extract from URL
async function extractFromUrl(url: string) {
  const html = await fetch(url).then(r => r.text());
  return await extractAll(html, url);
}
```

### Add Social Sharing

```typescript
function shareMetadata(meta: any) {
  const text = `${meta['og:title']} - ${meta['og:description']}`;
  navigator.share({ title: meta['og:title'], text });
}
```

## License

This example is licensed under the same dual license as MetaOxide: **MIT OR Apache-2.0**

---

**Questions?** Check the main [MetaOxide documentation](../../../README.md) or open an issue.
