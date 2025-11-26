using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace MetaOxide.Tests
{
    /// <summary>
    /// Tests for the ExtractionResult class.
    /// </summary>
    public class ExtractionResultTests
    {
        [Fact]
        public void Constructor_CreatesEmptyResult()
        {
            // Act
            var result = new ExtractionResult();

            // Assert
            result.Should().NotBeNull();
            result.HasAnyMetadata().Should().BeFalse();
            result.GetMetadataFormatCount().Should().Be(0);
        }

        [Fact]
        public void HasAnyMetadata_WithNoData_ReturnsFalse()
        {
            // Arrange
            var result = new ExtractionResult();

            // Act & Assert
            result.HasAnyMetadata().Should().BeFalse();
        }

        [Fact]
        public void HasAnyMetadata_WithMetaData_ReturnsTrue()
        {
            // Arrange
            var result = new ExtractionResult
            {
                Meta = new Dictionary<string, object> { ["title"] = "Test" }
            };

            // Act & Assert
            result.HasAnyMetadata().Should().BeTrue();
        }

        [Fact]
        public void HasAnyMetadata_WithOpenGraphData_ReturnsTrue()
        {
            // Arrange
            var result = new ExtractionResult
            {
                OpenGraph = new Dictionary<string, object> { ["title"] = "Test" }
            };

            // Act & Assert
            result.HasAnyMetadata().Should().BeTrue();
        }

        [Fact]
        public void HasAnyMetadata_WithJsonLdData_ReturnsTrue()
        {
            // Arrange
            var result = new ExtractionResult
            {
                JsonLd = new List<object> { new { type = "Article" } }
            };

            // Act & Assert
            result.HasAnyMetadata().Should().BeTrue();
        }

        [Fact]
        public void GetMetadataFormatCount_WithNoData_ReturnsZero()
        {
            // Arrange
            var result = new ExtractionResult();

            // Act & Assert
            result.GetMetadataFormatCount().Should().Be(0);
        }

        [Fact]
        public void GetMetadataFormatCount_WithMultipleFormats_ReturnsCorrectCount()
        {
            // Arrange
            var result = new ExtractionResult
            {
                Meta = new Dictionary<string, object> { ["title"] = "Test" },
                OpenGraph = new Dictionary<string, object> { ["title"] = "Test" },
                Twitter = new Dictionary<string, object> { ["card"] = "summary" }
            };

            // Act & Assert
            result.GetMetadataFormatCount().Should().Be(3);
        }

        [Fact]
        public void GetMetadataFormatCount_WithAllFormats_Returns11()
        {
            // Arrange
            var result = new ExtractionResult
            {
                Meta = new Dictionary<string, object> { ["title"] = "Test" },
                OpenGraph = new Dictionary<string, object> { ["title"] = "Test" },
                Twitter = new Dictionary<string, object> { ["card"] = "summary" },
                JsonLd = new List<object> { new { type = "Article" } },
                Microdata = new List<object> { new { type = "Product" } },
                Microformats = new Dictionary<string, object> { ["h-card"] = new[] { new { } } },
                RDFa = new List<object> { new { } },
                DublinCore = new Dictionary<string, object> { ["title"] = "Test" },
                Manifest = new Dictionary<string, object> { ["href"] = "/manifest.json" },
                OEmbed = new Dictionary<string, object> { ["href"] = "..." },
                RelLinks = new Dictionary<string, object> { ["canonical"] = "..." }
            };

            // Act & Assert
            result.GetMetadataFormatCount().Should().Be(11);
        }

        [Fact]
        public void ToJson_WithData_ReturnsValidJson()
        {
            // Arrange
            var result = new ExtractionResult
            {
                Meta = new Dictionary<string, object>
                {
                    ["title"] = "Test Title",
                    ["description"] = "Test Description"
                }
            };

            // Act
            var json = result.ToJson(Formatting.None);

            // Assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Contain("\"meta\"");
            json.Should().Contain("Test Title");
        }

        [Fact]
        public void ToJson_WithIndentation_ReturnsFormattedJson()
        {
            // Arrange
            var result = new ExtractionResult
            {
                Meta = new Dictionary<string, object> { ["title"] = "Test" }
            };

            // Act
            var json = result.ToJson(Formatting.Indented);

            // Assert
            json.Should().Contain("\n");
            json.Should().Contain("  ");
        }

        [Fact]
        public void FromJson_WithValidJson_ReturnsResult()
        {
            // Arrange
            var json = @"{
                ""meta"": {
                    ""title"": ""Test Title"",
                    ""description"": ""Test Description""
                },
                ""open_graph"": {
                    ""title"": ""OG Title""
                }
            }";

            // Act
            var result = ExtractionResult.FromJson(json);

            // Assert
            result.Should().NotBeNull();
            result.Meta.Should().NotBeNull();
            result.Meta.Should().ContainKey("title");
            result.OpenGraph.Should().NotBeNull();
        }

        [Fact]
        public void FromJson_WithInvalidJson_ThrowsJsonException()
        {
            // Arrange
            var json = "{ invalid json }";

            // Act & Assert
            Assert.Throws<JsonException>(() => ExtractionResult.FromJson(json));
        }

        [Fact]
        public void ToString_ReturnsDescription()
        {
            // Arrange
            var result = new ExtractionResult
            {
                Meta = new Dictionary<string, object> { ["title"] = "Test" },
                OpenGraph = new Dictionary<string, object> { ["title"] = "Test" }
            };

            // Act
            var str = result.ToString();

            // Assert
            str.Should().Contain("ExtractionResult");
            str.Should().Contain("2");
            str.Should().Contain("format");
        }

        [Fact]
        public void JsonSerialization_RoundTrip_PreservesData()
        {
            // Arrange
            var original = new ExtractionResult
            {
                Meta = new Dictionary<string, object>
                {
                    ["title"] = "Test Title",
                    ["description"] = "Test Description"
                },
                OpenGraph = new Dictionary<string, object>
                {
                    ["title"] = "OG Title",
                    ["type"] = "website"
                },
                JsonLd = new List<object>
                {
                    new Dictionary<string, object>
                    {
                        ["@type"] = "Article",
                        ["headline"] = "Test"
                    }
                }
            };

            // Act
            var json = original.ToJson();
            var deserialized = ExtractionResult.FromJson(json);

            // Assert
            deserialized.Meta.Should().NotBeNull();
            deserialized.Meta.Should().ContainKey("title");
            deserialized.Meta!["title"].ToString().Should().Be("Test Title");
            deserialized.OpenGraph.Should().NotBeNull();
            deserialized.JsonLd.Should().NotBeNull();
            deserialized.JsonLd.Should().HaveCount(1);
        }
    }
}
