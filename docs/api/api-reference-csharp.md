# MetaOxide C# API Reference

Complete API documentation for the .NET library.

## Table of Contents

- [Installation](#installation)
- [Namespace](#namespace)
- [Classes](#classes)
- [Methods](#methods)
- [Data Types](#data-types)
- [Exceptions](#exceptions)
- [Examples](#examples)

## Installation

### NuGet

```bash
dotnet add package MetaOxide
```

### Package Manager

```powershell
Install-Package MetaOxide
```

## Namespace

```csharp
using MetaOxide;
```

## Classes

### `MetaOxideExtractor`

Main class for extracting metadata. Implements `IDisposable`.

#### Constructor

```csharp
public MetaOxideExtractor(string html, string baseUrl)
```

**Parameters:**
- `html` (string) - HTML content to parse
- `baseUrl` (string) - Base URL for resolving relative URLs

**Throws:**
- `MetaOxideException` - If HTML parsing fails

**Example:**
```csharp
using var extractor = new MetaOxideExtractor(html, "https://example.com");
```

**Important:** Use `using` statement or call `Dispose()` to ensure proper cleanup.

## Methods

### `ExtractAll()`

Extract all metadata formats.

```csharp
public Dictionary<string, object> ExtractAll()
```

**Returns:** `Dictionary<string, object>` - All extracted metadata

**Throws:** `MetaOxideException` - If extraction fails

**Example:**
```csharp
using var extractor = new MetaOxideExtractor(html, url);
var metadata = extractor.ExtractAll();
Console.WriteLine($"Title: {metadata["title"]}");
```

### `ExtractBasicMeta()`

Extract basic HTML metadata.

```csharp
public Dictionary<string, object> ExtractBasicMeta()
```

**Returns:** `Dictionary<string, object>` - Basic metadata

**Example:**
```csharp
var basic = extractor.ExtractBasicMeta();
Console.WriteLine($"Title: {basic["title"]}");
Console.WriteLine($"Description: {basic["description"]}");
```

### `ExtractOpenGraph()`

Extract Open Graph metadata.

```csharp
public OpenGraphData ExtractOpenGraph()
```

**Returns:** `OpenGraphData` or `null` if not present

**Example:**
```csharp
var og = extractor.ExtractOpenGraph();
if (og != null)
{
    Console.WriteLine($"OG Title: {og.Title}");
    Console.WriteLine($"OG Image: {og.Image}");
}
```

### `ExtractTwitterCard()`

Extract Twitter Card metadata.

```csharp
public TwitterCardData ExtractTwitterCard()
```

**Returns:** `TwitterCardData` or `null` if not present

**Example:**
```csharp
var twitter = extractor.ExtractTwitterCard();
if (twitter != null)
{
    Console.WriteLine($"Card Type: {twitter.Card}");
}
```

### `ExtractJSONLD()`

Extract JSON-LD structured data.

```csharp
public List<object> ExtractJSONLD()
```

**Returns:** `List<object>` - List of JSON-LD objects

**Example:**
```csharp
var jsonld = extractor.ExtractJSONLD();
foreach (var item in jsonld)
{
    Console.WriteLine(item);
}
```

### `ExtractMicrodata()`

Extract Microdata items.

```csharp
public List<MicrodataItem> ExtractMicrodata()
```

**Returns:** `List<MicrodataItem>` - List of Microdata items

**Example:**
```csharp
var microdata = extractor.ExtractMicrodata();
foreach (var item in microdata)
{
    Console.WriteLine($"Type: {string.Join(", ", item.Type)}");
}
```

### `ExtractMicroformats()`

Extract Microformats data.

```csharp
public Dictionary<string, List<Dictionary<string, object>>> ExtractMicroformats()
```

**Returns:** `Dictionary` mapping format types to items

**Example:**
```csharp
var mf = extractor.ExtractMicroformats();
if (mf.ContainsKey("h-card"))
{
    foreach (var card in mf["h-card"])
    {
        Console.WriteLine($"Name: {card["name"]}");
    }
}
```

### `ExtractDublinCore()`

Extract Dublin Core metadata.

```csharp
public DublinCoreData ExtractDublinCore()
```

**Returns:** `DublinCoreData` or `null` if not present

### `ExtractRelLinks()`

Extract link relations.

```csharp
public Dictionary<string, List<Link>> ExtractRelLinks()
```

**Returns:** `Dictionary` mapping rel types to links

**Example:**
```csharp
var links = extractor.ExtractRelLinks();
if (links.ContainsKey("canonical"))
{
    Console.WriteLine($"Canonical: {links["canonical"][0].Href}");
}
```

### `Dispose()`

Release native resources. Automatically called by `using` statement.

```csharp
public void Dispose()
```

## Data Types

### `OpenGraphData`

Open Graph metadata class.

```csharp
public class OpenGraphData
{
    public string Title { get; set; }
    public string Type { get; set; }
    public string Image { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public string SiteName { get; set; }
    public string Locale { get; set; }
}
```

### `TwitterCardData`

Twitter Card metadata class.

```csharp
public class TwitterCardData
{
    public string Card { get; set; }
    public string Site { get; set; }
    public string Creator { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string ImageAlt { get; set; }
}
```

### `MicrodataItem`

Microdata item class.

```csharp
public class MicrodataItem
{
    public List<string> Type { get; set; }
    public Dictionary<string, List<object>> Properties { get; set; }
    public string Id { get; set; }
}
```

### `Link`

Link element class.

```csharp
public class Link
{
    public string Href { get; set; }
    public string Rel { get; set; }
    public string Media { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Hreflang { get; set; }
}
```

## Exceptions

### `MetaOxideException`

Base exception for MetaOxide errors.

```csharp
public class MetaOxideException : Exception
{
    public MetaOxideException(string message) : base(message) { }

    public MetaOxideException(string message, Exception innerException)
        : base(message, innerException) { }
}
```

### Exception Handling

```csharp
try
{
    using var extractor = new MetaOxideExtractor(html, url);
    var metadata = extractor.ExtractAll();
}
catch (MetaOxideException ex)
{
    Console.WriteLine($"MetaOxide error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Examples

### Basic Usage (.NET 6+)

```csharp
using MetaOxide;

string html = """
<!DOCTYPE html>
<html>
<head>
    <title>My Page</title>
</head>
</html>
""";

using var extractor = new MetaOxideExtractor(html, "https://example.com");
var metadata = extractor.ExtractAll();

Console.WriteLine($"Title: {metadata["title"]}");
```

### Extract from URL (Async)

```csharp
using System.Net.Http;
using MetaOxide;

public class MetadataExtractor
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<Dictionary<string, object>> ExtractFromURLAsync(string url)
    {
        string html = await client.GetStringAsync(url);

        using var extractor = new MetaOxideExtractor(html, url);
        return extractor.ExtractAll();
    }
}

// Usage
var metadata = await MetadataExtractor.ExtractFromURLAsync("https://example.com");
Console.WriteLine($"Title: {metadata["title"]}");
```

### Parallel Processing

```csharp
using System.Linq;
using System.Threading.Tasks;

public static async Task<List<Dictionary<string, object>>> ExtractMultipleAsync(
    List<string> urls)
{
    var tasks = urls.Select(async url =>
    {
        try
        {
            return await ExtractFromURLAsync(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {url} - {ex.Message}");
            return null;
        }
    });

    var results = await Task.WhenAll(tasks);
    return results.Where(r => r != null).ToList();
}
```

### ASP.NET Core Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using MetaOxide;

[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public MetadataController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public async Task<IActionResult> Extract([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("URL required");
        }

        try
        {
            string html = await _httpClient.GetStringAsync(url);

            using var extractor = new MetaOxideExtractor(html, url);
            var metadata = extractor.ExtractAll();

            return Ok(new { success = true, metadata });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
```

### .NET Framework 4.6.1+

```csharp
using System;
using System.Collections.Generic;
using MetaOxide;

namespace MetaOxideExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string html = @"<!DOCTYPE html>...";

            using (var extractor = new MetaOxideExtractor(html, "https://example.com"))
            {
                Dictionary<string, object> metadata = extractor.ExtractAll();
                Console.WriteLine("Title: " + metadata["title"]);
            }
        }
    }
}
```

### Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;

public interface IMetadataService
{
    Task<Dictionary<string, object>> ExtractAsync(string url);
}

public class MetadataService : IMetadataService
{
    private readonly HttpClient _httpClient;

    public MetadataService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Dictionary<string, object>> ExtractAsync(string url)
    {
        string html = await _httpClient.GetStringAsync(url);

        using var extractor = new MetaOxideExtractor(html, url);
        return extractor.ExtractAll();
    }
}

// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient();
    services.AddScoped<IMetadataService, MetadataService>();
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var metadata = extractor.ExtractAll();
string json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
{
    WriteIndented = true
});

Console.WriteLine(json);
```

### Selective Extraction

```csharp
using var extractor = new MetaOxideExtractor(html, url);

// Only extract social media metadata
var og = extractor.ExtractOpenGraph();
var twitter = extractor.ExtractTwitterCard();

// Skip other formats for better performance
```

## Thread Safety

MetaOxide is thread-safe. Each thread should create its own instance.

```csharp
// Safe: Each task has its own extractor
await Task.WhenAll(urls.Select(async url =>
{
    using var extractor = new MetaOxideExtractor(html, url);
    return extractor.ExtractAll();
}));
```

## Performance Tips

1. **Use `using` statements**: Ensures proper cleanup
2. **Async/await**: Use async methods for I/O operations
3. **Parallel.ForEach**: Process multiple documents concurrently
4. **Selective extraction**: Extract only needed formats
5. **HttpClient reuse**: Use IHttpClientFactory

## See Also

- [Getting Started Guide](/docs/getting-started/getting-started-csharp.md)
- [ASP.NET Core Integration](/docs/integrations/aspnetcore-integration.md)
- [Examples](/examples/real-world/csharp-aspnet-api/)
