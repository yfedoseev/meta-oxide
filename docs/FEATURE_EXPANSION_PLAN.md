# MetaOxide Feature Expansion Plan: RDFa and Web App Manifest

**Document Version**: 1.0.0
**Created**: 2025-11-25
**Author**: Architecture Team
**Status**: Ready for Implementation

---

## Executive Summary

This document provides a comprehensive implementation plan for adding **RDFa (Resource Description Framework in Attributes)** and **Web App Manifest** support to MetaOxide. These features address:

1. **RDFa**: W3C standard with 62% desktop adoption, enabling semantic web markup extraction
2. **Web App Manifest**: Growing PWA standard for extracting application metadata

The implementation follows MetaOxide's established patterns:
- Rust extractors with `Result<T, ParseError>` return types
- PyO3 Python bindings using `to_py_dict()` pattern
- Integration into `extract_all()` for unified extraction
- Comprehensive test coverage (targeting 1.5-2.0 test/code ratio)

**Estimated Total Effort**: 10-14 engineering days
**Test Target**: 150+ new tests across both features

---

## Table of Contents

1. [Current Architecture Analysis](#current-architecture-analysis)
2. [RDFa Implementation Design](#rdfa-implementation-design)
3. [Web App Manifest Implementation Design](#web-app-manifest-implementation-design)
4. [Technical Debt Assessment](#technical-debt-assessment)
5. [File Structure](#file-structure)
6. [Phase-by-Phase Implementation](#phase-by-phase-implementation)
7. [Testing Strategy](#testing-strategy)
8. [Risk Assessment](#risk-assessment)
9. [Detailed Todo List](#detailed-todo-list)

---

## Current Architecture Analysis

### Established Patterns

The codebase follows consistent patterns that new features MUST adhere to:

#### 1. Extractor Module Pattern
```
src/extractors/{format}/
  mod.rs      # Main extraction logic with extract() function
  tests.rs    # Comprehensive test suite
```

#### 2. Type Definition Pattern
```rust
// src/types/{format}.rs
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize, Default)]
pub struct TypeName {
    pub field: Option<String>,
    // ...
}

impl TypeName {
    pub fn to_py_dict(&self, py: Python) -> Py<PyDict> {
        // Convert to Python dict
    }
}
```

#### 3. Extractor Function Signature
```rust
pub fn extract(html: &str, base_url: Option<&str>) -> Result<TypeName>
```

#### 4. Python Binding Pattern
```rust
#[pyfunction]
#[pyo3(signature = (html, base_url=None))]
fn extract_{format}(py: Python, html: &str, base_url: Option<&str>) -> PyResult<Py<PyDict>> {
    let result = extractors::{format}::extract(html, base_url)
        .map_err(|e| PyErr::new::<pyo3::exceptions::PyRuntimeError, _>(e.to_string()))?;
    Ok(result.to_py_dict(py))
}
```

### SOLID Compliance Review

| Principle | Current Status | Notes |
|-----------|---------------|-------|
| **Single Responsibility** | PASS | Each extractor handles one format |
| **Open/Closed** | PASS | New formats added via new modules |
| **Liskov Substitution** | PASS | All extractors follow same interface |
| **Interface Segregation** | PARTIAL | `Extractor` trait in common.rs unused |
| **Dependency Inversion** | PASS | Depends on abstractions (Result, html_utils) |

### DRY Analysis

| Pattern | Reuse Status | Location |
|---------|-------------|----------|
| URL resolution | Centralized | `common::url_utils::resolve_url` |
| HTML parsing | Centralized | `common::html_utils::parse_html` |
| Selector creation | Centralized | `common::html_utils::create_selector` |
| Text extraction | Centralized | `common::html_utils::extract_text` |
| Attribute extraction | Centralized | `common::html_utils::get_attr` |
| Error types | Centralized | `errors::MicroformatError` |

---

## RDFa Implementation Design

### Overview

RDFa (Resource Description Framework in Attributes) is a W3C standard for embedding structured data in HTML using attributes:

- `vocab` - Vocabulary namespace (e.g., "https://schema.org/")
- `typeof` - Type of the resource (equivalent to `@type`)
- `property` - Property name
- `resource` - URI reference
- `about` - Subject URI
- `prefix` - Custom prefix definitions
- `content` - Override content value
- `datatype` - Data type specification

### Architecture Design

```
src/
  types/
    rdfa.rs                 # RDFa type definitions
  extractors/
    rdfa/
      mod.rs               # Main extraction logic
      value_resolver.rs    # Property value resolution
      prefix_handler.rs    # Prefix/CURIE handling
      tests.rs             # Test suite
```

### Type Definitions

```rust
// src/types/rdfa.rs

/// RDFa item representing a resource with properties
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize, Default)]
pub struct RdfaItem {
    /// The resource type(s) from typeof attribute
    #[serde(rename = "type")]
    #[serde(skip_serializing_if = "Option::is_none")]
    pub type_of: Option<Vec<String>>,

    /// The subject URI from about attribute
    #[serde(skip_serializing_if = "Option::is_none")]
    pub about: Option<String>,

    /// The vocabulary namespace
    #[serde(skip_serializing_if = "Option::is_none")]
    pub vocab: Option<String>,

    /// Properties extracted from property attributes
    /// Key: property name, Value: array of property values
    #[serde(flatten)]
    pub properties: HashMap<String, Vec<RdfaValue>>,
}

/// Value of an RDFa property
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(untagged)]
pub enum RdfaValue {
    /// Literal text value
    Literal(String),

    /// URI reference
    Resource(String),

    /// Nested RDFa item
    Item(Box<RdfaItem>),

    /// Typed literal with datatype
    TypedLiteral {
        value: String,
        datatype: String,
    },
}
```

### Extraction Algorithm

1. **Parse HTML document**
2. **Collect prefix definitions** from `prefix` attributes
3. **Find root RDFa elements** (elements with `typeof` or `vocab`)
4. **For each root element**:
   a. Extract `vocab`, `typeof`, `about` attributes
   b. Resolve CURIE prefixes
   c. Traverse descendants for `property` attributes
   d. For each property:
      - Check for nested `typeof` (nested item)
      - Check for `resource`/`href`/`src` (URI value)
      - Check for `content` attribute (override)
      - Fall back to text content
5. **Return list of RdfaItem**

### SOLID Compliance

| Principle | Implementation Strategy |
|-----------|------------------------|
| **SRP** | Separate modules for value resolution and prefix handling |
| **OCP** | Extensible property handlers via match patterns |
| **LSP** | RdfaValue enum provides substitutable value types |
| **ISP** | Clean public API with internal helpers |
| **DIP** | Uses common::html_utils abstractions |

---

## Web App Manifest Implementation Design

### Overview

Web App Manifest is a JSON file providing metadata for Progressive Web Apps:

- Discovered via `<link rel="manifest" href="...">`
- Contains: name, short_name, icons, display, start_url, theme_color, etc.

### Architecture Design

```
src/
  types/
    manifest.rs            # Manifest type definitions
  extractors/
    manifest/
      mod.rs              # Link discovery + JSON parsing
      tests.rs            # Test suite
```

### Type Definitions

```rust
// src/types/manifest.rs

/// Web App Manifest metadata
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize, Default)]
pub struct WebAppManifest {
    /// Application name
    #[serde(skip_serializing_if = "Option::is_none")]
    pub name: Option<String>,

    /// Short name for home screen
    #[serde(skip_serializing_if = "Option::is_none")]
    pub short_name: Option<String>,

    /// Application description
    #[serde(skip_serializing_if = "Option::is_none")]
    pub description: Option<String>,

    /// Start URL when launched
    #[serde(skip_serializing_if = "Option::is_none")]
    pub start_url: Option<String>,

    /// Display mode (fullscreen, standalone, minimal-ui, browser)
    #[serde(skip_serializing_if = "Option::is_none")]
    pub display: Option<String>,

    /// Screen orientation preference
    #[serde(skip_serializing_if = "Option::is_none")]
    pub orientation: Option<String>,

    /// Theme color for browser chrome
    #[serde(skip_serializing_if = "Option::is_none")]
    pub theme_color: Option<String>,

    /// Background color for splash screen
    #[serde(skip_serializing_if = "Option::is_none")]
    pub background_color: Option<String>,

    /// Application scope
    #[serde(skip_serializing_if = "Option::is_none")]
    pub scope: Option<String>,

    /// Language/direction
    #[serde(skip_serializing_if = "Option::is_none")]
    pub lang: Option<String>,

    /// Text direction (ltr, rtl, auto)
    #[serde(skip_serializing_if = "Option::is_none")]
    pub dir: Option<String>,

    /// Application icons
    #[serde(default, skip_serializing_if = "Vec::is_empty")]
    pub icons: Vec<ManifestIcon>,

    /// Related applications
    #[serde(default, skip_serializing_if = "Vec::is_empty")]
    pub related_applications: Vec<RelatedApplication>,

    /// Prefer related applications over web app
    #[serde(skip_serializing_if = "Option::is_none")]
    pub prefer_related_applications: Option<bool>,

    /// Application categories
    #[serde(default, skip_serializing_if = "Vec::is_empty")]
    pub categories: Vec<String>,

    /// Screenshots for app stores
    #[serde(default, skip_serializing_if = "Vec::is_empty")]
    pub screenshots: Vec<ManifestImage>,

    /// Shortcuts for quick actions
    #[serde(default, skip_serializing_if = "Vec::is_empty")]
    pub shortcuts: Vec<ManifestShortcut>,

    /// Unique identifier
    #[serde(skip_serializing_if = "Option::is_none")]
    pub id: Option<String>,
}

/// Manifest icon definition
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct ManifestIcon {
    pub src: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub sizes: Option<String>,
    #[serde(rename = "type", skip_serializing_if = "Option::is_none")]
    pub mime_type: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub purpose: Option<String>,
}

/// Related native application
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct RelatedApplication {
    pub platform: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub url: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub id: Option<String>,
}

/// Generic manifest image (for screenshots)
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct ManifestImage {
    pub src: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub sizes: Option<String>,
    #[serde(rename = "type", skip_serializing_if = "Option::is_none")]
    pub mime_type: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub label: Option<String>,
}

/// Shortcut definition
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct ManifestShortcut {
    pub name: String,
    pub url: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub short_name: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub description: Option<String>,
    #[serde(default, skip_serializing_if = "Vec::is_empty")]
    pub icons: Vec<ManifestIcon>,
}

/// Manifest link discovery result
#[derive(Debug, Clone, PartialEq, Serialize, Deserialize, Default)]
pub struct ManifestDiscovery {
    /// The manifest link URL (resolved)
    #[serde(skip_serializing_if = "Option::is_none")]
    pub href: Option<String>,

    /// Parsed manifest content (if provided)
    #[serde(skip_serializing_if = "Option::is_none")]
    pub manifest: Option<WebAppManifest>,
}
```

### Implementation Approach

The manifest extractor has two modes:

1. **Discovery Only** (from HTML): Extracts `<link rel="manifest">` URL
2. **Full Parse** (with manifest content): Parses manifest JSON

```rust
/// Discover manifest link from HTML
pub fn extract_link(html: &str, base_url: Option<&str>) -> Result<ManifestDiscovery>

/// Parse manifest JSON content
pub fn parse_manifest(json: &str, base_url: Option<&str>) -> Result<WebAppManifest>

/// Combined: discover link from HTML (manifest content provided separately)
pub fn extract(html: &str, base_url: Option<&str>) -> Result<ManifestDiscovery>
```

---

## Technical Debt Assessment

### Existing Technical Debt

| ID | Category | Severity | Description | Resolution Strategy |
|----|----------|----------|-------------|---------------------|
| TD-001 | Architecture | MEDIUM | `Extractor` trait in common.rs is defined but unused | Consider removing or implementing consistently |
| TD-002 | Testing | LOW | Some extractors lack edge case coverage | Add tests as part of new feature work |
| TD-003 | Documentation | LOW | API reference docs incomplete for some types | Update docs with new features |

### New Technical Debt to Avoid

| Risk | Mitigation |
|------|------------|
| Duplicating URL resolution logic | MUST use `common::url_utils::resolve_url` |
| Inconsistent error handling | MUST use `errors::MicroformatError` variants |
| Missing Python bindings | MUST implement `to_py_dict()` for all new types |
| Incomplete test coverage | MUST achieve 1.5x test/code ratio minimum |

---

## File Structure

### New Files to Create

```
src/
  types/
    rdfa.rs                    # NEW: RDFa type definitions
    manifest.rs                # NEW: Web App Manifest types
  extractors/
    rdfa/
      mod.rs                   # NEW: RDFa extraction
      tests.rs                 # NEW: RDFa tests
    manifest/
      mod.rs                   # NEW: Manifest extraction
      tests.rs                 # NEW: Manifest tests
```

### Files to Modify

```
src/
  types/
    mod.rs                     # ADD: pub mod rdfa; pub mod manifest;
  extractors/
    mod.rs                     # ADD: pub mod rdfa; pub mod manifest;
  lib.rs                       # ADD: Python bindings, extract_all() integration
  errors.rs                    # OPTIONAL: Add new error variants if needed
```

---

## Phase-by-Phase Implementation

### Phase 1: RDFa Core Implementation (5-7 days)

#### Phase 1.1: Type Definitions (1 day)

**Tasks:**
1. Create `src/types/rdfa.rs` with `RdfaItem`, `RdfaValue` types
2. Implement `Default`, `Clone`, `PartialEq`, `Serialize`, `Deserialize`
3. Implement `to_py_dict()` for all types
4. Add unit tests for type conversions
5. Register module in `src/types/mod.rs`

**Acceptance Criteria:**
- All types compile without warnings
- `to_py_dict()` produces valid Python dicts
- Unit tests pass for serialization round-trips

#### Phase 1.2: Basic Extraction (2 days)

**Tasks:**
1. Create `src/extractors/rdfa/mod.rs`
2. Implement basic `extract()` function
3. Handle `vocab`, `typeof`, `property` attributes
4. Extract literal property values
5. Register module in `src/extractors/mod.rs`

**Acceptance Criteria:**
- Extracts simple RDFa markup (vocab + typeof + property)
- Handles Schema.org vocabulary correctly
- Returns `Result<Vec<RdfaItem>>`

#### Phase 1.3: Advanced Features (2 days)

**Tasks:**
1. Implement prefix/CURIE handling
2. Implement `resource`, `about` attribute handling
3. Implement `content` attribute override
4. Implement `datatype` support
5. Handle nested RDFa items (typeof within typeof)

**Acceptance Criteria:**
- CURIE prefixes expand correctly
- Nested items extracted properly
- URI values resolved against base URL

#### Phase 1.4: Python Bindings (0.5 days)

**Tasks:**
1. Add `extract_rdfa()` function to `lib.rs`
2. Add to `extract_all()` aggregation
3. Register in PyO3 module

**Acceptance Criteria:**
- `meta_oxide.extract_rdfa(html)` works from Python
- `extract_all()` includes `rdfa` key when present

#### Phase 1.5: Testing (1-1.5 days)

**Tasks:**
1. Create `src/extractors/rdfa/tests.rs`
2. Unit tests for all extraction functions
3. Integration tests with real-world RDFa examples
4. Edge case tests (malformed markup, missing attributes)
5. Python binding tests

**Test Coverage Target:** 80+ tests

---

### Phase 2: Web App Manifest Implementation (3-4 days)

#### Phase 2.1: Type Definitions (0.5 days)

**Tasks:**
1. Create `src/types/manifest.rs`
2. Define `WebAppManifest`, `ManifestIcon`, etc.
3. Implement serde derives with proper renaming
4. Implement `to_py_dict()` for all types
5. Register module in `src/types/mod.rs`

**Acceptance Criteria:**
- All types derive required traits
- JSON deserialization works correctly
- Python conversion produces valid dicts

#### Phase 2.2: Link Discovery (0.5 days)

**Tasks:**
1. Create `src/extractors/manifest/mod.rs`
2. Implement `extract_link()` function
3. Find `<link rel="manifest">` tags
4. Resolve relative URLs
5. Register module in `src/extractors/mod.rs`

**Acceptance Criteria:**
- Discovers manifest links from HTML
- Handles relative URLs correctly
- Returns `ManifestDiscovery` with href

#### Phase 2.3: JSON Parsing (1 day)

**Tasks:**
1. Implement `parse_manifest()` function
2. Handle all standard manifest fields
3. Resolve relative URLs in manifest (icons, start_url, etc.)
4. Handle missing/optional fields gracefully

**Acceptance Criteria:**
- Parses valid manifest.json correctly
- Handles partial manifests
- Resolves relative URLs

#### Phase 2.4: Python Bindings (0.5 days)

**Tasks:**
1. Add `extract_manifest()` for link discovery
2. Add `parse_manifest()` for JSON parsing
3. Add to `extract_all()` aggregation
4. Register in PyO3 module

**Acceptance Criteria:**
- Python API works as expected
- `extract_all()` includes `manifest` key

#### Phase 2.5: Testing (1-1.5 days)

**Tasks:**
1. Create `src/extractors/manifest/tests.rs`
2. Unit tests for link discovery
3. Unit tests for JSON parsing
4. Real-world manifest examples
5. Edge cases and error handling

**Test Coverage Target:** 50+ tests

---

### Phase 3: Integration and Documentation (2-3 days)

#### Phase 3.1: Integration Testing (1 day)

**Tasks:**
1. Integration tests with combined formats
2. Performance benchmarking
3. Memory usage verification
4. Python integration tests

#### Phase 3.2: Documentation (1 day)

**Tasks:**
1. Update README.md with new features
2. Add API reference for new functions
3. Create usage examples
4. Update ROADMAP.md

#### Phase 3.3: Final Review (0.5-1 day)

**Tasks:**
1. Run full test suite
2. Run clippy with all features
3. Review for SOLID compliance
4. Address any remaining TODOs
5. Version bump preparation

---

## Testing Strategy

### Test Categories

| Category | Description | Target Count |
|----------|-------------|--------------|
| **Unit Tests** | Individual function testing | 80+ |
| **Integration Tests** | Combined format extraction | 20+ |
| **Edge Case Tests** | Malformed input, boundaries | 30+ |
| **Real-World Tests** | Actual website examples | 15+ |
| **Python Tests** | Binding verification | 10+ |

### RDFa Test Scenarios

```rust
// Basic extraction
#[test] fn test_rdfa_basic_vocab_typeof_property()
#[test] fn test_rdfa_schema_org_person()
#[test] fn test_rdfa_schema_org_product()

// Prefix handling
#[test] fn test_rdfa_prefix_definition()
#[test] fn test_rdfa_curie_expansion()
#[test] fn test_rdfa_default_vocab()

// Value types
#[test] fn test_rdfa_literal_value()
#[test] fn test_rdfa_resource_value()
#[test] fn test_rdfa_content_override()
#[test] fn test_rdfa_datatype_handling()

// Nested items
#[test] fn test_rdfa_nested_typeof()
#[test] fn test_rdfa_deeply_nested()

// Edge cases
#[test] fn test_rdfa_empty_html()
#[test] fn test_rdfa_no_rdfa_markup()
#[test] fn test_rdfa_malformed_attributes()
#[test] fn test_rdfa_missing_vocab()

// Real-world
#[test] fn test_rdfa_wikipedia_example()
#[test] fn test_rdfa_government_site()
```

### Manifest Test Scenarios

```rust
// Link discovery
#[test] fn test_manifest_link_discovery()
#[test] fn test_manifest_relative_url()
#[test] fn test_manifest_no_link()

// JSON parsing
#[test] fn test_manifest_full_parse()
#[test] fn test_manifest_minimal()
#[test] fn test_manifest_icons_array()
#[test] fn test_manifest_shortcuts()

// URL resolution
#[test] fn test_manifest_resolve_icon_urls()
#[test] fn test_manifest_resolve_start_url()

// Edge cases
#[test] fn test_manifest_invalid_json()
#[test] fn test_manifest_empty_json()
#[test] fn test_manifest_extra_fields()
```

---

## Risk Assessment

### High Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| RDFa spec complexity | High | High | Start with subset, expand iteratively |
| Nested RDFa edge cases | Medium | High | Extensive test coverage |
| Performance with large documents | Medium | Medium | Benchmark and optimize |

### Medium Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Manifest spec evolution | Medium | Medium | Use serde's `#[serde(flatten)]` for unknown fields |
| Python binding edge cases | Low | Medium | Comprehensive Python tests |
| Integration conflicts | Low | Medium | Incremental integration |

### Low Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Breaking existing tests | Low | Low | Run full suite frequently |
| Documentation gaps | Medium | Low | Document as implementing |

---

## Detailed Todo List

### Phase 1: RDFa Implementation

```
## Phase 1.1: RDFa Type Definitions (1 day)
- [ ] (M) Create src/types/rdfa.rs with RdfaItem struct
- [ ] (S) Add RdfaValue enum (Literal, Resource, Item, TypedLiteral)
- [ ] (S) Implement Default trait for RdfaItem
- [ ] (S) Implement Clone, PartialEq, Debug for all types
- [ ] (M) Implement Serialize/Deserialize with serde
- [ ] (M) Implement to_py_dict() for RdfaItem
- [ ] (M) Implement to_py_dict() for RdfaValue
- [ ] (S) Add module to src/types/mod.rs
- [ ] (S) Unit tests for RdfaItem creation
- [ ] (S) Unit tests for RdfaValue variants
- [ ] (M) Unit tests for serde round-trip
- [ ] (M) Unit tests for to_py_dict()

## Phase 1.2: RDFa Basic Extraction (2 days)
- [ ] (M) Create src/extractors/rdfa/mod.rs
- [ ] (M) Implement extract() function signature
- [ ] (M) Implement find_rdfa_roots() helper
- [ ] (L) Implement extract_item() for single root
- [ ] (M) Implement extract_properties() for descendants
- [ ] (M) Implement extract_property_value() helper
- [ ] (S) Handle vocab attribute
- [ ] (S) Handle typeof attribute (single and multiple)
- [ ] (M) Handle property attribute
- [ ] (S) Handle about attribute
- [ ] (S) Add module to src/extractors/mod.rs
- [ ] (M) Tests: basic vocab + typeof + property
- [ ] (M) Tests: Schema.org Person
- [ ] (M) Tests: Schema.org Article
- [ ] (S) Tests: multiple items on page

## Phase 1.3: RDFa Advanced Features (2 days)
- [ ] (L) Implement prefix_handler module
- [ ] (M) Parse prefix attribute syntax
- [ ] (M) Implement CURIE expansion
- [ ] (M) Handle default prefixes (schema:, foaf:, dc:)
- [ ] (M) Implement resource attribute handling
- [ ] (S) Implement content attribute override
- [ ] (M) Implement datatype attribute support
- [ ] (L) Implement nested item extraction
- [ ] (M) Handle property scope correctly
- [ ] (M) URL resolution for resource values
- [ ] (M) Tests: prefix definitions
- [ ] (M) Tests: CURIE expansion
- [ ] (M) Tests: nested typeof
- [ ] (M) Tests: content override
- [ ] (S) Tests: datatype handling

## Phase 1.4: RDFa Python Bindings (0.5 days)
- [ ] (M) Add extract_rdfa() pyfunction to lib.rs
- [ ] (M) Add proper docstring with examples
- [ ] (M) Add to pymodule registration
- [ ] (M) Add to extract_all() aggregation
- [ ] (S) Test: Python import and basic call
- [ ] (S) Test: Python with base_url
- [ ] (S) Test: extract_all() includes rdfa

## Phase 1.5: RDFa Testing (1.5 days)
- [ ] (M) Create src/extractors/rdfa/tests.rs
- [ ] (S) Tests: empty HTML
- [ ] (S) Tests: no RDFa markup
- [ ] (M) Tests: malformed typeof
- [ ] (M) Tests: missing vocab
- [ ] (M) Tests: deeply nested (5+ levels)
- [ ] (M) Tests: multiple vocabs on page
- [ ] (L) Tests: real-world Wikipedia example
- [ ] (L) Tests: real-world government site
- [ ] (M) Tests: Unicode in values
- [ ] (S) Tests: HTML entities
- [ ] (M) Verify test count >= 80
```

### Phase 2: Web App Manifest Implementation

```
## Phase 2.1: Manifest Type Definitions (0.5 days)
- [ ] (M) Create src/types/manifest.rs
- [ ] (M) Define WebAppManifest struct
- [ ] (S) Define ManifestIcon struct
- [ ] (S) Define RelatedApplication struct
- [ ] (S) Define ManifestImage struct
- [ ] (S) Define ManifestShortcut struct
- [ ] (S) Define ManifestDiscovery struct
- [ ] (M) Implement serde derives with proper field renaming
- [ ] (M) Implement to_py_dict() for WebAppManifest
- [ ] (S) Implement to_py_dict() for sub-types
- [ ] (S) Add module to src/types/mod.rs
- [ ] (M) Unit tests for JSON deserialization
- [ ] (M) Unit tests for to_py_dict()

## Phase 2.2: Manifest Link Discovery (0.5 days)
- [ ] (M) Create src/extractors/manifest/mod.rs
- [ ] (M) Implement extract_link() function
- [ ] (S) Find <link rel="manifest"> tags
- [ ] (M) Handle relative URL resolution
- [ ] (S) Return ManifestDiscovery
- [ ] (S) Add module to src/extractors/mod.rs
- [ ] (S) Tests: link discovery basic
- [ ] (S) Tests: relative URL
- [ ] (S) Tests: no manifest link

## Phase 2.3: Manifest JSON Parsing (1 day)
- [ ] (M) Implement parse_manifest() function
- [ ] (M) Parse all standard fields
- [ ] (M) Handle icons array with URL resolution
- [ ] (M) Handle shortcuts array
- [ ] (M) Handle related_applications array
- [ ] (M) Handle screenshots array
- [ ] (S) Handle optional fields gracefully
- [ ] (M) Resolve relative URLs in manifest
- [ ] (M) Tests: full manifest parse
- [ ] (M) Tests: minimal manifest
- [ ] (S) Tests: icons only
- [ ] (S) Tests: shortcuts
- [ ] (S) Tests: invalid JSON

## Phase 2.4: Manifest Python Bindings (0.5 days)
- [ ] (M) Add extract_manifest() pyfunction
- [ ] (M) Add parse_manifest() pyfunction
- [ ] (M) Add to pymodule registration
- [ ] (M) Add to extract_all() aggregation
- [ ] (S) Test: Python extract_manifest()
- [ ] (S) Test: Python parse_manifest()
- [ ] (S) Test: extract_all() includes manifest

## Phase 2.5: Manifest Testing (1 day)
- [ ] (M) Create src/extractors/manifest/tests.rs
- [ ] (S) Tests: empty manifest
- [ ] (S) Tests: extra unknown fields
- [ ] (M) Tests: all icon sizes
- [ ] (M) Tests: related applications
- [ ] (L) Tests: real-world PWA manifest
- [ ] (M) Tests: Twitter PWA manifest
- [ ] (S) Tests: malformed JSON recovery
- [ ] (M) Verify test count >= 50
```

### Phase 3: Integration and Documentation

```
## Phase 3.1: Integration Testing (1 day)
- [ ] (M) Integration test: RDFa + JSON-LD on same page
- [ ] (M) Integration test: Manifest + Open Graph
- [ ] (M) Integration test: All formats combined
- [ ] (L) Performance benchmark with large HTML
- [ ] (M) Memory usage verification
- [ ] (M) Python integration tests

## Phase 3.2: Documentation (1 day)
- [ ] (M) Update README.md with RDFa section
- [ ] (M) Update README.md with Manifest section
- [ ] (M) Add RDFa examples to docs/examples.md
- [ ] (M) Add Manifest examples to docs/examples.md
- [ ] (M) Update docs/api-reference.md
- [ ] (S) Update ROADMAP.md to mark complete
- [ ] (S) Update CHANGELOG.md

## Phase 3.3: Final Review (0.5 days)
- [ ] (S) Run cargo test --all-features
- [ ] (S) Run cargo clippy --all-features
- [ ] (S) Run cargo doc --no-deps
- [ ] (M) Review all new code for SOLID compliance
- [ ] (S) Check for any remaining TODO comments
- [ ] (S) Prepare version bump (0.1.0)
```

---

## Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Test count | 150+ new tests | `cargo test -- --list | grep -c test` |
| Test coverage | 85%+ | Coverage tool |
| Clippy warnings | 0 | `cargo clippy --all-features` |
| Doc coverage | 100% public items | `cargo doc --no-deps` |
| Performance | <10ms for typical page | Benchmarks |

---

## Agent Assignment Guide

### For Staff Rust Engineer

**Primary Tasks:**
- Phase 1.1: Type Definitions
- Phase 1.2-1.3: Core Extraction Logic
- Phase 2.1-2.3: Manifest Implementation
- Phase 3.3: Final Review

**Key Files:**
- `src/types/rdfa.rs`
- `src/types/manifest.rs`
- `src/extractors/rdfa/mod.rs`
- `src/extractors/manifest/mod.rs`

### For Staff Python Engineer

**Primary Tasks:**
- Phase 1.4: RDFa Python Bindings
- Phase 2.4: Manifest Python Bindings
- All Python integration tests

**Key Files:**
- `src/lib.rs` (Python binding functions)
- Python test files

### For Technical Writer

**Primary Tasks:**
- Phase 3.2: Documentation
- README updates
- API reference updates

**Key Files:**
- `README.md`
- `docs/examples.md`
- `docs/api-reference.md`
- `CHANGELOG.md`

---

## Appendix A: RDFa Specification Reference

### Attribute Priority

1. `content` attribute (highest priority)
2. `resource`, `href`, `src` attributes (for URIs)
3. Element text content (lowest priority)

### Common Prefixes

| Prefix | Namespace |
|--------|-----------|
| `schema` | https://schema.org/ |
| `foaf` | http://xmlns.com/foaf/0.1/ |
| `dc` | http://purl.org/dc/terms/ |
| `og` | http://ogp.me/ns# |

### Example RDFa Markup

```html
<div vocab="https://schema.org/" typeof="Person">
  <span property="name">Jane Doe</span>
  <span property="jobTitle">Professor</span>
  <a property="url" href="https://example.com/jane">Homepage</a>
  <div property="address" typeof="PostalAddress">
    <span property="streetAddress">123 Main St</span>
    <span property="addressLocality">Springfield</span>
  </div>
</div>
```

---

## Appendix B: Web App Manifest Reference

### Example Manifest

```json
{
  "name": "My Progressive Web App",
  "short_name": "MyPWA",
  "description": "A sample PWA",
  "start_url": "/",
  "display": "standalone",
  "theme_color": "#3367D6",
  "background_color": "#FFFFFF",
  "icons": [
    {
      "src": "/icons/icon-192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "/icons/icon-512.png",
      "sizes": "512x512",
      "type": "image/png"
    }
  ],
  "shortcuts": [
    {
      "name": "New Item",
      "url": "/new",
      "icons": [{"src": "/icons/new.png", "sizes": "96x96"}]
    }
  ]
}
```

---

**Document End**
