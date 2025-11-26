using System;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MetaOxide.Tests
{
    /// <summary>
    /// Tests for JSON extraction methods.
    /// </summary>
    public class JsonExtractionTests
    {
        private const string HtmlWithAllFormats = @"
<!DOCTYPE html>
<html>
<head>
    <title>Test Page</title>
    <meta name=""description"" content=""Test description"">
    <meta property=""og:title"" content=""OG Title"">
    <meta name=""twitter:card"" content=""summary"">
    <meta name=""DC.title"" content=""DC Title"">
    <link rel=""canonical"" href=""https://example.com/page"">
    <link rel=""manifest"" href=""/manifest.json"">
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Article"",
        ""headline"": ""Test Article""
    }
    </script>
</head>
<body>
    <div class=""h-card"">
        <span class=""p-name"">John Doe</span>
    </div>
    <div itemscope itemtype=""http://schema.org/Product"">
        <span itemprop=""name"">Product Name</span>
    </div>
</body>
</html>";

        [Fact]
        public void ExtractMetaJson_WithMetaTags_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractMetaJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["description"].Should().NotBeNull();
        }

        [Fact]
        public void ExtractMetaJson_WithNoMetaTags_ReturnsNull()
        {
            // Arrange
            var html = "<html><body>No meta</body></html>";

            // Act
            var json = Extractor.ExtractMetaJson(html);

            // Assert
            json.Should().BeNull();
        }

        [Fact]
        public void ExtractOpenGraphJson_WithOGTags_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractOpenGraphJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["title"].Should().NotBeNull();
            parsed["title"]!.ToString().Should().Be("OG Title");
        }

        [Fact]
        public void ExtractTwitterJson_WithTwitterTags_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractTwitterJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["card"].Should().NotBeNull();
        }

        [Fact]
        public void ExtractJsonLdJson_WithJsonLdScript_ReturnsValidJsonArray()
        {
            // Act
            var json = Extractor.ExtractJsonLdJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JArray.Parse(json);
            parsed.Should().HaveCount(1);
            parsed[0]["@type"]!.ToString().Should().Be("Article");
        }

        [Fact]
        public void ExtractJsonLdJson_WithMultipleScripts_ReturnsAllItems()
        {
            // Arrange
            var html = @"
<html>
<head>
    <script type=""application/ld+json"">
    { ""@type"": ""Article"" }
    </script>
    <script type=""application/ld+json"">
    { ""@type"": ""Organization"" }
    </script>
</head>
</html>";

            // Act
            var json = Extractor.ExtractJsonLdJson(html);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JArray.Parse(json);
            parsed.Should().HaveCount(2);
        }

        [Fact]
        public void ExtractMicrodataJson_WithMicrodata_ReturnsValidJsonArray()
        {
            // Act
            var json = Extractor.ExtractMicrodataJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JArray.Parse(json);
            parsed.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void ExtractMicroformatsJson_WithMicroformats_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractMicroformatsJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["h-card"].Should().NotBeNull();
        }

        [Fact]
        public void ExtractDublinCoreJson_WithDCTags_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractDublinCoreJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["title"].Should().NotBeNull();
        }

        [Fact]
        public void ExtractManifestJson_WithManifestLink_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractManifestJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["href"].Should().NotBeNull();
        }

        [Fact]
        public void ExtractRelLinksJson_WithRelLinks_ReturnsValidJson()
        {
            // Act
            var json = Extractor.ExtractRelLinksJson(HtmlWithAllFormats);

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["canonical"].Should().NotBeNull();
        }

        [Fact]
        public void AllJsonMethods_WithNullHtml_ThrowArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractMetaJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractOpenGraphJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractTwitterJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractJsonLdJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractMicrodataJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractMicroformatsJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractRDFaJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractDublinCoreJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractManifestJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractOEmbedJson(null!));
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractRelLinksJson(null!));
        }

        [Fact]
        public void JsonExtraction_WithBaseUrl_IncludesResolvedUrls()
        {
            // Arrange
            var html = @"
<html>
<head>
    <link rel=""canonical"" href=""/page"">
</head>
</html>";

            // Act
            var json = Extractor.ExtractRelLinksJson(html, "https://example.com");

            // Assert
            json.Should().NotBeNullOrEmpty();
            var parsed = JObject.Parse(json);
            parsed["canonical"].Should().NotBeNull();
            parsed["canonical"]!.ToString().Should().Contain("https://example.com");
        }
    }
}
