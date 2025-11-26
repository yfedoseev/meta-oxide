using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace MetaOxide.Tests
{
    /// <summary>
    /// Tests for the main Extractor API.
    /// </summary>
    public class ExtractorTests
    {
        private const string SimpleHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>Test Page</title>
    <meta name=""description"" content=""Test description"">
    <meta name=""keywords"" content=""test,seo"">
    <meta property=""og:title"" content=""OG Title"">
    <meta property=""og:type"" content=""website"">
    <meta name=""twitter:card"" content=""summary"">
    <meta name=""twitter:title"" content=""Twitter Title"">
</head>
<body>
    <h1>Test</h1>
</body>
</html>";

        [Fact]
        public void ExtractAll_WithValidHtml_ReturnsResult()
        {
            // Act
            var result = Extractor.ExtractAll(SimpleHtml);

            // Assert
            result.Should().NotBeNull();
            result.HasAnyMetadata().Should().BeTrue();
        }

        [Fact]
        public void ExtractAll_WithNullHtml_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractAll(null!));
        }

        [Fact]
        public void ExtractAll_WithEmptyHtml_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Extractor.ExtractAll(""));
        }

        [Fact]
        public void ExtractAll_WithBaseUrl_ReturnsResult()
        {
            // Act
            var result = Extractor.ExtractAll(SimpleHtml, "https://example.com");

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void ExtractMeta_WithMetaTags_ReturnsDictionary()
        {
            // Act
            var result = Extractor.ExtractMeta(SimpleHtml);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("description");
            result.Should().ContainKey("keywords");
        }

        [Fact]
        public void ExtractMeta_WithNoMetaTags_ReturnsNull()
        {
            // Arrange
            var html = "<html><body>No meta tags</body></html>";

            // Act
            var result = Extractor.ExtractMeta(html);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractOpenGraph_WithOGTags_ReturnsDictionary()
        {
            // Act
            var result = Extractor.ExtractOpenGraph(SimpleHtml);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("title");
            result.Should().ContainKey("type");
            result!["title"].ToString().Should().Be("OG Title");
        }

        [Fact]
        public void ExtractTwitter_WithTwitterTags_ReturnsDictionary()
        {
            // Act
            var result = Extractor.ExtractTwitter(SimpleHtml);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("card");
            result.Should().ContainKey("title");
            result!["card"].ToString().Should().Be("summary");
        }

        [Fact]
        public void ExtractJsonLd_WithJsonLdScript_ReturnsList()
        {
            // Arrange
            var html = @"
<html>
<head>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Article"",
        ""headline"": ""Test Article""
    }
    </script>
</head>
</html>";

            // Act
            var result = Extractor.ExtractJsonLd(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public void ExtractMicrodata_WithMicrodataMarkup_ReturnsList()
        {
            // Arrange
            var html = @"
<html>
<body>
    <div itemscope itemtype=""http://schema.org/Product"">
        <span itemprop=""name"">Product Name</span>
    </div>
</body>
</html>";

            // Act
            var result = Extractor.ExtractMicrodata(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public void ExtractMicroformats_WithHCard_ReturnsDictionary()
        {
            // Arrange
            var html = @"
<html>
<body>
    <div class=""h-card"">
        <span class=""p-name"">John Doe</span>
        <a class=""u-email"" href=""mailto:john@example.com"">Email</a>
    </div>
</body>
</html>";

            // Act
            var result = Extractor.ExtractMicroformats(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("h-card");
        }

        [Fact]
        public void ExtractRDFa_WithRDFaMarkup_ReturnsList()
        {
            // Arrange
            var html = @"
<html>
<body>
    <div vocab=""http://schema.org/"" typeof=""Person"">
        <span property=""name"">Jane Doe</span>
    </div>
</body>
</html>";

            // Act
            var result = Extractor.ExtractRDFa(html);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void ExtractDublinCore_WithDCTags_ReturnsDictionary()
        {
            // Arrange
            var html = @"
<html>
<head>
    <meta name=""DC.title"" content=""Dublin Core Title"">
    <meta name=""DC.creator"" content=""Author Name"">
</head>
</html>";

            // Act
            var result = Extractor.ExtractDublinCore(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("title");
            result.Should().ContainKey("creator");
        }

        [Fact]
        public void ExtractManifest_WithManifestLink_ReturnsDictionary()
        {
            // Arrange
            var html = @"
<html>
<head>
    <link rel=""manifest"" href=""/manifest.json"">
</head>
</html>";

            // Act
            var result = Extractor.ExtractManifest(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("href");
        }

        [Fact]
        public void ExtractOEmbed_WithOEmbedLink_ReturnsDictionary()
        {
            // Arrange
            var html = @"
<html>
<head>
    <link rel=""alternate"" type=""application/json+oembed""
          href=""https://example.com/oembed?url=..."">
</head>
</html>";

            // Act
            var result = Extractor.ExtractOEmbed(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("href");
        }

        [Fact]
        public void ExtractRelLinks_WithRelLinks_ReturnsDictionary()
        {
            // Arrange
            var html = @"
<html>
<head>
    <link rel=""canonical"" href=""https://example.com/page"">
    <link rel=""alternate"" href=""https://example.com/page?lang=es"" hreflang=""es"">
</head>
</html>";

            // Act
            var result = Extractor.ExtractRelLinks(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("canonical");
            result.Should().ContainKey("alternate");
        }

        [Fact]
        public void ExtractAll_WithComplexDocument_ExtractsMultipleFormats()
        {
            // Arrange
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Complex Page</title>
    <meta name=""description"" content=""Test"">
    <meta property=""og:title"" content=""OG Test"">
    <meta name=""twitter:card"" content=""summary"">
    <script type=""application/ld+json"">
    { ""@type"": ""Article"" }
    </script>
</head>
<body>
    <div class=""h-card"">
        <span class=""p-name"">Test</span>
    </div>
</body>
</html>";

            // Act
            var result = Extractor.ExtractAll(html);

            // Assert
            result.Should().NotBeNull();
            result.Meta.Should().NotBeNull();
            result.OpenGraph.Should().NotBeNull();
            result.Twitter.Should().NotBeNull();
            result.JsonLd.Should().NotBeNull();
            result.Microformats.Should().NotBeNull();
            result.GetMetadataFormatCount().Should().BeGreaterThan(3);
        }

        [Fact]
        public void GetDiagnosticInfo_ReturnsInformation()
        {
            // Act
            var info = Extractor.GetDiagnosticInfo();

            // Assert
            info.Should().NotBeNullOrEmpty();
            info.Should().Contain("Platform:");
            info.Should().Contain("Architecture:");
        }
    }
}
