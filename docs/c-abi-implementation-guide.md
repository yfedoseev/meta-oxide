# MetaOxide C-ABI Implementation Guide

## Overview

This document provides detailed technical specifications for implementing the C-ABI layer (`libmeta_oxide`), which serves as the foundation for all language bindings.

## Table of Contents

1. [Architecture](#architecture)
2. [C Header Specification](#c-header-specification)
3. [Rust Implementation](#rust-implementation)
4. [Memory Management](#memory-management)
5. [Build Configuration](#build-configuration)
6. [Testing Strategy](#testing-strategy)
7. [Platform Considerations](#platform-considerations)

---

## Architecture

### Module Structure

```
src/
  ffi/
    mod.rs           # Module exports
    types.rs         # C-compatible type definitions
    strings.rs       # String handling utilities
    meta.rs          # MetaTags FFI
    opengraph.rs     # OpenGraph FFI
    twitter.rs       # TwitterCard FFI
    jsonld.rs        # JSON-LD FFI
    microdata.rs     # Microdata FFI
    rdfa.rs          # RDFa FFI
    microformats.rs  # Microformats FFI
    oembed.rs        # oEmbed FFI
    dublin_core.rs   # Dublin Core FFI
    manifest.rs      # Web App Manifest FFI
    rel_links.rs     # rel-* links FFI
    result.rs        # Aggregate result type
    error.rs         # Error handling
    exports.rs       # C function exports
```

### Design Principles

1. **Opaque Pointers**: Internal structures hidden from C callers
2. **Error-First API**: Functions return error codes, results via out-params
3. **Explicit Memory**: Clear ownership, explicit free functions
4. **Thread Safety**: Stateless design, no global mutable state
5. **ABI Stability**: Versioned API, backward compatibility commitment

---

## C Header Specification

### meta_oxide.h

```c
#ifndef META_OXIDE_H
#define META_OXIDE_H

#include <stddef.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/* Version information */
#define META_OXIDE_VERSION_MAJOR 0
#define META_OXIDE_VERSION_MINOR 2
#define META_OXIDE_VERSION_PATCH 0

/* Error codes */
typedef enum {
    META_OXIDE_OK = 0,
    META_OXIDE_ERR_NULL_INPUT = 1,
    META_OXIDE_ERR_INVALID_HTML = 2,
    META_OXIDE_ERR_INVALID_URL = 3,
    META_OXIDE_ERR_ALLOCATION = 4,
    META_OXIDE_ERR_EXTRACTION = 5,
    META_OXIDE_ERR_INTERNAL = 99
} meta_oxide_error_code_t;

/* Opaque handle types */
typedef struct meta_oxide_string_s meta_oxide_string_t;
typedef struct meta_oxide_string_array_s meta_oxide_string_array_t;
typedef struct meta_oxide_meta_s meta_oxide_meta_t;
typedef struct meta_oxide_opengraph_s meta_oxide_opengraph_t;
typedef struct meta_oxide_twitter_s meta_oxide_twitter_t;
typedef struct meta_oxide_jsonld_s meta_oxide_jsonld_t;
typedef struct meta_oxide_jsonld_array_s meta_oxide_jsonld_array_t;
typedef struct meta_oxide_microdata_s meta_oxide_microdata_t;
typedef struct meta_oxide_microdata_array_s meta_oxide_microdata_array_t;
typedef struct meta_oxide_rdfa_s meta_oxide_rdfa_t;
typedef struct meta_oxide_rdfa_array_s meta_oxide_rdfa_array_t;
typedef struct meta_oxide_microformats_s meta_oxide_microformats_t;
typedef struct meta_oxide_oembed_s meta_oxide_oembed_t;
typedef struct meta_oxide_dublin_core_s meta_oxide_dublin_core_t;
typedef struct meta_oxide_manifest_s meta_oxide_manifest_t;
typedef struct meta_oxide_rel_links_s meta_oxide_rel_links_t;
typedef struct meta_oxide_result_s meta_oxide_result_t;
typedef struct meta_oxide_error_s meta_oxide_error_t;

/* ============================================================================
 * Version and Information
 * ============================================================================ */

/**
 * Get the library version string (e.g., "0.2.0")
 *
 * @return Static string, do not free
 */
const char* meta_oxide_version(void);

/**
 * Get the number of supported formats
 *
 * @return Number of supported metadata formats
 */
size_t meta_oxide_format_count(void);

/**
 * Get the name of a supported format by index
 *
 * @param index Format index (0 to format_count - 1)
 * @return Format name or NULL if index out of range
 */
const char* meta_oxide_format_name(size_t index);

/* ============================================================================
 * String Accessors
 * ============================================================================ */

/**
 * Get the C string pointer from a meta_oxide_string
 *
 * @param str String handle
 * @return Null-terminated string or NULL
 */
const char* meta_oxide_string_get(const meta_oxide_string_t* str);

/**
 * Get the length of a meta_oxide_string
 *
 * @param str String handle
 * @return String length (not including null terminator)
 */
size_t meta_oxide_string_len(const meta_oxide_string_t* str);

/**
 * Check if a meta_oxide_string is null or empty
 *
 * @param str String handle
 * @return 1 if null or empty, 0 otherwise
 */
int meta_oxide_string_is_empty(const meta_oxide_string_t* str);

/* ============================================================================
 * String Array Accessors
 * ============================================================================ */

/**
 * Get the length of a string array
 *
 * @param arr Array handle
 * @return Number of elements
 */
size_t meta_oxide_string_array_len(const meta_oxide_string_array_t* arr);

/**
 * Get a string from the array by index
 *
 * @param arr Array handle
 * @param index Element index
 * @return String handle or NULL if out of range
 */
const meta_oxide_string_t* meta_oxide_string_array_get(
    const meta_oxide_string_array_t* arr,
    size_t index
);

/* ============================================================================
 * Core Extraction Functions
 * ============================================================================ */

/**
 * Extract all metadata from HTML
 *
 * @param html HTML content (UTF-8)
 * @param html_len Length of HTML content
 * @param base_url Base URL for resolving relative URLs (may be NULL)
 * @param out_result Output: result handle (caller must free with meta_oxide_free_result)
 * @param out_error Output: error handle if return != 0 (caller must free)
 * @return META_OXIDE_OK on success, error code otherwise
 */
meta_oxide_error_code_t meta_oxide_extract_all(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_result_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract standard HTML meta tags
 */
meta_oxide_error_code_t meta_oxide_extract_meta(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_meta_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract Open Graph metadata
 */
meta_oxide_error_code_t meta_oxide_extract_opengraph(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_opengraph_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract Twitter Card metadata
 */
meta_oxide_error_code_t meta_oxide_extract_twitter(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_twitter_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract JSON-LD structured data
 */
meta_oxide_error_code_t meta_oxide_extract_jsonld(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_jsonld_array_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract Microdata
 */
meta_oxide_error_code_t meta_oxide_extract_microdata(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_microdata_array_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract RDFa
 */
meta_oxide_error_code_t meta_oxide_extract_rdfa(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_rdfa_array_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract Microformats
 */
meta_oxide_error_code_t meta_oxide_extract_microformats(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_microformats_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract oEmbed endpoints
 */
meta_oxide_error_code_t meta_oxide_extract_oembed(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_oembed_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract Dublin Core metadata
 */
meta_oxide_error_code_t meta_oxide_extract_dublin_core(
    const char* html,
    size_t html_len,
    meta_oxide_dublin_core_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract Web App Manifest link
 */
meta_oxide_error_code_t meta_oxide_extract_manifest(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_manifest_t** out_result,
    meta_oxide_error_t** out_error
);

/**
 * Extract rel-* link relationships
 */
meta_oxide_error_code_t meta_oxide_extract_rel_links(
    const char* html,
    size_t html_len,
    const char* base_url,
    meta_oxide_rel_links_t** out_result,
    meta_oxide_error_t** out_error
);

/* ============================================================================
 * Result Accessors
 * ============================================================================ */

/* Meta accessors */
const meta_oxide_string_t* meta_oxide_meta_title(const meta_oxide_meta_t* meta);
const meta_oxide_string_t* meta_oxide_meta_description(const meta_oxide_meta_t* meta);
const meta_oxide_string_t* meta_oxide_meta_canonical(const meta_oxide_meta_t* meta);
const meta_oxide_string_array_t* meta_oxide_meta_keywords(const meta_oxide_meta_t* meta);
/* ... additional accessors for all fields */

/* OpenGraph accessors */
const meta_oxide_string_t* meta_oxide_opengraph_title(const meta_oxide_opengraph_t* og);
const meta_oxide_string_t* meta_oxide_opengraph_type(const meta_oxide_opengraph_t* og);
const meta_oxide_string_t* meta_oxide_opengraph_url(const meta_oxide_opengraph_t* og);
const meta_oxide_string_t* meta_oxide_opengraph_image(const meta_oxide_opengraph_t* og);
const meta_oxide_string_t* meta_oxide_opengraph_description(const meta_oxide_opengraph_t* og);
/* ... additional accessors */

/* Twitter accessors */
const meta_oxide_string_t* meta_oxide_twitter_card(const meta_oxide_twitter_t* tw);
const meta_oxide_string_t* meta_oxide_twitter_title(const meta_oxide_twitter_t* tw);
const meta_oxide_string_t* meta_oxide_twitter_description(const meta_oxide_twitter_t* tw);
const meta_oxide_string_t* meta_oxide_twitter_image(const meta_oxide_twitter_t* tw);
/* ... additional accessors */

/* JSON-LD array accessors */
size_t meta_oxide_jsonld_array_len(const meta_oxide_jsonld_array_t* arr);
const meta_oxide_jsonld_t* meta_oxide_jsonld_array_get(
    const meta_oxide_jsonld_array_t* arr,
    size_t index
);
const meta_oxide_string_t* meta_oxide_jsonld_type(const meta_oxide_jsonld_t* obj);
const meta_oxide_string_t* meta_oxide_jsonld_json(const meta_oxide_jsonld_t* obj);
/* ... additional accessors */

/* Aggregate result accessors */
const meta_oxide_meta_t* meta_oxide_result_meta(const meta_oxide_result_t* result);
const meta_oxide_opengraph_t* meta_oxide_result_opengraph(const meta_oxide_result_t* result);
const meta_oxide_twitter_t* meta_oxide_result_twitter(const meta_oxide_result_t* result);
const meta_oxide_jsonld_array_t* meta_oxide_result_jsonld(const meta_oxide_result_t* result);
const meta_oxide_microdata_array_t* meta_oxide_result_microdata(const meta_oxide_result_t* result);
const meta_oxide_rdfa_array_t* meta_oxide_result_rdfa(const meta_oxide_result_t* result);
const meta_oxide_microformats_t* meta_oxide_result_microformats(const meta_oxide_result_t* result);
const meta_oxide_oembed_t* meta_oxide_result_oembed(const meta_oxide_result_t* result);
const meta_oxide_dublin_core_t* meta_oxide_result_dublin_core(const meta_oxide_result_t* result);
const meta_oxide_manifest_t* meta_oxide_result_manifest(const meta_oxide_result_t* result);
const meta_oxide_rel_links_t* meta_oxide_result_rel_links(const meta_oxide_result_t* result);

/* Error accessors */
meta_oxide_error_code_t meta_oxide_error_code(const meta_oxide_error_t* err);
const char* meta_oxide_error_message(const meta_oxide_error_t* err);

/* ============================================================================
 * Memory Management
 * ============================================================================ */

void meta_oxide_free_result(meta_oxide_result_t* result);
void meta_oxide_free_meta(meta_oxide_meta_t* meta);
void meta_oxide_free_opengraph(meta_oxide_opengraph_t* og);
void meta_oxide_free_twitter(meta_oxide_twitter_t* tw);
void meta_oxide_free_jsonld_array(meta_oxide_jsonld_array_t* arr);
void meta_oxide_free_microdata_array(meta_oxide_microdata_array_t* arr);
void meta_oxide_free_rdfa_array(meta_oxide_rdfa_array_t* arr);
void meta_oxide_free_microformats(meta_oxide_microformats_t* mf);
void meta_oxide_free_oembed(meta_oxide_oembed_t* oe);
void meta_oxide_free_dublin_core(meta_oxide_dublin_core_t* dc);
void meta_oxide_free_manifest(meta_oxide_manifest_t* m);
void meta_oxide_free_rel_links(meta_oxide_rel_links_t* rl);
void meta_oxide_free_error(meta_oxide_error_t* err);

#ifdef __cplusplus
}
#endif

#endif /* META_OXIDE_H */
```

---

## Rust Implementation

### Core Types (src/ffi/types.rs)

```rust
//! C-compatible type definitions

use std::ffi::{c_char, CString};
use std::ptr;

/// C-compatible string type
///
/// # Safety
/// - `ptr` is a valid, null-terminated, UTF-8 string
/// - Memory is owned by this struct and freed on drop
#[repr(C)]
pub struct MetaOxideString {
    ptr: *mut c_char,
    len: usize,
}

impl MetaOxideString {
    /// Create from Rust String
    pub fn from_string(s: String) -> Self {
        let len = s.len();
        match CString::new(s) {
            Ok(cstr) => Self {
                ptr: cstr.into_raw(),
                len,
            },
            Err(_) => Self::null(),
        }
    }

    /// Create from Option<String>
    pub fn from_option(opt: Option<String>) -> Self {
        opt.map(Self::from_string).unwrap_or_else(Self::null)
    }

    /// Create null string
    pub fn null() -> Self {
        Self {
            ptr: ptr::null_mut(),
            len: 0,
        }
    }

    /// Check if null or empty
    pub fn is_empty(&self) -> bool {
        self.ptr.is_null() || self.len == 0
    }

    /// Get as C string pointer
    pub fn as_ptr(&self) -> *const c_char {
        self.ptr
    }

    /// Get length
    pub fn len(&self) -> usize {
        self.len
    }
}

impl Drop for MetaOxideString {
    fn drop(&mut self) {
        if !self.ptr.is_null() {
            unsafe {
                let _ = CString::from_raw(self.ptr);
            }
        }
    }
}

/// C-compatible string array
#[repr(C)]
pub struct MetaOxideStringArray {
    ptr: *mut MetaOxideString,
    len: usize,
    capacity: usize,
}

impl MetaOxideStringArray {
    pub fn from_vec(v: Vec<String>) -> Self {
        let strings: Vec<MetaOxideString> = v.into_iter()
            .map(MetaOxideString::from_string)
            .collect();

        let len = strings.len();
        let capacity = strings.capacity();
        let ptr = Box::into_raw(strings.into_boxed_slice()) as *mut MetaOxideString;

        Self { ptr, len, capacity }
    }

    pub fn empty() -> Self {
        Self {
            ptr: ptr::null_mut(),
            len: 0,
            capacity: 0,
        }
    }

    pub fn len(&self) -> usize {
        self.len
    }

    pub fn get(&self, index: usize) -> Option<&MetaOxideString> {
        if index < self.len && !self.ptr.is_null() {
            unsafe { Some(&*self.ptr.add(index)) }
        } else {
            None
        }
    }
}

impl Drop for MetaOxideStringArray {
    fn drop(&mut self) {
        if !self.ptr.is_null() {
            unsafe {
                let slice = std::slice::from_raw_parts_mut(self.ptr, self.len);
                for s in slice {
                    ptr::drop_in_place(s);
                }
                let _ = Box::from_raw(std::slice::from_raw_parts_mut(self.ptr, self.capacity));
            }
        }
    }
}

/// Error codes matching C enum
#[repr(C)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum MetaOxideErrorCode {
    Ok = 0,
    NullInput = 1,
    InvalidHtml = 2,
    InvalidUrl = 3,
    Allocation = 4,
    Extraction = 5,
    Internal = 99,
}

/// C-compatible error type
#[repr(C)]
pub struct MetaOxideError {
    code: MetaOxideErrorCode,
    message: MetaOxideString,
}

impl MetaOxideError {
    pub fn new(code: MetaOxideErrorCode, message: String) -> Self {
        Self {
            code,
            message: MetaOxideString::from_string(message),
        }
    }

    pub fn from_microformat_error(err: crate::MicroformatError) -> Self {
        use crate::MicroformatError::*;
        match err {
            ParseError(msg) => Self::new(MetaOxideErrorCode::InvalidHtml, msg),
            InvalidUrl(_) => Self::new(MetaOxideErrorCode::InvalidUrl, err.to_string()),
            MissingProperty(msg) => Self::new(MetaOxideErrorCode::Extraction, msg),
            InvalidStructure(msg) => Self::new(MetaOxideErrorCode::Extraction, msg),
            ExtractionFailed(msg) => Self::new(MetaOxideErrorCode::Extraction, msg),
        }
    }

    pub fn code(&self) -> MetaOxideErrorCode {
        self.code
    }

    pub fn message(&self) -> &MetaOxideString {
        &self.message
    }
}
```

### Meta FFI Type (src/ffi/meta.rs)

```rust
//! FFI wrapper for MetaTags

use super::types::{MetaOxideString, MetaOxideStringArray};
use crate::types::meta::MetaTags;

/// C-compatible MetaTags
#[repr(C)]
pub struct MetaOxideMeta {
    pub title: MetaOxideString,
    pub description: MetaOxideString,
    pub canonical: MetaOxideString,
    pub keywords: MetaOxideStringArray,
    pub author: MetaOxideString,
    pub viewport: MetaOxideString,
    pub charset: MetaOxideString,
    pub language: MetaOxideString,
    pub robots: MetaOxideString,
    pub generator: MetaOxideString,
    pub theme_color: MetaOxideString,
    // Verification tags
    pub google_site_verification: MetaOxideString,
    pub msvalidate: MetaOxideString,
    pub yandex_verification: MetaOxideString,
    pub pinterest_verification: MetaOxideString,
    pub facebook_domain_verification: MetaOxideString,
    // PWA
    pub apple_touch_icon: MetaOxideString,
    pub manifest: MetaOxideString,
}

impl MetaOxideMeta {
    pub fn from_meta_tags(meta: MetaTags) -> Self {
        Self {
            title: MetaOxideString::from_option(meta.title),
            description: MetaOxideString::from_option(meta.description),
            canonical: MetaOxideString::from_option(meta.canonical),
            keywords: meta.keywords.map(MetaOxideStringArray::from_vec)
                .unwrap_or_else(MetaOxideStringArray::empty),
            author: MetaOxideString::from_option(meta.author),
            viewport: MetaOxideString::from_option(meta.viewport),
            charset: MetaOxideString::from_option(meta.charset),
            language: MetaOxideString::from_option(meta.language),
            robots: MetaOxideString::from_option(meta.robots),
            generator: MetaOxideString::from_option(meta.generator),
            theme_color: MetaOxideString::from_option(meta.theme_color),
            google_site_verification: MetaOxideString::from_option(meta.google_site_verification),
            msvalidate: MetaOxideString::from_option(meta.msvalidate),
            yandex_verification: MetaOxideString::from_option(meta.yandex_verification),
            pinterest_verification: MetaOxideString::from_option(meta.pinterest_verification),
            facebook_domain_verification: MetaOxideString::from_option(meta.facebook_domain_verification),
            apple_touch_icon: MetaOxideString::from_option(meta.apple_touch_icon),
            manifest: MetaOxideString::from_option(meta.manifest),
        }
    }
}
```

### Export Functions (src/ffi/exports.rs)

```rust
//! C function exports

use std::ffi::{c_char, CStr};
use std::ptr;
use std::slice;

use super::types::*;
use super::meta::MetaOxideMeta;
use crate::extractors;

/// Version string
static VERSION: &str = concat!(env!("CARGO_PKG_VERSION"), "\0");

/// Get library version
#[no_mangle]
pub extern "C" fn meta_oxide_version() -> *const c_char {
    VERSION.as_ptr() as *const c_char
}

/// Extract all metadata
///
/// # Safety
/// - `html` must be valid UTF-8
/// - `out_result` and `out_error` must be valid pointers
#[no_mangle]
pub unsafe extern "C" fn meta_oxide_extract_all(
    html: *const c_char,
    html_len: usize,
    base_url: *const c_char,
    out_result: *mut *mut MetaOxideResult,
    out_error: *mut *mut MetaOxideError,
) -> MetaOxideErrorCode {
    // Validate inputs
    if html.is_null() || out_result.is_null() {
        if !out_error.is_null() {
            *out_error = Box::into_raw(Box::new(
                MetaOxideError::new(MetaOxideErrorCode::NullInput, "Null input pointer".to_string())
            ));
        }
        return MetaOxideErrorCode::NullInput;
    }

    // Convert HTML to Rust str
    let html_slice = slice::from_raw_parts(html as *const u8, html_len);
    let html_str = match std::str::from_utf8(html_slice) {
        Ok(s) => s,
        Err(e) => {
            if !out_error.is_null() {
                *out_error = Box::into_raw(Box::new(
                    MetaOxideError::new(MetaOxideErrorCode::InvalidHtml, e.to_string())
                ));
            }
            return MetaOxideErrorCode::InvalidHtml;
        }
    };

    // Convert base_url
    let base_url_opt = if base_url.is_null() {
        None
    } else {
        match CStr::from_ptr(base_url).to_str() {
            Ok(s) => Some(s),
            Err(e) => {
                if !out_error.is_null() {
                    *out_error = Box::into_raw(Box::new(
                        MetaOxideError::new(MetaOxideErrorCode::InvalidUrl, e.to_string())
                    ));
                }
                return MetaOxideErrorCode::InvalidUrl;
            }
        }
    };

    // Perform extraction
    let result = extract_all_internal(html_str, base_url_opt);

    match result {
        Ok(r) => {
            *out_result = Box::into_raw(Box::new(r));
            MetaOxideErrorCode::Ok
        }
        Err(e) => {
            if !out_error.is_null() {
                *out_error = Box::into_raw(Box::new(e));
            }
            e.code()
        }
    }
}

fn extract_all_internal(
    html: &str,
    base_url: Option<&str>
) -> Result<MetaOxideResult, MetaOxideError> {
    let mut result = MetaOxideResult::default();

    // Extract meta
    if let Ok(meta) = extractors::meta::extract(html, base_url) {
        result.meta = Some(Box::new(MetaOxideMeta::from_meta_tags(meta)));
    }

    // Extract opengraph
    if let Ok(og) = extractors::social::extract_opengraph(html, base_url) {
        result.opengraph = Some(Box::new(MetaOxideOpenGraph::from_og(og)));
    }

    // ... similar for other formats

    Ok(result)
}

/// Free extraction result
///
/// # Safety
/// - `result` must have been allocated by `meta_oxide_extract_*`
#[no_mangle]
pub unsafe extern "C" fn meta_oxide_free_result(result: *mut MetaOxideResult) {
    if !result.is_null() {
        let _ = Box::from_raw(result);
    }
}

/// Free meta result
#[no_mangle]
pub unsafe extern "C" fn meta_oxide_free_meta(meta: *mut MetaOxideMeta) {
    if !meta.is_null() {
        let _ = Box::from_raw(meta);
    }
}

/// Free error
#[no_mangle]
pub unsafe extern "C" fn meta_oxide_free_error(error: *mut MetaOxideError) {
    if !error.is_null() {
        let _ = Box::from_raw(error);
    }
}

// Accessor functions

#[no_mangle]
pub extern "C" fn meta_oxide_string_get(str: *const MetaOxideString) -> *const c_char {
    if str.is_null() {
        ptr::null()
    } else {
        unsafe { (*str).as_ptr() }
    }
}

#[no_mangle]
pub extern "C" fn meta_oxide_string_len(str: *const MetaOxideString) -> usize {
    if str.is_null() {
        0
    } else {
        unsafe { (*str).len() }
    }
}

#[no_mangle]
pub extern "C" fn meta_oxide_string_is_empty(str: *const MetaOxideString) -> i32 {
    if str.is_null() {
        1
    } else {
        unsafe { if (*str).is_empty() { 1 } else { 0 } }
    }
}

#[no_mangle]
pub extern "C" fn meta_oxide_meta_title(meta: *const MetaOxideMeta) -> *const MetaOxideString {
    if meta.is_null() {
        ptr::null()
    } else {
        unsafe { &(*meta).title }
    }
}

// ... additional accessor implementations
```

---

## Memory Management

### Ownership Rules

1. **Extraction functions** allocate memory, caller owns result
2. **Accessor functions** return pointers into parent structure
3. **Free functions** deallocate entire structure hierarchy
4. **String data** is null-terminated, UTF-8 encoded

### Usage Pattern (C)

```c
meta_oxide_result_t* result = NULL;
meta_oxide_error_t* error = NULL;

int code = meta_oxide_extract_all(html, strlen(html), base_url, &result, &error);

if (code != META_OXIDE_OK) {
    printf("Error: %s\n", meta_oxide_error_message(error));
    meta_oxide_free_error(error);
    return;
}

// Use result
const meta_oxide_meta_t* meta = meta_oxide_result_meta(result);
if (meta) {
    const meta_oxide_string_t* title = meta_oxide_meta_title(meta);
    if (!meta_oxide_string_is_empty(title)) {
        printf("Title: %s\n", meta_oxide_string_get(title));
    }
}

// Free entire result (including nested structures)
meta_oxide_free_result(result);
```

### Thread Safety

- All extraction functions are thread-safe (stateless)
- Results can be used from any thread
- Free functions must be called from one thread per object

---

## Build Configuration

### Cargo.toml additions

```toml
[lib]
name = "meta_oxide"
crate-type = ["cdylib", "rlib", "staticlib"]

[features]
default = ["python"]
python = ["pyo3"]
ffi = []  # Enable C-ABI exports

[dependencies]
# ... existing deps

[build-dependencies]
cbindgen = "0.26"
```

### build.rs

```rust
fn main() {
    // Generate C header
    let crate_dir = std::env::var("CARGO_MANIFEST_DIR").unwrap();

    let config = cbindgen::Config::from_file("cbindgen.toml")
        .expect("cbindgen.toml not found");

    cbindgen::Builder::new()
        .with_crate(crate_dir)
        .with_config(config)
        .generate()
        .expect("Unable to generate bindings")
        .write_to_file("include/meta_oxide.h");
}
```

### cbindgen.toml

```toml
language = "C"
header = "/* MetaOxide C-ABI - Auto-generated, do not edit */"
include_guard = "META_OXIDE_H"
tab_width = 4
style = "Both"
cpp_compat = true

[export]
prefix = "meta_oxide_"
include = ["MetaOxide.*"]

[enum]
prefix_with_name = true

[parse]
parse_deps = false
include = ["meta_oxide"]
```

---

## Testing Strategy

### Unit Tests (Rust)

```rust
#[cfg(test)]
mod ffi_tests {
    use super::*;

    #[test]
    fn test_string_null() {
        let s = MetaOxideString::null();
        assert!(s.is_empty());
        assert!(s.as_ptr().is_null());
    }

    #[test]
    fn test_string_from_string() {
        let s = MetaOxideString::from_string("Hello".to_string());
        assert!(!s.is_empty());
        assert_eq!(s.len(), 5);

        unsafe {
            let cstr = CStr::from_ptr(s.as_ptr());
            assert_eq!(cstr.to_str().unwrap(), "Hello");
        }
    }

    #[test]
    fn test_extract_all_null_input() {
        unsafe {
            let mut result: *mut MetaOxideResult = ptr::null_mut();
            let mut error: *mut MetaOxideError = ptr::null_mut();

            let code = meta_oxide_extract_all(
                ptr::null(),
                0,
                ptr::null(),
                &mut result,
                &mut error,
            );

            assert_eq!(code, MetaOxideErrorCode::NullInput);
            assert!(result.is_null());
            assert!(!error.is_null());

            meta_oxide_free_error(error);
        }
    }

    #[test]
    fn test_extract_meta_basic() {
        let html = b"<html><head><title>Test</title></head></html>";

        unsafe {
            let mut result: *mut MetaOxideMeta = ptr::null_mut();
            let mut error: *mut MetaOxideError = ptr::null_mut();

            let code = meta_oxide_extract_meta(
                html.as_ptr() as *const c_char,
                html.len(),
                ptr::null(),
                &mut result,
                &mut error,
            );

            assert_eq!(code, MetaOxideErrorCode::Ok);
            assert!(!result.is_null());

            let title = meta_oxide_meta_title(result);
            assert!(!meta_oxide_string_is_empty(title));

            meta_oxide_free_meta(result);
        }
    }
}
```

### Memory Tests (Valgrind)

```bash
# Run with valgrind
valgrind --leak-check=full --show-leak-kinds=all \
    --track-origins=yes ./target/debug/test_ffi
```

### C Integration Tests

```c
// test/test_ffi.c
#include <assert.h>
#include <string.h>
#include "meta_oxide.h"

void test_version() {
    const char* version = meta_oxide_version();
    assert(version != NULL);
    assert(strlen(version) > 0);
    printf("Version: %s\n", version);
}

void test_extract_meta() {
    const char* html = "<html><head><title>Test Page</title></head></html>";
    meta_oxide_result_t* result = NULL;
    meta_oxide_error_t* error = NULL;

    int code = meta_oxide_extract_all(html, strlen(html), NULL, &result, &error);
    assert(code == META_OXIDE_OK);
    assert(result != NULL);

    const meta_oxide_meta_t* meta = meta_oxide_result_meta(result);
    assert(meta != NULL);

    const meta_oxide_string_t* title = meta_oxide_meta_title(meta);
    assert(!meta_oxide_string_is_empty(title));
    assert(strcmp(meta_oxide_string_get(title), "Test Page") == 0);

    meta_oxide_free_result(result);
}

int main() {
    test_version();
    test_extract_meta();
    printf("All tests passed!\n");
    return 0;
}
```

---

## Platform Considerations

### Windows

- Output: `meta_oxide.dll`
- Use `__declspec(dllexport)` (handled by Rust)
- MSVC and MinGW support

### macOS

- Output: `libmeta_oxide.dylib`
- Universal binary (x86_64 + arm64)
- Code signing for distribution

### Linux

- Output: `libmeta_oxide.so`
- Support glibc 2.17+ (CentOS 7)
- Include soname for versioning

### Build Matrix

| OS | Arch | Toolchain | Output |
|----|------|-----------|--------|
| Linux | x86_64 | GNU | libmeta_oxide.so |
| Linux | aarch64 | GNU | libmeta_oxide.so |
| macOS | x86_64 | Apple | libmeta_oxide.dylib |
| macOS | aarch64 | Apple | libmeta_oxide.dylib |
| Windows | x86_64 | MSVC | meta_oxide.dll |
| Windows | x86_64 | GNU | meta_oxide.dll |

### CI/CD Configuration

```yaml
# .github/workflows/ffi.yml
name: FFI Build

on: [push, pull_request]

jobs:
  build:
    strategy:
      matrix:
        include:
          - os: ubuntu-latest
            target: x86_64-unknown-linux-gnu
          - os: ubuntu-latest
            target: aarch64-unknown-linux-gnu
          - os: macos-latest
            target: x86_64-apple-darwin
          - os: macos-latest
            target: aarch64-apple-darwin
          - os: windows-latest
            target: x86_64-pc-windows-msvc

    runs-on: ${{ matrix.os }}

    steps:
      - uses: actions/checkout@v4

      - name: Install Rust
        uses: dtolnay/rust-action@stable
        with:
          targets: ${{ matrix.target }}

      - name: Build
        run: cargo build --release --target ${{ matrix.target }} --features ffi

      - name: Test
        run: cargo test --release --target ${{ matrix.target }} --features ffi

      - name: Upload artifacts
        uses: actions/upload-artifact@v3
        with:
          name: ${{ matrix.target }}
          path: |
            target/${{ matrix.target }}/release/*.so
            target/${{ matrix.target }}/release/*.dylib
            target/${{ matrix.target }}/release/*.dll
            include/meta_oxide.h
```

---

## Checklist for Implementation

### Phase 0.1: Design (Days 1-3)
- [ ] Finalize C header specification
- [ ] Define all FFI struct layouts
- [ ] Document memory ownership rules
- [ ] Review with team

### Phase 0.2: Core Implementation (Days 4-11)
- [ ] Create src/ffi/ module structure
- [ ] Implement MetaOxideString
- [ ] Implement MetaOxideStringArray
- [ ] Implement all FFI wrapper types
- [ ] Implement conversion functions
- [ ] Implement export functions

### Phase 0.3: Build System (Days 12-15)
- [ ] Configure cdylib output
- [ ] Setup cbindgen
- [ ] Create cross-platform scripts
- [ ] Setup CI/CD

### Phase 0.4: Testing (Days 16-20)
- [ ] Rust unit tests
- [ ] C integration tests
- [ ] Valgrind memory tests
- [ ] Thread safety tests

### Phase 0.5: Documentation (Days 21-23)
- [ ] C API reference
- [ ] Integration guide
- [ ] Example code

### Phase 0.6: Release (Days 24-30)
- [ ] Version tagging
- [ ] Binary releases
- [ ] Changelog
- [ ] Announcement

---

*This document is part of the MetaOxide Multi-Language Strategy. See [multi-language-strategy.md](./multi-language-strategy.md) for the complete plan.*
