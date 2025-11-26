#!/usr/bin/env node

/**
 * MetaOxide WASM - Node.js Example
 *
 * Demonstrates using the WASM binding in Node.js as an alternative
 * to the native N-API binding. Useful for environments where native
 * modules aren't supported or for consistent cross-platform behavior.
 *
 * Usage:
 *   node node-wasm.js https://example.com
 *   node node-wasm.js local-file.html
 */

const { readFile } = require('fs/promises');
const { extractAll, initialize } = require('../lib/index.js');

// Colors for terminal output
const colors = {
    reset: '\x1b[0m',
    bright: '\x1b[1m',
    dim: '\x1b[2m',
    cyan: '\x1b[36m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m',
};

function log(color, ...args) {
    console.log(color + args.join(' ') + colors.reset);
}

async function fetchHtml(url) {
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

async function readLocalFile(filepath) {
    try {
        return await readFile(filepath, 'utf-8');
    } catch (error) {
        throw new Error(`Failed to read file ${filepath}: ${error.message}`);
    }
}

function formatMetadata(result) {
    const sections = [];

    // Summary
    const formatCount = Object.values(result).filter(v => v && Object.keys(v).length > 0).length;
    const totalFields = Object.values(result).reduce((sum, format) => {
        if (!format) return sum;
        return sum + Object.keys(format).length;
    }, 0);

    sections.push(`\n${colors.bright}📊 Summary${colors.reset}`);
    sections.push(`${colors.dim}  Formats found: ${formatCount}`);
    sections.push(`  Total fields: ${totalFields}${colors.reset}\n`);

    // HTML Meta Tags
    if (result.meta && Object.keys(result.meta).length > 0) {
        sections.push(`${colors.cyan}📝 HTML Meta Tags${colors.reset}`);
        Object.entries(result.meta).forEach(([key, value]) => {
            const displayValue = Array.isArray(value) ? value.join(', ') : value;
            sections.push(`  ${colors.dim}${key}:${colors.reset} ${displayValue}`);
        });
        sections.push('');
    }

    // Open Graph
    if (result.openGraph && Object.keys(result.openGraph).length > 0) {
        sections.push(`${colors.green}🌐 Open Graph${colors.reset}`);
        Object.entries(result.openGraph).forEach(([key, value]) => {
            const displayValue = Array.isArray(value) ? value.join(', ') : value;
            sections.push(`  ${colors.dim}${key}:${colors.reset} ${displayValue}`);
        });
        sections.push('');
    }

    // Twitter Card
    if (result.twitter && Object.keys(result.twitter).length > 0) {
        sections.push(`${colors.blue}🐦 Twitter Card${colors.reset}`);
        Object.entries(result.twitter).forEach(([key, value]) => {
            sections.push(`  ${colors.dim}${key}:${colors.reset} ${value}`);
        });
        sections.push('');
    }

    // JSON-LD
    if (result.jsonLd && result.jsonLd.items && result.jsonLd.items.length > 0) {
        sections.push(`${colors.yellow}📋 JSON-LD (${result.jsonLd.items.length} items)${colors.reset}`);
        result.jsonLd.items.forEach((item, idx) => {
            sections.push(`  ${colors.dim}Item ${idx + 1}:${colors.reset} ${JSON.stringify(item).substring(0, 100)}...`);
        });
        sections.push('');
    }

    // Microdata
    if (result.microdata && result.microdata.items && result.microdata.items.length > 0) {
        sections.push(`${colors.magenta}🔬 Microdata (${result.microdata.items.length} items)${colors.reset}`);
        result.microdata.items.forEach((item, idx) => {
            const type = item.type ? item.type.join(', ') : 'unknown';
            sections.push(`  ${colors.dim}Item ${idx + 1} [${type}]:${colors.reset} ${Object.keys(item.properties || {}).length} properties`);
        });
        sections.push('');
    }

    // Microformats
    if (result.microformats && result.microformats.items && result.microformats.items.length > 0) {
        sections.push(`${colors.cyan}🏷️  Microformats (${result.microformats.items.length} items)${colors.reset}`);
        result.microformats.items.forEach((item, idx) => {
            const type = item.type.join(', ');
            sections.push(`  ${colors.dim}Item ${idx + 1} [${type}]:${colors.reset} ${Object.keys(item.properties).length} properties`);
        });
        sections.push('');
    }

    // Other formats (compact display)
    const otherFormats = [
        { key: 'rdfa', title: 'RDFa' },
        { key: 'dublinCore', title: 'Dublin Core' },
        { key: 'manifest', title: 'Web App Manifest' },
        { key: 'oembed', title: 'oEmbed' },
        { key: 'relLinks', title: 'rel-* Links' },
    ];

    otherFormats.forEach(({ key, title }) => {
        const data = result[key];
        if (data && Object.keys(data).length > 0) {
            sections.push(`${colors.dim}${title}: ${Object.keys(data).length} entries${colors.reset}`);
        }
    });

    return sections.join('\n');
}

async function main() {
    const args = process.argv.slice(2);

    if (args.length === 0) {
        log(colors.yellow, '\nUsage:');
        log(colors.dim, '  node node-wasm.js <url-or-file>');
        log(colors.dim, '\nExamples:');
        log(colors.dim, '  node node-wasm.js https://example.com');
        log(colors.dim, '  node node-wasm.js local-file.html\n');
        process.exit(1);
    }

    const input = args[0];
    const isUrl = input.startsWith('http://') || input.startsWith('https://');

    log(colors.bright, `\n🚀 MetaOxide WASM - Node.js Example\n`);
    log(colors.dim, `Source: ${input}`);

    try {
        // Initialize WASM module
        log(colors.dim, 'Initializing WASM module...');
        await initialize();

        // Fetch or read HTML
        log(colors.dim, isUrl ? 'Fetching HTML...' : 'Reading file...');
        const html = isUrl ? await fetchHtml(input) : await readLocalFile(input);
        const baseUrl = isUrl ? input : undefined;

        // Extract metadata
        log(colors.dim, 'Extracting metadata...');
        const startTime = performance.now();
        const result = await extractAll(html, { baseUrl });
        const duration = (performance.now() - startTime).toFixed(2);

        // Display results
        console.log(formatMetadata(result));

        log(colors.green, `\n✅ Extraction completed in ${duration}ms\n`);

        // Optional: Write full JSON output
        if (args.includes('--json')) {
            const fs = require('fs');
            const outputFile = 'metadata.json';
            fs.writeFileSync(outputFile, JSON.stringify(result, null, 2));
            log(colors.dim, `Full JSON written to ${outputFile}`);
        }

    } catch (error) {
        log(colors.yellow, `\n❌ Error: ${error.message}\n`);
        process.exit(1);
    }
}

// Run if executed directly
if (require.main === module) {
    main();
}

module.exports = { main };
