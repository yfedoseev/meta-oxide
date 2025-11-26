# Contributing to MetaOxide

Thank you for your interest in contributing to MetaOxide! We welcome contributions of all kinds, including bug reports, feature requests, documentation improvements, and code submissions.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Reporting Issues](#reporting-issues)
- [Submitting Changes](#submitting-changes)
- [Development Setup](#development-setup)
- [Testing](#testing)
- [Code Style](#code-style)
- [Documentation](#documentation)

---

## Code of Conduct

Please read and follow our [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) to maintain a welcoming and inclusive community.

---

## Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR_USERNAME/meta_oxide.git
   cd meta_oxide
   ```
3. Add the upstream repository:
   ```bash
   git remote add upstream https://github.com/yfedoseev/meta_oxide.git
   ```
4. Create a feature branch for your work:
   ```bash
   git checkout -b feature/your-feature-name
   ```

---

## Reporting Issues

### Before You Start
- Check if the issue already exists
- Verify you're using the latest version
- Test with the latest development version if possible

### How to Report
1. Use a clear, descriptive title
2. Provide a detailed description of the issue
3. Include steps to reproduce (if applicable)
4. Include actual and expected behavior
5. Provide relevant error messages or logs
6. Specify your environment (OS, Rust version, Python version, etc.)

### Example Issue Template
```
### Environment
- OS: macOS 13.5
- Rust: 1.71.0
- Python: 3.11

### Description
Brief description of the issue

### Steps to Reproduce
1. First step
2. Second step
3. ...

### Expected Behavior
What should happen

### Actual Behavior
What actually happens

### Error Message
Any relevant error messages
```

---

## Submitting Changes

### Pull Request Process

1. **Before starting**: Create an issue or comment on an existing one to discuss your proposed change
2. **Create a branch**: Use descriptive names like `feature/add-new-format` or `fix/issue-123`
3. **Keep commits atomic**: Each commit should represent a single logical change
4. **Write meaningful commit messages**:
   ```
   Add support for custom metadata format extraction

   - Implement new extractor module
   - Add 15 test cases
   - Update documentation

   Fixes #123
   ```
5. **Push to your fork** and create a Pull Request
6. **Link to the issue** your PR addresses
7. **Wait for review**: Maintainers will review your code

### PR Description Template
```
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation improvement
- [ ] Performance improvement

## Related Issue
Fixes #(issue number)

## Changes Made
- Item 1
- Item 2
- Item 3

## Testing Done
Describe the testing performed

## Checklist
- [ ] Tests pass locally
- [ ] Code follows style guidelines
- [ ] Documentation updated
- [ ] No breaking changes
```

---

## Development Setup

### Prerequisites
- Rust 1.70+ (https://rustup.rs/)
- Python 3.8+ (for Python bindings)
- Node.js 18+ (for WASM bindings)
- Go 1.18+ (for Go bindings)
- Java 8+ (for Java bindings)
- .NET SDK 6+ (for C# bindings)

### Clone and Setup

```bash
# Clone the repository
git clone https://github.com/yfedoseev/meta_oxide.git
cd meta_oxide

# Install Rust components
rustup component add rustfmt clippy
rustup toolchain install stable

# Install Python development tools
pip install maturin black ruff mypy pytest pytest-cov

# Optional: Set up pre-commit hooks
pip install pre-commit
pre-commit install

# Verify setup
cargo --version
rustc --version
python --version
```

### Building the Project

```bash
# Build core library (Rust)
cargo build

# Build with Python bindings
cargo build --features python

# Build with C-ABI
cargo build --features c-api

# Build for specific target
cargo build --release
```

---

## Testing

### Running Tests

```bash
# Run all Rust tests
cargo test

# Run Rust tests with output
cargo test -- --nocapture

# Run specific test
cargo test test_name

# Run Python tests
pytest python/tests/

# Run Python tests with coverage
pytest --cov python/tests/

# Run with pre-commit hooks
pre-commit run --all-files
```

### Writing Tests

When adding new functionality:

1. **Add unit tests** in the same file as your code
2. **Add integration tests** in the `tests/` directory
3. **Add language-specific tests** for each binding
4. **Ensure >95% coverage** of new code
5. **Test edge cases** and error conditions

Example Rust test:
```rust
#[test]
fn test_extract_new_format() {
    let html = r#"<html><head>...</head></html>"#;
    let extractor = MetaOxide::new(html, "https://example.com");
    let result = extractor.extract_new_format();

    assert_eq!(result.len(), 3);
    assert_eq!(result[0].name, "expected_name");
}
```

### Test Coverage Requirements

- Core library: >95%
- Each language binding: >90%
- All format extractors: >98%
- Error handling paths: 100%

---

## Code Style

### Rust Code Style

We follow Rust community standards with some custom rules:

```bash
# Format code (enforced via pre-commit)
cargo fmt

# Check for issues (enforced via pre-commit)
cargo clippy -- -D warnings
```

**Key guidelines:**
- Maximum line length: 100 characters
- Use meaningful variable names
- Comments should explain "why", not "what"
- Document public APIs with doc comments
- Use `#[must_use]` for important return values

**Clippy configuration** (`clippy.toml`):
```toml
cognitive-complexity-threshold = 15
```

### Python Code Style

We follow PEP 8 with Black formatter:

```bash
# Format code
black python/

# Check style
ruff check python/

# Type check
mypy python/
```

**Key guidelines:**
- Line length: 100 characters
- Use type hints for all functions
- Docstrings for all public functions
- Follow Black formatting

---

## Documentation

### When to Update Documentation

1. **New features**: Add to README.md and API reference
2. **API changes**: Update language-specific API docs
3. **New format support**: Add to FORMATS.md
4. **New examples**: Add to examples/ directory
5. **Bug fixes**: Update relevant documentation if behavior changed

### Documentation Standards

**README updates:**
- Clear examples showing new functionality
- Installation/usage instructions if needed
- Link to detailed documentation

**API Documentation:**
- Function signature with all parameters
- Return type and possible errors
- Example usage code
- Link to related functions

**Format Documentation:**
- What metadata is extracted
- HTML example showing extraction
- Expected output
- Use cases

**Example Code:**
- Runnable (copy-paste ready)
- Properly commented
- Shows best practices
- Demonstrates error handling

### Building Documentation

```bash
# View README locally
# (Use a markdown viewer or GitHub preview)

# Build rustdoc
cargo doc --open

# Generate Python docs
pydoc python/
```

---

## Adding Support for New Metadata Formats

If you want to add support for a new metadata format:

1. **Research the format**:
   - Read specifications
   - Find real-world examples
   - Understand use cases

2. **Create implementation**:
   - Add module in `src/extractors/[format_name]/`
   - Implement extraction logic
   - Add error handling

3. **Add tests**:
   - Minimum 15 test cases
   - Real-world HTML examples
   - Edge cases and errors
   - Performance tests

4. **Update documentation**:
   - Add format to FORMATS.md
   - Update API references (all 7 languages)
   - Add examples

5. **Update bindings**:
   - Ensure all language bindings expose the new format
   - Test in each language

6. **Submit PR** with all of the above

---

## Adding Support for New Languages

To add bindings for a new programming language:

1. **Plan the binding**:
   - Choose FFI approach (similar to existing bindings)
   - Plan API design (should match other language patterns)
   - Identify package management system

2. **Implement binding**:
   - Create language-specific wrapper
   - Ensure all 13 formats are exposed
   - Handle error cases properly
   - Memory management

3. **Add tests**:
   - Minimum 50 test cases per format
   - Real-world examples
   - Error handling tests

4. **Add documentation**:
   - Getting started guide (100 lines)
   - Complete API reference (400 lines)
   - Real-world example project
   - Performance documentation

5. **Create package configuration**:
   - Package metadata
   - Proper versioning
   - License declaration
   - Build instructions

6. **Submit RFC (Request For Comments)**:
   - Discuss approach in an issue first
   - Get feedback before heavy implementation
   - Ensure alignment with project goals

---

## Performance Considerations

When implementing features:

1. **Benchmark your code**:
   ```bash
   cargo bench
   ```

2. **Minimize allocations**:
   - Reuse buffers where possible
   - Use references over owned values
   - Prefer iterators over collecting

3. **Profile for bottlenecks**:
   ```bash
   cargo install flamegraph
   cargo flamegraph
   ```

4. **Compare before/after**:
   - Document performance impact
   - Ensure no regression for other formats

---

## Release Process

**For maintainers:**

1. Update version numbers in all manifests
2. Update CHANGELOG.md
3. Create annotated git tag
4. Publish to all package registries
5. Create GitHub release with notes

**Version numbering**: Semantic Versioning (MAJOR.MINOR.PATCH)

---

## Questions or Need Help?

- Check existing issues and discussions
- Review documentation in `/docs/`
- Open a discussion issue for questions
- Review the architecture documentation

---

## Licensing

All contributions are licensed under the same dual license as MetaOxide: **MIT OR Apache-2.0**.

By submitting a PR, you agree that your contributions will be licensed under these terms.

---

**Thank you for contributing to MetaOxide!** 🎉

Together, we're building the universal metadata extraction library for the world.
