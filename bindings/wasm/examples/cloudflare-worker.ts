/**
 * MetaOxide WASM - Cloudflare Workers Example
 *
 * Edge computing example that extracts metadata from URLs at the edge.
 * Deploy this worker to extract metadata close to your users worldwide.
 *
 * Deployment:
 *   1. Install Wrangler: npm install -g wrangler
 *   2. Configure wrangler.toml:
 *      ```toml
 *      name = "meta-oxide-worker"
 *      main = "cloudflare-worker.ts"
 *      compatibility_date = "2024-01-01"
 *      ```
 *   3. Deploy: wrangler deploy
 *
 * Usage:
 *   GET  /extract?url=https://example.com
 *   POST /extract (with HTML in body)
 *
 * Features:
 *   - Extract metadata from any URL
 *   - Direct HTML extraction via POST
 *   - JSON API response
 *   - CORS-enabled
 *   - Error handling
 */

import { extractAll, initialize } from '../lib/index';
import type { ExtractionResult } from '../lib/index';

interface Env {
    // Add KV namespaces, Durable Objects, or other bindings here
}

// CORS headers for cross-origin requests
const corsHeaders = {
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
    'Access-Control-Allow-Headers': 'Content-Type',
};

/**
 * Handle CORS preflight requests
 */
function handleOptions(): Response {
    return new Response(null, {
        status: 204,
        headers: corsHeaders,
    });
}

/**
 * Create error response
 */
function errorResponse(message: string, status: number = 400): Response {
    return new Response(
        JSON.stringify({
            error: message,
            success: false,
        }),
        {
            status,
            headers: {
                'Content-Type': 'application/json',
                ...corsHeaders,
            },
        }
    );
}

/**
 * Create success response
 */
function successResponse(data: ExtractionResult, meta?: Record<string, any>): Response {
    return new Response(
        JSON.stringify({
            success: true,
            data,
            meta,
        }),
        {
            status: 200,
            headers: {
                'Content-Type': 'application/json',
                ...corsHeaders,
            },
        }
    );
}

/**
 * Extract metadata from URL
 */
async function extractFromUrl(url: string): Promise<{ result: ExtractionResult; meta: any }> {
    const startTime = Date.now();

    // Fetch HTML from URL
    const response = await fetch(url, {
        headers: {
            'User-Agent': 'MetaOxide/1.0 (Cloudflare Worker)',
        },
    });

    if (!response.ok) {
        throw new Error(`Failed to fetch ${url}: ${response.status} ${response.statusText}`);
    }

    const contentType = response.headers.get('content-type') || '';
    if (!contentType.includes('text/html')) {
        throw new Error(`URL must return HTML content, got ${contentType}`);
    }

    const html = await response.text();
    const htmlSize = html.length;

    // Extract metadata
    const result = await extractAll(html, { baseUrl: url });

    const duration = Date.now() - startTime;

    return {
        result,
        meta: {
            url,
            htmlSize,
            duration: `${duration}ms`,
            timestamp: new Date().toISOString(),
        },
    };
}

/**
 * Extract metadata from HTML body
 */
async function extractFromHtml(html: string, baseUrl?: string): Promise<{ result: ExtractionResult; meta: any }> {
    const startTime = Date.now();

    const result = await extractAll(html, { baseUrl });

    const duration = Date.now() - startTime;

    return {
        result,
        meta: {
            htmlSize: html.length,
            duration: `${duration}ms`,
            timestamp: new Date().toISOString(),
            baseUrl: baseUrl || null,
        },
    };
}

/**
 * Main request handler
 */
export default {
    async fetch(request: Request, env: Env, ctx: ExecutionContext): Promise<Response> {
        // Initialize WASM (cached after first call)
        await initialize();

        const { method, url: requestUrl } = request;
        const url = new URL(requestUrl);

        // Handle CORS preflight
        if (method === 'OPTIONS') {
            return handleOptions();
        }

        // Root path - API documentation
        if (url.pathname === '/') {
            return new Response(
                JSON.stringify({
                    name: 'MetaOxide WASM - Cloudflare Worker',
                    version: '1.0.0',
                    endpoints: {
                        'GET /extract': {
                            description: 'Extract metadata from URL',
                            query: {
                                url: 'URL to extract metadata from (required)',
                            },
                            example: '/extract?url=https://example.com',
                        },
                        'POST /extract': {
                            description: 'Extract metadata from HTML body',
                            body: 'HTML content (text/plain or application/json)',
                            query: {
                                baseUrl: 'Base URL for resolving relative links (optional)',
                            },
                            example: 'POST /extract with HTML in body',
                        },
                    },
                }),
                {
                    status: 200,
                    headers: {
                        'Content-Type': 'application/json',
                        ...corsHeaders,
                    },
                }
            );
        }

        // Extract endpoint
        if (url.pathname === '/extract') {
            try {
                if (method === 'GET') {
                    // Extract from URL parameter
                    const targetUrl = url.searchParams.get('url');
                    if (!targetUrl) {
                        return errorResponse('Missing "url" query parameter');
                    }

                    // Validate URL
                    try {
                        new URL(targetUrl);
                    } catch {
                        return errorResponse('Invalid URL format');
                    }

                    const { result, meta } = await extractFromUrl(targetUrl);
                    return successResponse(result, meta);

                } else if (method === 'POST') {
                    // Extract from HTML body
                    const contentType = request.headers.get('content-type') || '';
                    let html: string;
                    let baseUrl: string | undefined;

                    if (contentType.includes('application/json')) {
                        const body = await request.json() as { html: string; baseUrl?: string };
                        html = body.html;
                        baseUrl = body.baseUrl;
                    } else {
                        html = await request.text();
                        baseUrl = url.searchParams.get('baseUrl') || undefined;
                    }

                    if (!html || html.length === 0) {
                        return errorResponse('Empty HTML content');
                    }

                    const { result, meta } = await extractFromHtml(html, baseUrl);
                    return successResponse(result, meta);

                } else {
                    return errorResponse('Method not allowed', 405);
                }

            } catch (error) {
                const message = error instanceof Error ? error.message : 'Unknown error';
                return errorResponse(message, 500);
            }
        }

        return errorResponse('Not found', 404);
    },
};
