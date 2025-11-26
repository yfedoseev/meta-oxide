# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability in MetaOxide, please report it responsibly and **do not disclose it publicly** until a fix has been released.

### How to Report

1. **Do NOT open a public GitHub issue** for security vulnerabilities
2. **Email**: Send your report to `security@meta-oxide.dev`
3. **Include**:
   - Description of the vulnerability
   - Steps to reproduce (if applicable)
   - Affected versions (e.g., 0.1.0, 0.1.x)
   - Potential impact
   - Suggested fix (if you have one)

### Response Timeline

- **Initial response**: Within 24 hours
- **Assessment & confirmation**: Within 48 hours
- **Fix development**: Target 30 days for patch release
- **Coordinated disclosure**: We will work with you on timing and credit

## Supported Versions

| Version | Status | Security Updates |
|---------|--------|------------------|
| 0.1.x | Current | Yes |
| 0.0.x | End of Life | No |

**Recommendation**: Always use the latest version to ensure you have all security fixes.

## Security Best Practices

When using MetaOxide:

1. **Keep Dependencies Updated**
   - Regularly update MetaOxide and its dependencies
   - Subscribe to security advisories

2. **Validate Input**
   - Always validate and sanitize HTML input before extraction
   - Be aware of potential XXE (XML External Entity) attacks in XML-based formats
   - Handle edge cases in RDFa and microdata parsing

3. **Use TLS for Data Transmission**
   - When transmitting extracted metadata over networks, use TLS/SSL
   - Never send extracted data over unencrypted connections

4. **Review Performance Guarantees**
   - MetaOxide extracts data from HTML as-is
   - Malformed HTML may produce unexpected results
   - Always validate extracted data before use in critical systems

5. **Monitor Library Changes**
   - Review release notes for security-related changes
   - Update regularly to get security patches

## Known Limitations

MetaOxide is a **metadata extraction library**, not an HTML validator or sanitizer:

1. **HTML Parsing**: Uses fast parsing algorithms; does not validate HTML correctness
2. **Format Coverage**: Extracts 13 common metadata formats; some edge cases may not be handled
3. **Data Accuracy**: Extracted data is only as valid as the source HTML
4. **XXE Protection**: While not typically an issue for HTML parsing, be cautious with RDFa/XML content
5. **XSLT/Script Injection**: Does NOT execute scripts or stylesheets; treats them as data

## Security Considerations by Language

### Rust Core
- Memory-safe through Rust's ownership system
- No unsafe code in FFI boundary (checked)
- C-ABI interface properly validates string encoding

### Language Bindings
All bindings inherit memory safety from the Rust core:

- **Python**: No buffer overflows possible; uses PyO3 safe bindings
- **Node.js**: N-API provides safe JavaScript-Rust interaction
- **Java**: JNI bindings with proper exception handling
- **C#**: P/Invoke with safe type marshaling
- **Go**: cgo with Go's memory safety
- **WASM**: WebAssembly sandboxing provides isolation

## Dependency Security

MetaOxide minimizes external dependencies:

**Core (Rust)**:
- `scraper` - HTML parsing (actively maintained)
- `serde_json` - JSON parsing (widely trusted)
- Minimal transitive dependencies

**Bindings**:
- Python: PyO3 (widely used, actively maintained)
- Node.js: neon-rs (Node.js recommended native binding framework)
- Java: Maven managed via pom.xml
- C#: .NET Framework (Microsoft maintained)
- Go: Standard library only
- WASM: Standard wasm-bindgen

All dependencies are regularly reviewed and updated.

## Security Incident History

No known security incidents have been reported for MetaOxide v0.1.0.

## Contributors

Please see CONTRIBUTING.md for how to responsibly report and contribute security fixes.

---

**Last Updated**: November 25, 2025

For more information, see:
- [CONTRIBUTING.md](CONTRIBUTING.md) - How to contribute
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) - Community standards
