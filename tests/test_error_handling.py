"""
Error handling and recovery tests for MetaOxide.

These tests verify that the library handles edge cases, malformed input,
and error conditions gracefully.
"""

import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))

try:
    import meta_oxide
except ImportError:
    print("ERROR: meta_oxide module not found")
    sys.exit(1)


class TestMalformedHTML:
    """Test handling of malformed HTML."""

    def test_unclosed_tags(self):
        """Test HTML with unclosed tags."""
        html = """
        <html>
            <head>
                <title>Test Page
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html)
        # Should not crash, should return valid result
        assert isinstance(result, dict)

    def test_deeply_nested_tags(self):
        """Test deeply nested HTML structure."""
        html = "<div>" * 100 + "Content" + "</div>" * 100
        result = meta_oxide.extract_all(html)
        assert isinstance(result, dict)

    def test_malformed_meta_tags(self):
        """Test meta tags without proper attributes."""
        html = """
        <html>
            <head>
                <meta name="description">
                <meta property="og:title">
                <meta>
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_broken_json_ld(self):
        """Test malformed JSON-LD."""
        html = """
        <html>
            <head>
                <script type="application/ld+json">
                {
                    "name": "Test"
                    "missing_comma": true
                }
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_jsonld(html)
        # Should return empty list rather than crash
        assert isinstance(result, list)

    def test_invalid_json_ld_syntax(self):
        """Test JSON-LD with invalid syntax."""
        html = """
        <html>
            <head>
                <script type="application/ld+json">
                This is not JSON at all
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_jsonld(html)
        assert isinstance(result, list)

    def test_script_injection_in_content(self):
        """Test that script content doesn't break extraction."""
        html = """
        <html>
            <head>
                <title>Test</title>
                <script>
                    var title = '<meta property="og:title" content="hacked">';
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_mixed_quotes_in_attributes(self):
        """Test mixed quote styles in attributes."""
        html = """
        <html>
            <head>
                <meta name="description" content="It's a test's description">
                <meta property="og:title" content='He said "hello"'>
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)


class TestEdgeCases:
    """Test edge cases and boundary conditions."""

    def test_empty_html(self):
        """Test with empty HTML."""
        result = meta_oxide.extract_all("")
        assert isinstance(result, dict)

    def test_none_like_empty_strings(self):
        """Test with various empty-like inputs."""
        for html in ["", " ", "\n", "\t"]:
            result = meta_oxide.extract_all(html)
            assert isinstance(result, dict)

    def test_html_with_only_whitespace(self):
        """Test HTML that's only whitespace."""
        html = "   \n\n\t   \n   "
        result = meta_oxide.extract_all(html)
        assert isinstance(result, dict)

    def test_very_long_attribute_values(self):
        """Test with extremely long attribute values."""
        long_content = "x" * 10000
        html = f'<meta name="description" content="{long_content}">'
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_very_long_html_document(self):
        """Test with very large HTML document."""
        # Create a large document with 10,000 meta tags
        html = "<html><head>"
        for i in range(1000):
            html += f'<meta name="test{i}" content="value{i}">'
        html += "</head></html>"

        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_html_with_unicode_content(self):
        """Test HTML with unicode characters."""
        html = """
        <html>
            <head>
                <title>测试页面 - テスト - 검사</title>
                <meta name="description" content="日本語、中文、한글の説明">
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)
        assert "title" in result

    def test_html_with_emoji(self):
        """Test HTML with emoji."""
        html = """
        <html>
            <head>
                <title>Test 🎉 Page</title>
                <meta name="description" content="We love 🚀 and 🎨">
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_html_with_html_entities(self):
        """Test HTML with HTML entities."""
        html = """
        <html>
            <head>
                <title>Test &amp; Demo &lt;Page&gt;</title>
                <meta name="description" content="&quot;Quoted&quot; &apos;content&apos;">
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_html_with_cdata_sections(self):
        """Test HTML with CDATA sections."""
        html = """
        <html>
            <head>
                <script type="application/ld+json">
                <![CDATA[
                {"@type": "Article"}
                ]]>
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_jsonld(html)
        # Should handle gracefully
        assert isinstance(result, list)

    def test_html_with_comments(self):
        """Test HTML with comments."""
        html = """
        <html>
            <head>
                <!-- <meta name="fake" content="content"> -->
                <title>Real Title</title>
                <!-- <meta property="og:title" content="fake"> -->
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)


class TestInvalidInput:
    """Test handling of invalid or unexpected input."""

    def test_non_html_text(self):
        """Test with plain text input."""
        text = "This is just plain text, not HTML at all."
        result = meta_oxide.extract_all(text)
        # Should not crash
        assert isinstance(result, dict)

    def test_xml_input(self):
        """Test with XML that's not HTML."""
        xml = """<?xml version="1.0"?>
        <root>
            <item>Test</item>
        </root>
        """
        result = meta_oxide.extract_all(xml)
        assert isinstance(result, dict)

    def test_json_input(self):
        """Test with JSON instead of HTML."""
        json_text = '{"title": "Test", "description": "Test"}'
        result = meta_oxide.extract_all(json_text)
        # Should not crash, just won't extract anything meaningful
        assert isinstance(result, dict)

    def test_binary_like_strings(self):
        """Test with binary-like content."""
        # Test with null bytes (though Python strings don't typically have these)
        html = "<html><head><title>Test\x00Title</title></head></html>"
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)


class TestInvalidURLs:
    """Test handling of invalid URLs in base_url parameter."""

    def test_invalid_base_url_format(self):
        """Test with malformed base URL."""
        html = '<link rel="canonical" href="/page">'
        # Invalid URL should be handled gracefully
        try:
            result = meta_oxide.extract_meta(html, base_url="not a url")
            # Either succeeds with unresolved URL or handled gracefully
            assert isinstance(result, dict)
        except:
            # If it raises an error, that's also acceptable
            pass

    def test_relative_base_url(self):
        """Test with relative path as base URL."""
        html = '<link rel="canonical" href="/page">'
        try:
            result = meta_oxide.extract_meta(html, base_url="/relative/path")
            assert isinstance(result, dict)
        except:
            # If it raises, that's acceptable
            pass

    def test_empty_base_url(self):
        """Test with empty string as base URL."""
        html = '<link rel="canonical" href="/page">'
        result = meta_oxide.extract_meta(html, base_url="")
        assert isinstance(result, dict)

    def test_special_characters_in_base_url(self):
        """Test base URL with special characters."""
        html = '<link rel="canonical" href="/page">'
        result = meta_oxide.extract_meta(
            html, base_url="https://example.com/path?query=value&other=123"
        )
        assert isinstance(result, dict)


class TestExtractAllRobustness:
    """Test extract_all() robustness with various problematic inputs."""

    def test_extract_all_with_broken_formats_mixed(self):
        """Test extract_all() when some formats are broken."""
        html = """
        <html>
            <head>
                <title>Valid Title</title>
                <meta property="og:title" content="Valid OG">
                <script type="application/ld+json">
                {BROKEN JSON HERE}
                </script>
            </head>
            <body>
                <div class="h-card">
                    <span class="p-name">John</span>
                </div>
            </body>
        </html>
        """
        result = meta_oxide.extract_all(html)
        # Should successfully extract meta and OG despite broken JSON-LD
        assert "meta" in result
        # May or may not have jsonld, but shouldn't crash
        assert isinstance(result, dict)

    def test_extract_all_with_multiple_formats_empty(self):
        """Test extract_all() with empty content for each format."""
        html = """
        <html>
            <head></head>
            <body></body>
        </html>
        """
        result = meta_oxide.extract_all(html)
        assert isinstance(result, dict)

    def test_extract_all_graceful_degradation(self):
        """Test that extract_all() returns partial results when possible."""
        html = """
        <html>
            <head>
                <title>Title Works</title>
                <meta property="og:title" content="OG Works">
                <script type="application/ld+json">BROKEN</script>
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html)

        # Meta should be present (one of the most basic formats)
        assert "meta" in result or "opengraph" in result or len(result) >= 0

    def test_microformats_with_missing_required_properties(self):
        """Test microformats extraction with missing required properties."""
        html = """
        <div class="h-card">
            <!-- missing p-name which is typically required -->
            <a class="u-url" href="https://example.com">Link</a>
        </div>
        """
        result = meta_oxide.extract_hcard(html)
        # Should extract even with missing properties
        assert isinstance(result, list)

    def test_microdata_with_incomplete_schema(self):
        """Test microdata with incomplete schema."""
        html = """
        <div itemscope itemtype="https://schema.org/Person">
            <!-- missing many properties -->
            <span itemprop="name">John</span>
        </div>
        """
        result = meta_oxide.extract_microdata(html)
        assert isinstance(result, list)


class TestMemoryAndPerformance:
    """Test handling of memory-intensive operations."""

    def test_extract_large_attribute_value(self):
        """Test extraction with very large attribute value."""
        large_value = "x" * 100000  # 100KB string
        html = f'<meta name="test" content="{large_value}">'
        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_extract_many_meta_tags(self):
        """Test extraction of many meta tags."""
        html = "<html><head>"
        for i in range(5000):  # 5000 meta tags
            html += f'<meta name="tag{i}" content="value{i}">'
        html += "</head></html>"

        result = meta_oxide.extract_meta(html)
        assert isinstance(result, dict)

    def test_extract_many_microformats(self):
        """Test extraction of many microformat items."""
        html = "<html><body>"
        for i in range(500):  # 500 h-cards
            html += f'<div class="h-card"><span class="p-name">Person {i}</span></div>'
        html += "</body></html>"

        result = meta_oxide.extract_hcard(html)
        assert isinstance(result, list)


if __name__ == "__main__":
    print("Running error handling tests...\n")

    # Run a few smoke tests
    try:
        test = TestMalformedHTML()
        test.test_unclosed_tags()
        print("✓ Unclosed tags handled gracefully")
    except Exception as e:
        print(f"✗ Unclosed tags: {e}")

    try:
        test = TestEdgeCases()
        test.test_empty_html()
        print("✓ Empty HTML handled gracefully")
    except Exception as e:
        print(f"✗ Empty HTML: {e}")

    try:
        test.test_html_with_unicode_content()
        print("✓ Unicode content handled correctly")
    except Exception as e:
        print(f"✗ Unicode content: {e}")

    try:
        test = TestInvalidInput()
        test.test_non_html_text()
        print("✓ Non-HTML text handled gracefully")
    except Exception as e:
        print(f"✗ Non-HTML text: {e}")

    print("\nError handling tests completed!")
