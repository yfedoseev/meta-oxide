/**
 * MetaOxide WASM - Comprehensive Test Suite
 *
 * Tests all 11 extraction functions with real HTML examples.
 * Ensures correctness, error handling, and performance.
 */

import {
    initialize,
    extractAll,
    extractMeta,
    extractOpenGraph,
    extractTwitter,
    extractJsonLd,
    extractMicrodata,
    extractMicroformats,
    extractRDFa,
    extractDublinCore,
    extractManifest,
    extractOEmbed,
    extractRelLinks,
} from '../lib/index';

describe('MetaOxide WASM', () => {
    beforeAll(async () => {
        await initialize();
    });

    describe('Initialization', () => {
        test('should initialize without errors', async () => {
            await expect(initialize()).resolves.not.toThrow();
        });

        test('should handle multiple initializations', async () => {
            await initialize();
            await initialize();
            await expect(initialize()).resolves.not.toThrow();
        });
    });

    describe('HTML Meta Tags', () => {
        test('should extract basic meta tags', async () => {
            const html = `
                <html>
                <head>
                    <meta name="description" content="Test description">
                    <meta name="keywords" content="test, keywords">
                    <meta name="author" content="Test Author">
                </head>
                </html>
            `;

            const result = await extractMeta(html);
            expect(result).toBeDefined();
            expect(result.description).toBe('Test description');
            expect(result.keywords).toBe('test, keywords');
            expect(result.author).toBe('Test Author');
        });

        test('should handle meta with http-equiv', async () => {
            const html = `
                <html>
                <head>
                    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
                    <meta http-equiv="refresh" content="30">
                </head>
                </html>
            `;

            const result = await extractMeta(html);
            expect(result).toBeDefined();
            expect(result['content-type']).toBeDefined();
        });

        test('should handle empty HTML', async () => {
            const html = '<html></html>';
            const result = await extractMeta(html);
            expect(result).toBeDefined();
        });
    });

    describe('Open Graph', () => {
        test('should extract Open Graph metadata', async () => {
            const html = `
                <html>
                <head>
                    <meta property="og:title" content="Test Title">
                    <meta property="og:type" content="website">
                    <meta property="og:url" content="https://example.com">
                    <meta property="og:image" content="https://example.com/image.jpg">
                    <meta property="og:description" content="Test OG description">
                </head>
                </html>
            `;

            const result = await extractOpenGraph(html);
            expect(result).toBeDefined();
            expect(result.title).toBe('Test Title');
            expect(result.type).toBe('website');
            expect(result.url).toBe('https://example.com');
            expect(result.image).toBe('https://example.com/image.jpg');
            expect(result.description).toBe('Test OG description');
        });

        test('should handle multiple og:image tags', async () => {
            const html = `
                <html>
                <head>
                    <meta property="og:image" content="https://example.com/img1.jpg">
                    <meta property="og:image" content="https://example.com/img2.jpg">
                </head>
                </html>
            `;

            const result = await extractOpenGraph(html);
            expect(result.image).toBeDefined();
        });
    });

    describe('Twitter Card', () => {
        test('should extract Twitter Card metadata', async () => {
            const html = `
                <html>
                <head>
                    <meta name="twitter:card" content="summary_large_image">
                    <meta name="twitter:site" content="@example">
                    <meta name="twitter:creator" content="@author">
                    <meta name="twitter:title" content="Twitter Title">
                    <meta name="twitter:description" content="Twitter description">
                    <meta name="twitter:image" content="https://example.com/twitter.jpg">
                </head>
                </html>
            `;

            const result = await extractTwitter(html);
            expect(result).toBeDefined();
            expect(result.card).toBe('summary_large_image');
            expect(result.site).toBe('@example');
            expect(result.creator).toBe('@author');
            expect(result.title).toBe('Twitter Title');
            expect(result.description).toBe('Twitter description');
            expect(result.image).toBe('https://example.com/twitter.jpg');
        });
    });

    describe('JSON-LD', () => {
        test('should extract JSON-LD structured data', async () => {
            const html = `
                <html>
                <head>
                    <script type="application/ld+json">
                    {
                        "@context": "https://schema.org",
                        "@type": "Article",
                        "headline": "Test Article",
                        "author": "Test Author",
                        "datePublished": "2024-01-01"
                    }
                    </script>
                </head>
                </html>
            `;

            const result = await extractJsonLd(html);
            expect(result).toBeDefined();
            expect(result.items).toBeDefined();
            expect(result.items.length).toBeGreaterThan(0);
            expect(result.items[0]['@type']).toBe('Article');
            expect(result.items[0].headline).toBe('Test Article');
        });

        test('should extract multiple JSON-LD blocks', async () => {
            const html = `
                <html>
                <head>
                    <script type="application/ld+json">
                    {"@type": "Article", "headline": "Article 1"}
                    </script>
                    <script type="application/ld+json">
                    {"@type": "Person", "name": "John Doe"}
                    </script>
                </head>
                </html>
            `;

            const result = await extractJsonLd(html);
            expect(result.items.length).toBeGreaterThanOrEqual(2);
        });

        test('should handle malformed JSON-LD gracefully', async () => {
            const html = `
                <html>
                <head>
                    <script type="application/ld+json">
                    { invalid json }
                    </script>
                </head>
                </html>
            `;

            await expect(extractJsonLd(html)).resolves.toBeDefined();
        });
    });

    describe('Microdata', () => {
        test('should extract Microdata items', async () => {
            const html = `
                <html>
                <body>
                    <div itemscope itemtype="https://schema.org/Person">
                        <span itemprop="name">John Doe</span>
                        <span itemprop="jobTitle">Software Engineer</span>
                    </div>
                </body>
                </html>
            `;

            const result = await extractMicrodata(html);
            expect(result).toBeDefined();
            expect(result.items).toBeDefined();
            expect(result.items.length).toBeGreaterThan(0);
        });

        test('should handle nested Microdata', async () => {
            const html = `
                <html>
                <body>
                    <div itemscope itemtype="https://schema.org/Organization">
                        <span itemprop="name">Example Corp</span>
                        <div itemprop="address" itemscope itemtype="https://schema.org/PostalAddress">
                            <span itemprop="streetAddress">123 Main St</span>
                        </div>
                    </div>
                </body>
                </html>
            `;

            const result = await extractMicrodata(html);
            expect(result.items.length).toBeGreaterThan(0);
        });
    });

    describe('Microformats', () => {
        test('should extract h-card microformat', async () => {
            const html = `
                <html>
                <body>
                    <div class="h-card">
                        <a class="p-name u-url" href="https://example.com">John Doe</a>
                        <p class="p-note">Software Engineer</p>
                    </div>
                </body>
                </html>
            `;

            const result = await extractMicroformats(html);
            expect(result).toBeDefined();
            expect(result.items).toBeDefined();
        });

        test('should extract h-entry microformat', async () => {
            const html = `
                <html>
                <body>
                    <article class="h-entry">
                        <h1 class="p-name">Blog Post Title</h1>
                        <p class="p-summary">Post summary</p>
                        <div class="e-content">Full content here</div>
                        <time class="dt-published" datetime="2024-01-01">Jan 1, 2024</time>
                    </article>
                </body>
                </html>
            `;

            const result = await extractMicroformats(html);
            expect(result.items.length).toBeGreaterThan(0);
        });
    });

    describe('RDFa', () => {
        test('should extract RDFa triples', async () => {
            const html = `
                <html xmlns:dc="http://purl.org/dc/elements/1.1/">
                <head>
                    <title property="dc:title">Test Title</title>
                </head>
                <body>
                    <div about="https://example.com" typeof="schema:Article">
                        <span property="schema:headline">Article Headline</span>
                    </div>
                </body>
                </html>
            `;

            const result = await extractRDFa(html);
            expect(result).toBeDefined();
            expect(result.triples).toBeDefined();
        });
    });

    describe('Dublin Core', () => {
        test('should extract Dublin Core metadata', async () => {
            const html = `
                <html>
                <head>
                    <meta name="DC.title" content="Test Document">
                    <meta name="DC.creator" content="Test Author">
                    <meta name="DC.date" content="2024-01-01">
                    <meta name="DC.description" content="Test description">
                </head>
                </html>
            `;

            const result = await extractDublinCore(html);
            expect(result).toBeDefined();
            expect(result.title).toBe('Test Document');
            expect(result.creator).toBe('Test Author');
        });
    });

    describe('Web App Manifest', () => {
        test('should extract manifest link', async () => {
            const html = `
                <html>
                <head>
                    <link rel="manifest" href="/manifest.json">
                </head>
                </html>
            `;

            const result = await extractManifest(html, { baseUrl: 'https://example.com' });
            expect(result).toBeDefined();
        });
    });

    describe('oEmbed', () => {
        test('should extract oEmbed endpoints', async () => {
            const html = `
                <html>
                <head>
                    <link rel="alternate" type="application/json+oembed"
                          href="https://example.com/oembed?format=json">
                    <link rel="alternate" type="text/xml+oembed"
                          href="https://example.com/oembed?format=xml">
                </head>
                </html>
            `;

            const result = await extractOEmbed(html);
            expect(result).toBeDefined();
        });
    });

    describe('rel-* Links', () => {
        test('should extract rel links', async () => {
            const html = `
                <html>
                <head>
                    <link rel="canonical" href="https://example.com/page">
                    <link rel="alternate" hreflang="es" href="https://example.com/es">
                    <link rel="prev" href="https://example.com/prev">
                    <link rel="next" href="https://example.com/next">
                </head>
                </html>
            `;

            const result = await extractRelLinks(html);
            expect(result).toBeDefined();
        });
    });

    describe('extractAll', () => {
        test('should extract all metadata formats', async () => {
            const html = `
                <html>
                <head>
                    <title>Test Page</title>
                    <meta name="description" content="Test description">
                    <meta property="og:title" content="OG Title">
                    <meta name="twitter:card" content="summary">
                    <script type="application/ld+json">
                    {"@type": "Article", "headline": "Test"}
                    </script>
                    <link rel="canonical" href="https://example.com">
                </head>
                <body>
                    <div itemscope itemtype="https://schema.org/Person">
                        <span itemprop="name">John</span>
                    </div>
                </body>
                </html>
            `;

            const result = await extractAll(html, { baseUrl: 'https://example.com' });
            expect(result).toBeDefined();
            expect(result.meta).toBeDefined();
            expect(result.openGraph).toBeDefined();
            expect(result.twitter).toBeDefined();
            expect(result.jsonLd).toBeDefined();
            expect(result.microdata).toBeDefined();
        });

        test('should handle complex real-world HTML', async () => {
            const html = `
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Complex Test Page</title>
                    <meta name="description" content="A complex page with multiple metadata formats">
                    <meta property="og:title" content="Complex OG Title">
                    <meta property="og:type" content="website">
                    <meta property="og:url" content="https://example.com">
                    <meta property="og:image" content="https://example.com/img.jpg">
                    <meta name="twitter:card" content="summary_large_image">
                    <meta name="twitter:site" content="@example">
                    <script type="application/ld+json">
                    {
                        "@context": "https://schema.org",
                        "@type": "WebPage",
                        "name": "Complex Page",
                        "description": "Page description"
                    }
                    </script>
                    <link rel="canonical" href="https://example.com/page">
                    <link rel="manifest" href="/manifest.json">
                </head>
                <body>
                    <article class="h-entry">
                        <h1 class="p-name">Article Title</h1>
                        <div class="e-content">Article content</div>
                    </article>
                </body>
                </html>
            `;

            const result = await extractAll(html, { baseUrl: 'https://example.com' });

            expect(result.meta).toBeDefined();
            expect(result.meta?.description).toBe('A complex page with multiple metadata formats');

            expect(result.openGraph).toBeDefined();
            expect(result.openGraph?.title).toBe('Complex OG Title');

            expect(result.twitter).toBeDefined();
            expect(result.twitter?.card).toBe('summary_large_image');

            expect(result.jsonLd).toBeDefined();
            expect(result.jsonLd?.items.length).toBeGreaterThan(0);

            expect(result.microformats).toBeDefined();
        });
    });

    describe('Error Handling', () => {
        test('should handle malformed HTML gracefully', async () => {
            const html = '<html><head><meta name="broken';
            await expect(extractMeta(html)).resolves.toBeDefined();
        });

        test('should handle empty string', async () => {
            const html = '';
            await expect(extractMeta(html)).resolves.toBeDefined();
        });

        test('should handle very large HTML', async () => {
            const largeHtml = '<html><body>' + 'x'.repeat(100000) + '</body></html>';
            await expect(extractMeta(largeHtml)).resolves.toBeDefined();
        });

        test('should handle special characters', async () => {
            const html = `
                <html>
                <head>
                    <meta name="description" content="Special chars: © ® ™ 中文 العربية">
                </head>
                </html>
            `;
            const result = await extractMeta(html);
            expect(result.description).toContain('©');
        });
    });

    describe('Performance', () => {
        test('should extract metadata in reasonable time', async () => {
            const html = `
                <html>
                <head>
                    <meta name="description" content="Performance test">
                    <meta property="og:title" content="OG Title">
                </head>
                </html>
            `;

            const start = performance.now();
            await extractAll(html);
            const duration = performance.now() - start;

            // Should complete in under 100ms for simple HTML
            expect(duration).toBeLessThan(100);
        });

        test('should handle multiple extractions efficiently', async () => {
            const html = '<html><head><meta name="test" content="value"></head></html>';

            const start = performance.now();
            await Promise.all([
                extractMeta(html),
                extractMeta(html),
                extractMeta(html),
            ]);
            const duration = performance.now() - start;

            expect(duration).toBeLessThan(300);
        });
    });

    describe('Base URL Resolution', () => {
        test('should resolve relative URLs with base URL', async () => {
            const html = `
                <html>
                <head>
                    <meta property="og:image" content="/image.jpg">
                    <link rel="canonical" href="/page">
                </head>
                </html>
            `;

            const result = await extractAll(html, { baseUrl: 'https://example.com' });

            // URLs should be resolved
            expect(result.openGraph?.image).toContain('example.com');
        });

        test('should work without base URL', async () => {
            const html = `
                <html>
                <head>
                    <meta name="description" content="No base URL">
                </head>
                </html>
            `;

            await expect(extractAll(html)).resolves.toBeDefined();
        });
    });
});
