//! WebAssembly bindings for MetaOxide
//!
//! This module provides WebAssembly bindings using wasm-bindgen, enabling
//! MetaOxide to run in browsers, Node.js, Deno, and edge computing platforms.
//!
//! # Features
//!
//! - Extract all 13 metadata formats from HTML
//! - Zero-copy parsing for maximum performance
//! - Works in all modern browsers
//! - Compatible with Node.js, Deno, Cloudflare Workers, Vercel Edge
//! - TypeScript definitions included
//!
//! # Example
//!
//! ```javascript
//! import { extractAll } from '@yfedoseev/meta-oxide-wasm';
//!
//! const html = '<html><head><meta name="description" content="Test"></head></html>';
//! const result = extractAll(html, 'https://example.com');
//! console.log(result.meta.description); // "Test"
//! ```

use wasm_bindgen::prelude::*;
use serde::{Deserialize, Serialize};

// Re-export from core library
use meta_oxide::{extractors, parser};

/// Initialize panic hook for better error messages in development
#[wasm_bindgen(start)]
pub fn init() {
    #[cfg(feature = "console_error_panic_hook")]
    console_error_panic_hook::set_once();
}

/// Log a message to the console (for debugging)
#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console)]
    fn log(s: &str);
}

/// Complete extraction result containing all metadata formats
#[wasm_bindgen]
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ExtractionResult {
    /// Standard HTML meta tags
    meta: Option<String>,
    /// Open Graph metadata
    open_graph: Option<String>,
    /// Twitter Card metadata
    twitter: Option<String>,
    /// JSON-LD structured data
    json_ld: Option<String>,
    /// Microdata items
    microdata: Option<String>,
    /// Microformats data
    microformats: Option<String>,
    /// RDFa structured data
    rdfa: Option<String>,
    /// Dublin Core metadata
    dublin_core: Option<String>,
    /// Web App Manifest
    manifest: Option<String>,
    /// oEmbed endpoint
    oembed: Option<String>,
    /// rel-* link relationships
    rel_links: Option<String>,
}

#[wasm_bindgen]
impl ExtractionResult {
    /// Get standard HTML meta tags as JSON string
    #[wasm_bindgen(getter)]
    pub fn meta(&self) -> Option<String> {
        self.meta.clone()
    }

    /// Get Open Graph metadata as JSON string
    #[wasm_bindgen(getter)]
    pub fn open_graph(&self) -> Option<String> {
        self.open_graph.clone()
    }

    /// Get Twitter Card metadata as JSON string
    #[wasm_bindgen(getter)]
    pub fn twitter(&self) -> Option<String> {
        self.twitter.clone()
    }

    /// Get JSON-LD structured data as JSON string
    #[wasm_bindgen(getter)]
    pub fn json_ld(&self) -> Option<String> {
        self.json_ld.clone()
    }

    /// Get Microdata items as JSON string
    #[wasm_bindgen(getter)]
    pub fn microdata(&self) -> Option<String> {
        self.microdata.clone()
    }

    /// Get Microformats data as JSON string
    #[wasm_bindgen(getter)]
    pub fn microformats(&self) -> Option<String> {
        self.microformats.clone()
    }

    /// Get RDFa structured data as JSON string
    #[wasm_bindgen(getter)]
    pub fn rdfa(&self) -> Option<String> {
        self.rdfa.clone()
    }

    /// Get Dublin Core metadata as JSON string
    #[wasm_bindgen(getter)]
    pub fn dublin_core(&self) -> Option<String> {
        self.dublin_core.clone()
    }

    /// Get Web App Manifest as JSON string
    #[wasm_bindgen(getter)]
    pub fn manifest(&self) -> Option<String> {
        self.manifest.clone()
    }

    /// Get oEmbed endpoint as JSON string
    #[wasm_bindgen(getter)]
    pub fn oembed(&self) -> Option<String> {
        self.oembed.clone()
    }

    /// Get rel-* link relationships as JSON string
    #[wasm_bindgen(getter)]
    pub fn rel_links(&self) -> Option<String> {
        self.rel_links.clone()
    }

    /// Get count of metadata formats found
    #[wasm_bindgen(js_name = getFormatCount)]
    pub fn get_format_count(&self) -> usize {
        let mut count = 0;
        if self.meta.is_some() { count += 1; }
        if self.open_graph.is_some() { count += 1; }
        if self.twitter.is_some() { count += 1; }
        if self.json_ld.is_some() { count += 1; }
        if self.microdata.is_some() { count += 1; }
        if self.microformats.is_some() { count += 1; }
        if self.rdfa.is_some() { count += 1; }
        if self.dublin_core.is_some() { count += 1; }
        if self.manifest.is_some() { count += 1; }
        if self.oembed.is_some() { count += 1; }
        if self.rel_links.is_some() { count += 1; }
        count
    }

    /// Get all data as a single JSON string
    #[wasm_bindgen(js_name = toJSON)]
    pub fn to_json(&self) -> Result<String, JsValue> {
        serde_json::to_string(self)
            .map_err(|e| JsValue::from_str(&format!("JSON serialization error: {}", e)))
    }
}

/// Extract ALL metadata from HTML
///
/// # Arguments
/// * `html` - HTML content to parse
/// * `base_url` - Optional base URL for resolving relative URLs
///
/// # Returns
/// ExtractionResult containing all extracted metadata as JSON strings
///
/// # Example
/// ```javascript
/// const result = extractAll(htmlString, 'https://example.com');
/// console.log(result.meta); // JSON string of meta tags
/// ```
#[wasm_bindgen(js_name = extractAll)]
pub fn extract_all(html: &str, base_url: Option<String>) -> Result<ExtractionResult, JsValue> {
    let base = base_url.as_deref();

    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let meta = extractors::meta::extract(&document, base)
        .ok()
        .and_then(|m| serde_json::to_string(&m).ok());

    let open_graph = extractors::open_graph::extract(&document, base)
        .ok()
        .and_then(|og| serde_json::to_string(&og).ok());

    let twitter = extractors::twitter::extract(&document, base)
        .ok()
        .and_then(|tw| serde_json::to_string(&tw).ok());

    let json_ld = extractors::json_ld::extract(&document, base)
        .ok()
        .and_then(|jl| serde_json::to_string(&jl).ok());

    let microdata = extractors::microdata::extract(&document, base)
        .ok()
        .and_then(|md| serde_json::to_string(&md).ok());

    let microformats = extractors::microformats::extract(&document, base)
        .ok()
        .and_then(|mf| serde_json::to_string(&mf).ok());

    let rdfa = extractors::rdfa::extract(&document, base)
        .ok()
        .and_then(|r| serde_json::to_string(&r).ok());

    let dublin_core = extractors::dublin_core::extract(&document, base)
        .ok()
        .and_then(|dc| serde_json::to_string(&dc).ok());

    let manifest = extractors::manifest::extract(&document, base)
        .ok()
        .and_then(|m| serde_json::to_string(&m).ok());

    let oembed = extractors::oembed::extract(&document, base)
        .ok()
        .and_then(|oe| serde_json::to_string(&oe).ok());

    let rel_links = extractors::rel_links::extract(&document, base)
        .ok()
        .and_then(|rl| serde_json::to_string(&rl).ok());

    Ok(ExtractionResult {
        meta,
        open_graph,
        twitter,
        json_ld,
        microdata,
        microformats,
        rdfa,
        dublin_core,
        manifest,
        oembed,
        rel_links,
    })
}

/// Extract standard HTML meta tags
#[wasm_bindgen(js_name = extractMeta)]
pub fn extract_meta(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let meta = extractors::meta::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&meta)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract Open Graph metadata
#[wasm_bindgen(js_name = extractOpenGraph)]
pub fn extract_open_graph(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let og = extractors::open_graph::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&og)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract Twitter Card metadata
#[wasm_bindgen(js_name = extractTwitter)]
pub fn extract_twitter(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let twitter = extractors::twitter::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&twitter)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract JSON-LD structured data
#[wasm_bindgen(js_name = extractJsonLd)]
pub fn extract_json_ld(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let json_ld = extractors::json_ld::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&json_ld)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract Microdata items
#[wasm_bindgen(js_name = extractMicrodata)]
pub fn extract_microdata(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let microdata = extractors::microdata::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&microdata)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract Microformats data (h-card, h-entry, etc.)
#[wasm_bindgen(js_name = extractMicroformats)]
pub fn extract_microformats(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let microformats = extractors::microformats::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&microformats)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract RDFa structured data
#[wasm_bindgen(js_name = extractRDFa)]
pub fn extract_rdfa(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let rdfa = extractors::rdfa::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&rdfa)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract Dublin Core metadata
#[wasm_bindgen(js_name = extractDublinCore)]
pub fn extract_dublin_core(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let dc = extractors::dublin_core::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&dc)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract Web App Manifest discovery
#[wasm_bindgen(js_name = extractManifest)]
pub fn extract_manifest(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let manifest = extractors::manifest::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&manifest)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract oEmbed endpoint discovery
#[wasm_bindgen(js_name = extractOEmbed)]
pub fn extract_oembed(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let oembed = extractors::oembed::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&oembed)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

/// Extract rel-* link relationships
#[wasm_bindgen(js_name = extractRelLinks)]
pub fn extract_rel_links(html: &str, base_url: Option<String>) -> Result<String, JsValue> {
    let document = parser::parse_html(html)
        .map_err(|e| JsValue::from_str(&format!("Parse error: {}", e)))?;

    let rel_links = extractors::rel_links::extract(&document, base_url.as_deref())
        .map_err(|e| JsValue::from_str(&format!("Extraction error: {}", e)))?;

    serde_json::to_string(&rel_links)
        .map_err(|e| JsValue::from_str(&format!("JSON error: {}", e)))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_extract_all() {
        let html = r#"
        <html>
        <head>
            <title>Test</title>
            <meta name="description" content="Test description">
            <meta property="og:title" content="OG Title">
        </head>
        </html>
        "#;

        let result = extract_all(html, Some("https://example.com".to_string())).unwrap();
        assert!(result.meta.is_some());
        assert!(result.open_graph.is_some());
        assert_eq!(result.get_format_count(), 2);
    }
}
