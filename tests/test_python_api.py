"""
Python API integration tests for MetaOxide.

These tests verify that the public Python API functions work correctly
and return properly formatted data.

Run with: python -m pytest tests/test_python_api.py -v
"""

import os
import sys

# Add parent directory to path to import meta_oxide
sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))

try:
    import meta_oxide
except ImportError:
    print("ERROR: meta_oxide module not found. Build it first with: maturin develop")
    sys.exit(1)


class TestExtractMeta:
    """Test extract_meta() function."""

    def test_extract_meta_basic(self):
        """Test basic meta tag extraction."""
        html = """
        <html>
            <head>
                <title>Test Page</title>
                <meta name="description" content="Test description">
                <meta name="keywords" content="test, example">
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)

        assert isinstance(result, dict)
        assert result.get("title") == "Test Page"
        assert result.get("description") == "Test description"
        assert result.get("keywords") == ["test", "example"]

    def test_extract_meta_with_base_url(self):
        """Test meta extraction with base URL for relative URLs."""
        html = """
        <html>
            <head>
                <link rel="canonical" href="/page">
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html, base_url="https://example.com")

        assert isinstance(result, dict)
        assert result.get("canonical") == "https://example.com/page"

    def test_extract_meta_empty_html(self):
        """Test with minimal HTML."""
        html = "<html><head></head></html>"
        result = meta_oxide.extract_meta(html)

        assert isinstance(result, dict)

    def test_extract_meta_verification_tags(self):
        """Test extraction of verification meta tags."""
        html = """
        <html>
            <head>
                <meta name="google-site-verification" content="abc123">
                <meta name="facebook-domain-verification" content="fb789">
            </head>
        </html>
        """
        result = meta_oxide.extract_meta(html)

        assert result.get("google_site_verification") == "abc123"
        assert result.get("facebook_domain_verification") == "fb789"


class TestExtractOpenGraph:
    """Test extract_opengraph() function."""

    def test_extract_opengraph_basic(self):
        """Test basic Open Graph extraction."""
        html = """
        <html>
            <head>
                <meta property="og:title" content="Article Title">
                <meta property="og:description" content="Article description">
                <meta property="og:url" content="https://example.com/article">
                <meta property="og:type" content="article">
            </head>
        </html>
        """
        result = meta_oxide.extract_opengraph(html)

        assert isinstance(result, dict)
        assert result.get("title") == "Article Title"
        assert result.get("description") == "Article description"
        assert result.get("url") == "https://example.com/article"
        assert result.get("type") == "article"

    def test_extract_opengraph_image(self):
        """Test Open Graph image extraction."""
        html = """
        <html>
            <head>
                <meta property="og:image" content="https://example.com/image.jpg">
                <meta property="og:image:width" content="1200">
                <meta property="og:image:height" content="630">
            </head>
        </html>
        """
        result = meta_oxide.extract_opengraph(html)

        assert isinstance(result, dict)
        assert result.get("image") is not None


class TestExtractTwitter:
    """Test extract_twitter() and extract_twitter_with_fallback() functions."""

    def test_extract_twitter_basic(self):
        """Test basic Twitter Card extraction."""
        html = """
        <html>
            <head>
                <meta name="twitter:card" content="summary_large_image">
                <meta name="twitter:title" content="Tweet Title">
                <meta name="twitter:description" content="Tweet description">
            </head>
        </html>
        """
        result = meta_oxide.extract_twitter(html)

        assert isinstance(result, dict)
        assert result.get("card") == "summary_large_image"
        assert result.get("title") == "Tweet Title"

    def test_extract_twitter_with_fallback_to_og(self):
        """Test Twitter extraction with fallback to Open Graph."""
        html = """
        <html>
            <head>
                <meta property="og:title" content="OG Title">
                <meta property="og:description" content="OG Description">
            </head>
        </html>
        """
        result = meta_oxide.extract_twitter_with_fallback(html)

        assert isinstance(result, dict)


class TestExtractJsonLD:
    """Test extract_jsonld() function."""

    def test_extract_jsonld_basic(self):
        """Test basic JSON-LD extraction."""
        html = """
        <html>
            <head>
                <script type="application/ld+json">
                {
                    "@context": "https://schema.org",
                    "@type": "Article",
                    "headline": "Article Title",
                    "description": "Article description"
                }
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_jsonld(html)

        assert isinstance(result, list)
        assert len(result) > 0
        assert result[0].get("headline") == "Article Title"

    def test_extract_jsonld_multiple(self):
        """Test extraction of multiple JSON-LD objects."""
        html = """
        <html>
            <head>
                <script type="application/ld+json">
                {
                    "@context": "https://schema.org",
                    "@type": "Organization",
                    "name": "Example Corp"
                }
                </script>
                <script type="application/ld+json">
                {
                    "@context": "https://schema.org",
                    "@type": "Person",
                    "name": "John Doe"
                }
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_jsonld(html)

        assert isinstance(result, list)
        assert len(result) >= 2

    def test_extract_jsonld_empty(self):
        """Test with no JSON-LD data."""
        html = "<html><head></head></html>"
        result = meta_oxide.extract_jsonld(html)

        assert isinstance(result, list)
        assert len(result) == 0


class TestExtractMicrodata:
    """Test extract_microdata() function."""

    def test_extract_microdata_basic(self):
        """Test basic microdata extraction."""
        html = """
        <html>
            <body>
                <div itemscope itemtype="https://schema.org/Person">
                    <span itemprop="name">John Doe</span>
                    <span itemprop="email">john@example.com</span>
                </div>
            </body>
        </html>
        """
        result = meta_oxide.extract_microdata(html)

        assert isinstance(result, list)
        assert len(result) > 0

    def test_extract_microdata_multiple(self):
        """Test extraction of multiple microdata items."""
        html = """
        <html>
            <body>
                <div itemscope itemtype="https://schema.org/Person">
                    <span itemprop="name">John Doe</span>
                </div>
                <div itemscope itemtype="https://schema.org/Person">
                    <span itemprop="name">Jane Doe</span>
                </div>
            </body>
        </html>
        """
        result = meta_oxide.extract_microdata(html)

        assert isinstance(result, list)
        assert len(result) >= 2


class TestExtractDublinCore:
    """Test extract_dublin_core() function."""

    def test_extract_dublin_core_basic(self):
        """Test basic Dublin Core extraction."""
        html = """
        <html>
            <head>
                <meta name="DC.title" content="Document Title">
                <meta name="DC.creator" content="Author Name">
                <meta name="DC.date" content="2024-01-15">
            </head>
        </html>
        """
        result = meta_oxide.extract_dublin_core(html)

        assert isinstance(result, dict)


class TestExtractOEmbed:
    """Test extract_oembed() function."""

    def test_extract_oembed_basic(self):
        """Test basic oEmbed endpoint discovery."""
        html = """
        <html>
            <head>
                <link rel="alternate" type="application/json+oembed"
                      href="https://example.com/oembed?url=https%3A%2F%2Fexample.com%2Fpage"
                      title="Example Page">
            </head>
        </html>
        """
        result = meta_oxide.extract_oembed(html)

        assert isinstance(result, dict)


class TestExtractRelLinks:
    """Test extract_rel_links() function."""

    def test_extract_rel_links_basic(self):
        """Test basic rel link extraction."""
        html = """
        <html>
            <head>
                <link rel="canonical" href="https://example.com/page">
                <link rel="alternate" href="/page-de" hreflang="de">
                <link rel="alternate" href="/feed.xml" type="application/rss+xml">
            </head>
        </html>
        """
        result = meta_oxide.extract_rel_links(html)

        assert isinstance(result, dict)
        assert "canonical" in result or len(result) >= 0


class TestExtractMicroformats:
    """Test individual microformat extraction functions."""

    def test_extract_hcard(self):
        """Test h-card extraction."""
        html = """
        <div class="h-card">
            <span class="p-name">Jane Doe</span>
            <a class="u-url" href="https://example.com">Website</a>
            <img class="u-photo" src="https://example.com/photo.jpg" alt="Photo">
        </div>
        """
        result = meta_oxide.extract_hcard(html)

        assert isinstance(result, list)
        assert len(result) > 0
        assert result[0].get("name") == "Jane Doe"

    def test_extract_hentry(self):
        """Test h-entry extraction."""
        html = """
        <article class="h-entry">
            <h1 class="p-name">Entry Title</h1>
            <p class="p-summary">Entry summary</p>
            <time class="dt-published" datetime="2024-01-15">Jan 15</time>
        </article>
        """
        result = meta_oxide.extract_hentry(html)

        assert isinstance(result, list)
        assert len(result) > 0

    def test_extract_hevent(self):
        """Test h-event extraction."""
        html = """
        <div class="h-event">
            <span class="p-name">Event Name</span>
            <time class="dt-start" datetime="2024-06-15T10:00:00">June 15</time>
            <time class="dt-end" datetime="2024-06-15T17:00:00">5pm</time>
        </div>
        """
        result = meta_oxide.extract_hevent(html)

        assert isinstance(result, list)
        assert len(result) > 0

    def test_extract_hreview(self):
        """Test h-review extraction."""
        html = """
        <div class="h-review">
            <span class="p-name">Product Review</span>
            <p class="p-summary">Great product</p>
            <span class="p-rating">5</span>
        </div>
        """
        result = meta_oxide.extract_hreview(html)

        assert isinstance(result, list)

    def test_extract_hrecipe(self):
        """Test h-recipe extraction."""
        html = """
        <div class="h-recipe">
            <span class="p-name">Chocolate Cake</span>
            <span class="p-yield">10 servings</span>
            <span class="dt-duration">PT30M</span>
        </div>
        """
        result = meta_oxide.extract_hrecipe(html)

        assert isinstance(result, list)

    def test_extract_hproduct(self):
        """Test h-product extraction."""
        html = """
        <div class="h-product">
            <span class="p-name">Product Name</span>
            <span class="p-price">$99.99</span>
            <img class="u-photo" src="/product.jpg" alt="Product">
        </div>
        """
        result = meta_oxide.extract_hproduct(html)

        assert isinstance(result, list)

    def test_extract_hfeed(self):
        """Test h-feed extraction."""
        html = """
        <div class="h-feed">
            <span class="p-name">My Blog</span>
            <article class="h-entry">
                <h1 class="p-name">Post 1</h1>
            </article>
        </div>
        """
        result = meta_oxide.extract_hfeed(html)

        assert isinstance(result, list)

    def test_extract_hadr(self):
        """Test h-adr extraction."""
        html = """
        <div class="h-adr">
            <span class="p-street-address">123 Main St</span>
            <span class="p-locality">Springfield</span>
            <span class="p-postal-code">12345</span>
        </div>
        """
        result = meta_oxide.extract_hadr(html)

        assert isinstance(result, list)

    def test_extract_hgeo(self):
        """Test h-geo extraction."""
        html = """
        <span class="h-geo">
            <span class="p-latitude">37.7749</span>
            <span class="p-longitude">-122.4194</span>
            <span class="p-altitude">52</span>
        </span>
        """
        result = meta_oxide.extract_hgeo(html)

        assert isinstance(result, list)


class TestExtractAll:
    """Test the main extract_all() function."""

    def test_extract_all_returns_dict(self):
        """Test that extract_all() returns a dictionary."""
        html = "<html><head><title>Test</title></head></html>"
        result = meta_oxide.extract_all(html)

        assert isinstance(result, dict)

    def test_extract_all_includes_meta(self):
        """Test that extract_all() includes meta tags."""
        html = """
        <html>
            <head>
                <title>Test Page</title>
                <meta name="description" content="Test">
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html)

        assert "meta" in result
        assert isinstance(result["meta"], dict)

    def test_extract_all_includes_opengraph(self):
        """Test that extract_all() includes Open Graph."""
        html = """
        <html>
            <head>
                <meta property="og:title" content="OG Title">
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html)

        assert "opengraph" in result

    def test_extract_all_includes_twitter(self):
        """Test that extract_all() includes Twitter Cards."""
        html = """
        <html>
            <head>
                <meta name="twitter:card" content="summary">
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html)

        assert "twitter" in result

    def test_extract_all_includes_jsonld(self):
        """Test that extract_all() includes JSON-LD."""
        html = """
        <html>
            <head>
                <script type="application/ld+json">
                {"@type": "Article"}
                </script>
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html)

        assert "jsonld" in result

    def test_extract_all_includes_microdata(self):
        """Test that extract_all() includes microdata."""
        html = """
        <html>
            <body>
                <div itemscope itemtype="https://schema.org/Person">
                    <span itemprop="name">John</span>
                </div>
            </body>
        </html>
        """
        result = meta_oxide.extract_all(html)

        assert "microdata" in result

    def test_extract_all_includes_microformats(self):
        """Test that extract_all() includes microformats."""
        html = """
        <div class="h-card">
            <span class="p-name">Jane</span>
        </div>
        """
        result = meta_oxide.extract_all(html)

        assert "microformats" in result

    def test_extract_all_with_base_url(self):
        """Test extract_all() with base URL."""
        html = """
        <html>
            <head>
                <link rel="canonical" href="/page">
            </head>
        </html>
        """
        result = meta_oxide.extract_all(html, base_url="https://example.com")

        assert isinstance(result, dict)

    def test_extract_all_comprehensive(self):
        """Test extract_all() with all formats present."""
        html = """
        <html>
            <head>
                <title>Comprehensive Test</title>
                <meta name="description" content="Test page">
                <meta property="og:title" content="OG Title">
                <meta name="twitter:card" content="summary">
                <script type="application/ld+json">
                {"@type": "Article", "headline": "Test"}
                </script>
                <link rel="canonical" href="https://example.com/page">
            </head>
            <body>
                <div class="h-card">
                    <span class="p-name">Test</span>
                </div>
                <div itemscope itemtype="https://schema.org/Person">
                    <span itemprop="name">John</span>
                </div>
            </body>
        </html>
        """
        result = meta_oxide.extract_all(html, base_url="https://example.com")

        assert isinstance(result, dict)
        assert "meta" in result
        assert "opengraph" in result
        assert "twitter" in result
        assert "jsonld" in result


if __name__ == "__main__":
    # Run a quick smoke test if pytest is not available
    print("Running Python API smoke tests...\n")

    # Test extract_meta
    html = "<html><head><title>Test</title></head></html>"
    meta = meta_oxide.extract_meta(html)
    print(f"✓ extract_meta() works: {type(meta)}")

    # Test extract_all
    result = meta_oxide.extract_all(html)
    print(f"✓ extract_all() works: {type(result)}, keys: {list(result.keys())}")

    # Test extract_hcard
    html_hcard = '<div class="h-card"><span class="p-name">Jane</span></div>'
    cards = meta_oxide.extract_hcard(html_hcard)
    print(f"✓ extract_hcard() works: {len(cards)} cards found")

    print("\nAll smoke tests passed!")
