#!/usr/bin/env -S deno run --allow-net --allow-read

/**
 * MetaOxide WASM - Deno Example
 *
 * Demonstrates using MetaOxide in Deno with full TypeScript support.
 * Deno's modern runtime makes it ideal for server-side metadata extraction.
 *
 * Usage:
 *   deno run --allow-net --allow-read deno.ts https://example.com
 *   deno run --allow-read deno.ts local-file.html
 *
 * Install as a command:
 *   deno install --allow-net --allow-read -n meta-extract deno.ts
 */

import { extractAll, initialize, type ExtractionResult } from '../lib/index.ts';

// Terminal colors for beautiful output
const colors = {
    reset: '\x1b[0m',
    bright: '\x1b[1m',
    dim: '\x1b[2m',
    cyan: '\x1b[36m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m',
    red: '\x1b[31m',
};

function log(color: string, ...args: string[]): void {
    console.log(color + args.join(' ') + colors.reset);
}

async function fetchHtml(url: string): Promise<string> {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
        return await response.text();
    } catch (error) {
        throw new Error(`Failed to fetch ${url}: ${error.message}`);
    }
}

async function readLocalFile(filepath: string): Promise<string> {
    try {
        return await Deno.readTextFile(filepath);
    } catch (error) {
        throw new Error(`Failed to read file ${filepath}: ${error.message}`);
    }
}

function formatMetadata(result: ExtractionResult): string {
    const sections: string[] = [];

    // Calculate summary statistics
    const formatCount = Object.values(result).filter(v => v && Object.keys(v).length > 0).length;
    const totalFields = Object.values(result).reduce((sum, format) => {
        if (!format) return sum;
        return sum + Object.keys(format).length;
    }, 0);

    sections.push(`\n${colors.bright}📊 Extraction Summary${colors.reset}`);
    sections.push(`${colors.dim}  Formats found: ${colors.green}${formatCount}${colors.reset}`);
    sections.push(`${colors.dim}  Total fields: ${colors.green}${totalFields}${colors.reset}\n`);

    // HTML Meta Tags
    if (result.meta && Object.keys(result.meta).length > 0) {
        sections.push(`${colors.cyan}${colors.bright}📝 HTML Meta Tags${colors.reset}`);
        Object.entries(result.meta).forEach(([key, value]) => {
            const displayValue = Array.isArray(value) ? value.join(', ') : value;
            sections.push(`  ${colors.dim}${key}:${colors.reset} ${displayValue}`);
        });
        sections.push('');
    }

    // Open Graph
    if (result.openGraph && Object.keys(result.openGraph).length > 0) {
        sections.push(`${colors.green}${colors.bright}🌐 Open Graph${colors.reset}`);
        Object.entries(result.openGraph).forEach(([key, value]) => {
            const displayValue = Array.isArray(value) ? value.join(', ') : String(value);
            sections.push(`  ${colors.dim}${key}:${colors.reset} ${displayValue}`);
        });
        sections.push('');
    }

    // Twitter Card
    if (result.twitter && Object.keys(result.twitter).length > 0) {
        sections.push(`${colors.blue}${colors.bright}🐦 Twitter Card${colors.reset}`);
        Object.entries(result.twitter).forEach(([key, value]) => {
            sections.push(`  ${colors.dim}${key}:${colors.reset} ${value}`);
        });
        sections.push('');
    }

    // JSON-LD
    if (result.jsonLd?.items && result.jsonLd.items.length > 0) {
        sections.push(`${colors.yellow}${colors.bright}📋 JSON-LD${colors.reset} ${colors.dim}(${result.jsonLd.items.length} items)${colors.reset}`);
        result.jsonLd.items.forEach((item, idx) => {
            const preview = JSON.stringify(item).substring(0, 80);
            sections.push(`  ${colors.dim}Item ${idx + 1}:${colors.reset} ${preview}...`);
        });
        sections.push('');
    }

    // Microdata
    if (result.microdata?.items && result.microdata.items.length > 0) {
        sections.push(`${colors.magenta}${colors.bright}🔬 Microdata${colors.reset} ${colors.dim}(${result.microdata.items.length} items)${colors.reset}`);
        result.microdata.items.forEach((item, idx) => {
            const type = item.type?.join(', ') || 'unknown';
            const propCount = Object.keys(item.properties || {}).length;
            sections.push(`  ${colors.dim}Item ${idx + 1} [${type}]:${colors.reset} ${propCount} properties`);
        });
        sections.push('');
    }

    // Microformats
    if (result.microformats?.items && result.microformats.items.length > 0) {
        sections.push(`${colors.cyan}${colors.bright}🏷️  Microformats${colors.reset} ${colors.dim}(${result.microformats.items.length} items)${colors.reset}`);
        result.microformats.items.forEach((item, idx) => {
            const type = item.type.join(', ');
            const propCount = Object.keys(item.properties).length;
            sections.push(`  ${colors.dim}Item ${idx + 1} [${type}]:${colors.reset} ${propCount} properties`);
        });
        sections.push('');
    }

    // Compact display for other formats
    const otherFormats = [
        { key: 'rdfa' as const, title: 'RDFa', icon: '🔗' },
        { key: 'dublinCore' as const, title: 'Dublin Core', icon: '📚' },
        { key: 'manifest' as const, title: 'Web App Manifest', icon: '📱' },
        { key: 'oembed' as const, title: 'oEmbed', icon: '🎬' },
        { key: 'relLinks' as const, title: 'rel-* Links', icon: '🔗' },
    ];

    const hasOtherFormats = otherFormats.some(({ key }) => {
        const data = result[key];
        return data && Object.keys(data).length > 0;
    });

    if (hasOtherFormats) {
        sections.push(`${colors.bright}Other Formats:${colors.reset}`);
        otherFormats.forEach(({ key, title, icon }) => {
            const data = result[key];
            if (data && Object.keys(data).length > 0) {
                sections.push(`  ${colors.dim}${icon} ${title}: ${Object.keys(data).length} entries${colors.reset}`);
            }
        });
        sections.push('');
    }

    return sections.join('\n');
}

async function main() {
    const args = Deno.args;

    if (args.length === 0) {
        log(colors.yellow, '\n📚 MetaOxide WASM - Deno Example\n');
        log(colors.dim, 'Usage:');
        log(colors.dim, '  deno run --allow-net --allow-read deno.ts <url-or-file>');
        log(colors.dim, '\nExamples:');
        log(colors.dim, '  deno run --allow-net deno.ts https://example.com');
        log(colors.dim, '  deno run --allow-read deno.ts local-file.html');
        log(colors.dim, '  deno run --allow-net --allow-write deno.ts https://example.com --json');
        log(colors.dim, '\nOptions:');
        log(colors.dim, '  --json    Write full output to metadata.json\n');
        Deno.exit(1);
    }

    const input = args[0];
    const isUrl = input.startsWith('http://') || input.startsWith('https://');
    const writeJson = args.includes('--json');

    log(colors.bright, `\n🚀 MetaOxide WASM - Deno TypeScript Example\n`);
    log(colors.dim, `Source: ${input}`);
    log(colors.dim, `Runtime: Deno ${Deno.version.deno} (TypeScript ${Deno.version.typescript})`);

    try {
        // Initialize WASM module
        log(colors.dim, '\nInitializing WASM module...');
        await initialize();

        // Fetch or read HTML
        log(colors.dim, isUrl ? 'Fetching HTML...' : 'Reading file...');
        const html = isUrl ? await fetchHtml(input) : await readLocalFile(input);
        const baseUrl = isUrl ? input : undefined;

        // Extract metadata with timing
        log(colors.dim, 'Extracting metadata...');
        const startTime = performance.now();
        const result = await extractAll(html, { baseUrl });
        const duration = (performance.now() - startTime).toFixed(2);

        // Display formatted results
        console.log(formatMetadata(result));

        log(colors.green, `✅ Extraction completed in ${duration}ms\n`);

        // Optional: Write JSON output
        if (writeJson) {
            const outputFile = 'metadata.json';
            await Deno.writeTextFile(outputFile, JSON.stringify(result, null, 2));
            log(colors.dim, `Full JSON written to ${outputFile}`);
        }

    } catch (error) {
        log(colors.red, `\n❌ Error: ${error.message}\n`);
        Deno.exit(1);
    }
}

// Run if executed directly
if (import.meta.main) {
    main();
}

export { main };
