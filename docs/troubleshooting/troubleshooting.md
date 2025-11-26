# MetaOxide Troubleshooting Guide

Common issues and solutions for MetaOxide across all language bindings.

## Table of Contents

- [Installation Issues](#installation-issues)
- [Runtime Errors](#runtime-errors)
- [Performance Issues](#performance-issues)
- [Language-Specific Issues](#language-specific-issues)
- [Platform-Specific Issues](#platform-specific-issues)

## Installation Issues

### Rust: Compilation Fails

**Problem**: `cargo build` fails with compilation errors

**Solutions**:

1. **Update Rust toolchain**:
```bash
rustup update
```

2. **Check Rust version** (requires 1.70+):
```bash
rustc --version
```

3. **Clean and rebuild**:
```bash
cargo clean
cargo build --release
```

4. **Check dependencies**:
```bash
cargo tree
```

### Python: `pip install meta-oxide` Fails

**Problem**: Installation fails with wheel build errors

**Solutions**:

1. **Install build dependencies**:
```bash
# Ubuntu/Debian
sudo apt-get install python3-dev build-essential

# macOS
xcode-select --install

# Windows
# Install Visual Studio Build Tools
```

2. **Use pre-built wheels**:
```bash
pip install --only-binary meta-oxide meta-oxide
```

3. **Update pip**:
```bash
pip install --upgrade pip setuptools wheel
```

4. **Check Python version** (requires 3.7+):
```bash
python --version
```

### Go: CGO Errors

**Problem**: `go get` fails with CGO-related errors

**Solutions**:

1. **Enable CGO**:
```bash
export CGO_ENABLED=1
```

2. **Install C compiler**:
```bash
# Ubuntu/Debian
sudo apt-get install build-essential

# macOS
xcode-select --install

# Windows
# Install MinGW-w64
```

3. **Check CGO is working**:
```bash
go env CGO_ENABLED
```

### Node.js: Native Module Build Fails

**Problem**: `npm install meta-oxide` fails with node-gyp errors

**Solutions**:

1. **Install build tools**:
```bash
# Windows
npm install --global windows-build-tools

# macOS
xcode-select --install

# Linux
sudo apt-get install build-essential
```

2. **Use correct Node version**:
```bash
nvm install 18
nvm use 18
```

3. **Clear npm cache**:
```bash
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

### Java: JNI Library Not Found

**Problem**: `UnsatisfiedLinkError` when running

**Solutions**:

1. **Check library is in classpath**:
```bash
mvn dependency:tree
```

2. **Set library path**:
```bash
java -Djava.library.path=/path/to/lib -jar app.jar
```

3. **Verify Java version** (requires Java 8+):
```bash
java -version
```

### C#: DllNotFoundException

**Problem**: Cannot find native DLL

**Solutions**:

1. **Check NuGet package restored**:
```bash
dotnet restore
```

2. **Copy native library to output**:
```xml
<ItemGroup>
    <Content Include="runtimes\**\*">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
</ItemGroup>
```

3. **Check platform target**:
```bash
dotnet build --runtime win-x64
dotnet build --runtime linux-x64
dotnet build --runtime osx-x64
```

### WebAssembly: Module Not Found

**Problem**: Cannot import WASM module

**Solutions**:

1. **Check bundler configuration**:

**Webpack**:
```javascript
module.exports = {
    experiments: {
        asyncWebAssembly: true
    }
};
```

**Vite**:
```javascript
export default {
    optimizeDeps: {
        exclude: ['meta-oxide-wasm']
    }
};
```

2. **Initialize module**:
```javascript
import init from 'meta-oxide-wasm';
await init();  // Must call before use
```

## Runtime Errors

### ParseError: Invalid HTML

**Problem**: HTML parsing fails

**Solutions**:

1. **Check HTML is valid UTF-8**:
```python
html = html.encode('utf-8', errors='ignore').decode('utf-8')
```

2. **Handle encoding issues**:
```python
from charset_normalizer import from_bytes

result = from_bytes(html_bytes).best()
html = str(result)
```

3. **Try with minimal HTML**:
```python
# Test if parser works with simple HTML
test_html = "<!DOCTYPE html><html><head><title>Test</title></head></html>"
extractor = MetaOxide(test_html, "https://example.com")
```

### UrlError: Invalid Base URL

**Problem**: Base URL parsing fails

**Solutions**:

1. **Ensure URL is absolute**:
```python
from urllib.parse import urlparse

parsed = urlparse(url)
if not parsed.scheme:
    url = f"https://{url}"
```

2. **Validate URL format**:
```python
import validators

if not validators.url(url):
    raise ValueError(f"Invalid URL: {url}")
```

### Memory Leak (Go/C#)

**Problem**: Memory usage grows over time

**Solutions**:

**Go**:
```go
// Always call Free()
extractor, err := metaoxide.NewExtractor(html, url)
if err != nil {
    return err
}
defer extractor.Free()  // Critical!
```

**C#**:
```csharp
// Always use using statement
using (var extractor = new MetaOxideExtractor(html, url))
{
    // Use extractor
}  // Automatically disposed
```

### Segmentation Fault / Access Violation

**Problem**: Crashes with segfault

**Solutions**:

1. **Don't use after free** (Go):
```go
// Bad
extractor.Free()
metadata := extractor.ExtractAll()  // Crash!

// Good
metadata := extractor.ExtractAll()
extractor.Free()
```

2. **Don't share across threads**:
```go
// Bad
go func() {
    extractor.ExtractAll()  // Unsafe!
}()

// Good
go func() {
    localExtractor, _ := metaoxide.NewExtractor(html, url)
    defer localExtractor.Free()
    localExtractor.ExtractAll()
}()
```

## Performance Issues

### Extraction is Slow

**Problem**: Extraction takes longer than expected

**Solutions**:

1. **Extract selectively**:
```python
# Bad: Extracts all 13 formats
metadata = extractor.extract_all()

# Good: Only extract what you need
og = extractor.extract_opengraph()
twitter = extractor.extract_twitter_card()
```

2. **Use caching**:
```python
from functools import lru_cache

@lru_cache(maxsize=1000)
def extract_cached(url):
    # ...
```

3. **Process in parallel**:
```python
from concurrent.futures import ThreadPoolExecutor

with ThreadPoolExecutor(max_workers=10) as executor:
    results = list(executor.map(extract_from_url, urls))
```

### High Memory Usage

**Problem**: Process uses too much memory

**Solutions**:

1. **Process in batches**:
```python
def process_in_batches(urls, batch_size=1000):
    for i in range(0, len(urls), batch_size):
        batch = urls[i:i+batch_size]
        process_batch(batch)
```

2. **Limit concurrency**:
```python
# Don't process all at once
with ThreadPoolExecutor(max_workers=10) as executor:  # Limit to 10
    results = list(executor.map(extract, urls))
```

3. **Free resources promptly**:
```go
for _, url := range urls {
    extractor, _ := metaoxide.NewExtractor(html, url)
    metadata := extractor.ExtractAll()
    extractor.Free()  // Free immediately
    process(metadata)
}
```

### HTTP Requests are Slow

**Problem**: Fetching HTML is the bottleneck

**Solutions**:

1. **Use connection pooling**:
```python
import requests
from requests.adapters import HTTPAdapter

session = requests.Session()
adapter = HTTPAdapter(pool_connections=100, pool_maxsize=100)
session.mount('http://', adapter)
session.mount('https://', adapter)
```

2. **Concurrent requests**:
```python
import asyncio
import aiohttp

async def fetch_all(urls):
    async with aiohttp.ClientSession() as session:
        tasks = [session.get(url) for url in urls]
        return await asyncio.gather(*tasks)
```

## Language-Specific Issues

### Python: ModuleNotFoundError

**Problem**: `ModuleNotFoundError: No module named 'meta_oxide'`

**Solutions**:

1. **Check installation**:
```bash
pip list | grep meta-oxide
```

2. **Install in correct environment**:
```bash
# If using virtualenv
source venv/bin/activate
pip install meta-oxide

# If using poetry
poetry add meta-oxide
```

3. **Check Python path**:
```python
import sys
print(sys.path)
```

### Go: Undefined Reference

**Problem**: Linker errors about undefined symbols

**Solutions**:

1. **Ensure CGO is enabled**:
```bash
export CGO_ENABLED=1
go build
```

2. **Check library path**:
```bash
export LD_LIBRARY_PATH=/path/to/lib:$LD_LIBRARY_PATH
```

### Node.js: Cannot Find Module

**Problem**: `Cannot find module 'meta-oxide'`

**Solutions**:

1. **Reinstall module**:
```bash
npm uninstall meta-oxide
npm install meta-oxide
```

2. **Check node_modules**:
```bash
ls node_modules/meta-oxide
```

3. **Use correct import**:
```javascript
// CommonJS
const { MetaOxide } = require('meta-oxide');

// ES Modules
import { MetaOxide } from 'meta-oxide';
```

### Java: NoClassDefFoundError

**Problem**: Class not found at runtime

**Solutions**:

1. **Check Maven/Gradle**:
```bash
mvn dependency:tree | grep meta-oxide
```

2. **Rebuild project**:
```bash
mvn clean install
```

3. **Check classpath**:
```bash
java -cp target/app.jar:target/lib/* Main
```

### C#: TypeLoadException

**Problem**: Cannot load type

**Solutions**:

1. **Check .NET version**:
```bash
dotnet --version
```

2. **Rebuild**:
```bash
dotnet clean
dotnet build --configuration Release
```

3. **Check assembly binding**:
```xml
<dependentAssembly>
    <assemblyIdentity name="MetaOxide" />
    <bindingRedirect oldVersion="0.0.0.0-0.1.0.0" newVersion="0.1.0.0" />
</dependentAssembly>
```

## Platform-Specific Issues

### Linux: Library Not Found

**Problem**: `error while loading shared libraries`

**Solutions**:

1. **Install dependencies**:
```bash
sudo apt-get install build-essential
```

2. **Update library cache**:
```bash
sudo ldconfig
```

3. **Set LD_LIBRARY_PATH**:
```bash
export LD_LIBRARY_PATH=/usr/local/lib:$LD_LIBRARY_PATH
```

### macOS: Library Not Loaded

**Problem**: `dyld: Library not loaded`

**Solutions**:

1. **Install Xcode Command Line Tools**:
```bash
xcode-select --install
```

2. **Set DYLD_LIBRARY_PATH**:
```bash
export DYLD_LIBRARY_PATH=/usr/local/lib:$DYLD_LIBRARY_PATH
```

3. **Check architecture**:
```bash
file /path/to/libmeta_oxide.dylib
# Should match your system (arm64 or x86_64)
```

### Windows: DLL Not Found

**Problem**: `The specified module could not be found`

**Solutions**:

1. **Install Visual C++ Redistributable**:
Download from Microsoft: https://aka.ms/vs/17/release/vc_redist.x64.exe

2. **Copy DLL to executable directory**:
```cmd
copy meta_oxide.dll C:\path\to\exe\
```

3. **Add to PATH**:
```cmd
setx PATH "%PATH%;C:\path\to\dll"
```

### Android: UnsatisfiedLinkError

**Problem**: Native library not found on Android

**Solutions**:

1. **Include ABIs**:
```gradle
android {
    defaultConfig {
        ndk {
            abiFilters 'armeabi-v7a', 'arm64-v8a', 'x86', 'x86_64'
        }
    }
}
```

2. **Check library in APK**:
```bash
unzip -l app.apk | grep libmeta_oxide
```

## Getting Help

If none of these solutions work:

1. **Check existing issues**: https://github.com/yourusername/meta_oxide/issues
2. **Create new issue**: Include error message, platform, language version
3. **Join discussions**: https://github.com/yourusername/meta_oxide/discussions
4. **Email support**: support@metaoxide.dev

## See Also

- [FAQ](/docs/troubleshooting/faq.md)
- [Performance Tuning](/docs/performance/performance-tuning-guide.md)
- [Architecture Overview](/docs/architecture/architecture-overview.md)
