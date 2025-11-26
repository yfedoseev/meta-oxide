## Description

<!-- Provide a clear and concise description of the changes in this PR -->

Fixes #(issue number, if applicable)

## Type of Change

<!-- Delete options that are not relevant -->

- [ ] Bug fix (non-breaking change that fixes an issue)
- [ ] New feature (non-breaking change that adds functionality)
- [ ] Breaking change (fix or feature that causes existing functionality to change)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Test addition or improvement
- [ ] Refactoring
- [ ] Dependencies update
- [ ] CI/CD changes

## Changes Made

<!-- List the main changes in this PR -->

-
-
-

## Related Issues

<!-- Link to related issues, e.g., "Fixes #123" or "Relates to #456" -->

Closes #

## Testing

### Core Library (Rust)

- [ ] I have added/updated tests in `tests/` or `src/*/tests.rs`
- [ ] All tests pass locally: `cargo test --all-features`
- [ ] Clippy warnings addressed: `cargo clippy -- -D warnings`
- [ ] Code formatted: `cargo fmt`

### Language Bindings

<!-- Check all that apply -->

- [ ] **Python**: Tests pass with `pytest`, formatted with `ruff format`, linted with `ruff check`
- [ ] **Go**: Tests pass with `go test ./...`, formatted with `gofmt`
- [ ] **Node.js**: Tests pass with `npm test`, TypeScript compiles without errors
- [ ] **Java**: Tests pass with `mvn test`, formatted with `google-java-format`
- [ ] **C#**: Tests pass with `dotnet test`, formatted with `dotnet format`
- [ ] **WASM**: Tests pass with `npm test`, no TypeScript errors
- [ ] **No bindings affected** (core only)

### Integration Testing

- [ ] Tested across multiple operating systems if applicable (Windows/Linux/macOS)
- [ ] Verified binary compatibility if changes to FFI interface
- [ ] Tested with edge cases and malformed input

## Documentation

- [ ] Updated README.md (if user-facing changes)
- [ ] Updated CHANGELOG.md with entry for this change
- [ ] Added/updated code comments for complex logic
- [ ] Updated relevant docs in `/docs/` directory
- [ ] Updated API reference if applicable
- [ ] Added/updated example code if relevant
- [ ] No documentation needed (internal refactoring only)

## Performance Impact

- [ ] No performance impact
- [ ] Performance improvement (describe below)
- [ ] Potential performance impact (describe below)

**Performance notes** (if applicable):

## Breaking Changes

- [ ] No breaking changes
- [ ] Breaking changes (describe below with migration guide)

**Breaking changes** (if applicable):

## Metadata Formats Affected

<!-- Check all that apply -->

- [ ] HTML Meta Tags
- [ ] Open Graph
- [ ] Twitter Cards
- [ ] JSON-LD
- [ ] Microdata
- [ ] Microformats (h-card, h-entry, h-event, h-review, h-recipe, h-product, h-feed, h-adr, h-geo)
- [ ] RDFa
- [ ] Dublin Core
- [ ] Web App Manifest
- [ ] oEmbed
- [ ] rel-links
- [ ] Images & SEO
- [ ] No format-specific changes

## Checklist

- [ ] My code follows the project's style guidelines
- [ ] I have performed a self-review of my own code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] I have updated the documentation
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix is effective or feature works
- [ ] New and existing tests pass locally with my changes
- [ ] The PR title follows conventional commits format (e.g., `feat:`, `fix:`, `docs:`, `perf:`)
- [ ] Any dependent changes have been merged and published

## Screenshots / Benchmarks

<!-- If applicable, add screenshots, benchmark results, or comparison data -->

## Additional Notes

<!-- Any additional information reviewers should know -->
