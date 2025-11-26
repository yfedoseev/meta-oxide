/**
 * MetaOxide WASM Browser Demo
 *
 * Interactive browser-based metadata extraction demo
 */

import { extractAll, initialize } from '../lib/index.js';

// Initialize WASM module on load
let wasmReady = false;

async function initWasm() {
    if (!wasmReady) {
        try {
            await initialize();
            wasmReady = true;
            console.log('✅ WASM module initialized');
        } catch (error) {
            console.error('Failed to initialize WASM:', error);
            showError('Failed to initialize WASM module. Check console for details.');
        }
    }
}

// UI elements
const urlInput = document.getElementById('url-input');
const htmlInput = document.getElementById('html-input');
const fetchBtn = document.getElementById('fetch-btn');
const extractBtn = document.getElementById('extract-btn');
const currentPageBtn = document.getElementById('current-page-btn');
const clearBtn = document.getElementById('clear-btn');
const loadingDiv = document.getElementById('loading');
const errorDiv = document.getElementById('error');
const resultsDiv = document.getElementById('results');
const emptyStateDiv = document.getElementById('empty-state');
const statsDiv = document.getElementById('stats');
const resultSectionsDiv = document.getElementById('result-sections');

// Event listeners
fetchBtn.addEventListener('click', handleFetchAndExtract);
extractBtn.addEventListener('click', handleExtract);
currentPageBtn.addEventListener('click', handleCurrentPage);
clearBtn.addEventListener('click', handleClear);

// Handle fetch and extract from URL
async function handleFetchAndExtract() {
    const url = urlInput.value.trim();
    if (!url) {
        showError('Please enter a valid URL');
        return;
    }

    showLoading();
    hideError();

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const html = await response.text();
        htmlInput.value = html;
        await extractMetadata(html, url);
    } catch (error) {
        showError(`Failed to fetch URL: ${error.message}`);
        hideLoading();
    }
}

// Handle extract from HTML input
async function handleExtract() {
    const html = htmlInput.value.trim();
    if (!html) {
        showError('Please paste HTML content');
        return;
    }

    const baseUrl = urlInput.value.trim() || undefined;
    await extractMetadata(html, baseUrl);
}

// Handle extract from current page
async function handleCurrentPage() {
    showLoading();
    hideError();

    try {
        const html = document.documentElement.outerHTML;
        htmlInput.value = html;
        urlInput.value = window.location.href;
        await extractMetadata(html, window.location.href);
    } catch (error) {
        showError(`Failed to extract from current page: ${error.message}`);
        hideLoading();
    }
}

// Main extraction function
async function extractMetadata(html, baseUrl) {
    showLoading();
    hideError();

    try {
        await initWasm();

        const startTime = performance.now();
        const result = await extractAll(html, { baseUrl });
        const duration = (performance.now() - startTime).toFixed(2);

        displayResults(result, duration);
        hideLoading();
    } catch (error) {
        showError(`Extraction failed: ${error.message}`);
        hideLoading();
    }
}

// Display extraction results
function displayResults(result, duration) {
    emptyStateDiv.style.display = 'none';
    resultsDiv.style.display = 'block';

    // Calculate stats
    const formatCount = Object.values(result).filter(v => v && Object.keys(v).length > 0).length;
    const totalFields = Object.values(result).reduce((sum, format) => {
        if (!format) return sum;
        return sum + Object.keys(format).length;
    }, 0);

    // Display stats
    statsDiv.innerHTML = `
        <div class="stat-card">
            <div class="stat-value">${formatCount}</div>
            <div class="stat-label">Formats Found</div>
        </div>
        <div class="stat-card">
            <div class="stat-value">${totalFields}</div>
            <div class="stat-label">Total Fields</div>
        </div>
        <div class="stat-card">
            <div class="stat-value">${duration}ms</div>
            <div class="stat-label">Processing Time</div>
        </div>
    `;

    // Display each format
    resultSectionsDiv.innerHTML = '';

    const sections = [
        { key: 'meta', title: 'HTML Meta Tags', data: result.meta },
        { key: 'openGraph', title: 'Open Graph', data: result.openGraph },
        { key: 'twitter', title: 'Twitter Card', data: result.twitter },
        { key: 'jsonLd', title: 'JSON-LD', data: result.jsonLd },
        { key: 'microdata', title: 'Microdata', data: result.microdata },
        { key: 'microformats', title: 'Microformats', data: result.microformats },
        { key: 'rdfa', title: 'RDFa', data: result.rdfa },
        { key: 'dublinCore', title: 'Dublin Core', data: result.dublinCore },
        { key: 'manifest', title: 'Web App Manifest', data: result.manifest },
        { key: 'oembed', title: 'oEmbed', data: result.oembed },
        { key: 'relLinks', title: 'rel-* Links', data: result.relLinks },
    ];

    sections.forEach(section => {
        if (section.data && Object.keys(section.data).length > 0) {
            const sectionDiv = document.createElement('div');
            sectionDiv.className = 'result-section';
            sectionDiv.innerHTML = `
                <h3>${section.title}</h3>
                <div class="result-content">
                    <pre>${JSON.stringify(section.data, null, 2)}</pre>
                </div>
            `;
            resultSectionsDiv.appendChild(sectionDiv);
        }
    });

    if (resultSectionsDiv.children.length === 0) {
        resultSectionsDiv.innerHTML = '<div class="empty-state">No metadata found</div>';
    }
}

// Clear all inputs and results
function handleClear() {
    urlInput.value = '';
    htmlInput.value = '';
    resultsDiv.style.display = 'none';
    emptyStateDiv.style.display = 'block';
    hideError();
}

// UI helper functions
function showLoading() {
    loadingDiv.style.display = 'block';
    fetchBtn.disabled = true;
    extractBtn.disabled = true;
}

function hideLoading() {
    loadingDiv.style.display = 'none';
    fetchBtn.disabled = false;
    extractBtn.disabled = false;
}

function showError(message) {
    errorDiv.textContent = message;
    errorDiv.style.display = 'block';
}

function hideError() {
    errorDiv.style.display = 'none';
}

// Initialize on page load
initWasm();

console.log('MetaOxide WASM Browser Demo ready!');
console.log('Try extracting metadata from the current page or any URL');
