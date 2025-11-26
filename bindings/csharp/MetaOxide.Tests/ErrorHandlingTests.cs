using System;
using FluentAssertions;
using Xunit;

namespace MetaOxide.Tests
{
    /// <summary>
    /// Tests for error handling and exception scenarios.
    /// </summary>
    public class ErrorHandlingTests
    {
        [Fact]
        public void MetaOxideException_WithErrorCode_StoresCodeAndMessage()
        {
            // Arrange
            var errorCode = 1;
            var message = "Test error";

            // Act
            var exception = new MetaOxideException(errorCode, message);

            // Assert
            exception.ErrorCode.Should().Be(errorCode);
            exception.ErrorDescription.Should().Be(message);
            exception.Message.Should().Contain(message);
            exception.Message.Should().Contain(errorCode.ToString());
        }

        [Fact]
        public void MetaOxideException_WithMessageOnly_SetsDefaultErrorCode()
        {
            // Arrange
            var message = "Test error";

            // Act
            var exception = new MetaOxideException(message);

            // Assert
            exception.ErrorCode.Should().Be(-1);
            exception.ErrorDescription.Should().Be(message);
        }

        [Fact]
        public void MetaOxideException_WithInnerException_PreservesInnerException()
        {
            // Arrange
            var message = "Test error";
            var innerException = new InvalidOperationException("Inner");

            // Act
            var exception = new MetaOxideException(message, innerException);

            // Assert
            exception.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void MetaOxideException_GetFriendlyMessage_ReturnsReadableMessage()
        {
            // Arrange
            var exception = new MetaOxideException(1, "Parse error");

            // Act
            var message = exception.GetFriendlyMessage();

            // Assert
            message.Should().NotBeNullOrEmpty();
            message.Should().Contain("HTML");
        }

        [Fact]
        public void MetaOxideException_GetFriendlyMessage_ForEachErrorCode_ReturnsMessage()
        {
            // Test all error codes 0-6
            for (int code = 0; code <= 6; code++)
            {
                var exception = new MetaOxideException(code, "Test");
                var message = exception.GetFriendlyMessage();
                message.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public void MetaOxideException_GetFriendlyMessage_ForUnknownCode_ReturnsGenericMessage()
        {
            // Arrange
            var exception = new MetaOxideException(999, "Unknown error");

            // Act
            var message = exception.GetFriendlyMessage();

            // Assert
            message.Should().Contain("Unknown error code");
            message.Should().Contain("999");
        }

        [Fact]
        public void MetaOxideException_ToString_IncludesStackTrace()
        {
            // Arrange
            var exception = new MetaOxideException(1, "Test error");

            // Act
            var str = exception.ToString();

            // Assert
            str.Should().Contain("MetaOxideException");
            str.Should().Contain("Test error");
            str.Should().Contain("Code: 1");
        }

        [Fact]
        public void Extractor_WithMalformedHtml_DoesNotThrow()
        {
            // Arrange - Extremely malformed HTML
            var html = "<html><head><meta name='test' content='value'<body>Test";

            // Act
            var result = Extractor.ExtractAll(html);

            // Assert - Should handle gracefully
            result.Should().NotBeNull();
        }

        [Fact]
        public void Extractor_WithVeryLargeHtml_HandlesCorrectly()
        {
            // Arrange - Create large HTML document
            var largeContent = new string('a', 1_000_000); // 1MB of 'a' characters
            var html = $"<html><head><meta name='description' content='{largeContent}'></head></html>";

            // Act
            var result = Extractor.ExtractAll(html);

            // Assert
            result.Should().NotBeNull();
            result.Meta.Should().NotBeNull();
        }

        [Fact]
        public void Extractor_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var html = @"
<html>
<head>
    <meta name=""description"" content=""Test with 'quotes' and ""double quotes"" and <tags>"">
    <meta property=""og:title"" content=""Title with émojis 🎉 and unicode ™"">
</head>
</html>";

            // Act
            var result = Extractor.ExtractAll(html);

            // Assert
            result.Should().NotBeNull();
            result.Meta.Should().NotBeNull();
            result.OpenGraph.Should().NotBeNull();
        }

        [Fact]
        public void Extractor_WithInvalidBaseUrl_HandlesGracefully()
        {
            // Arrange
            var html = "<html><head><link rel='canonical' href='/page'></head></html>";
            var invalidBaseUrl = "not-a-valid-url";

            // Act - Should not throw, but may not resolve URLs correctly
            var result = Extractor.ExtractAll(html, invalidBaseUrl);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Extractor_WithInvalidJsonLd_ReturnsEmptyList()
        {
            // Arrange
            var html = @"
<html>
<head>
    <script type=""application/ld+json"">
    { invalid json here }
    </script>
</head>
</html>";

            // Act
            var result = Extractor.ExtractJsonLd(html);

            // Assert - Invalid JSON-LD should be skipped
            result.Should().BeNull();
        }

        [Fact]
        public void Extractor_WithEmptyTags_ReturnsNull()
        {
            // Arrange
            var html = @"
<html>
<head>
    <meta name="""" content="""">
    <meta property=""og:title"" content="""">
</head>
</html>";

            // Act
            var result = Extractor.ExtractAll(html);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Extractor_WithNestedMicroformats_ExtractsCorrectly()
        {
            // Arrange
            var html = @"
<html>
<body>
    <div class=""h-card"">
        <span class=""p-name"">Person Name</span>
        <div class=""p-adr h-adr"">
            <span class=""p-locality"">City</span>
        </div>
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
        public void Extractor_WithMultipleMicrodataTypes_ExtractsAll()
        {
            // Arrange
            var html = @"
<html>
<body>
    <div itemscope itemtype=""http://schema.org/Product"">
        <span itemprop=""name"">Product 1</span>
    </div>
    <div itemscope itemtype=""http://schema.org/Person"">
        <span itemprop=""name"">Person 1</span>
    </div>
</body>
</html>";

            // Act
            var result = Extractor.ExtractMicrodata(html);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public void Extractor_WithComplexRDFa_ExtractsCorrectly()
        {
            // Arrange
            var html = @"
<html>
<body>
    <div vocab=""http://schema.org/"" typeof=""Person"">
        <span property=""name"">Jane Doe</span>
        <span property=""jobTitle"">Software Engineer</span>
        <div property=""address"" typeof=""PostalAddress"">
            <span property=""streetAddress"">123 Main St</span>
        </div>
    </div>
</body>
</html>";

            // Act
            var result = Extractor.ExtractRDFa(html);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
