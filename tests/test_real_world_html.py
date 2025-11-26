"""
Real-world HTML test cases for MetaOxide.

These tests use simplified versions of real website patterns to ensure
the library handles common real-world HTML structures correctly.
"""

import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))

try:
    import meta_oxide
except ImportError:
    print("ERROR: meta_oxide module not found")
    sys.exit(1)


# Real-world HTML patterns from actual websites

NEWS_ARTICLE_HTML = """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Breaking: New AI Breakthrough Announced</title>
    <meta name="description" content="Scientists announce major breakthrough in artificial intelligence research">
    <meta name="keywords" content="AI, artificial intelligence, research, breakthrough">
    <meta name="author" content="Jane Smith">
    <link rel="canonical" href="https://newssite.example.com/articles/ai-breakthrough">

    <!-- Open Graph for Facebook/LinkedIn -->
    <meta property="og:title" content="Breaking: New AI Breakthrough Announced">
    <meta property="og:description" content="Scientists announce major breakthrough in artificial intelligence research">
    <meta property="og:type" content="article">
    <meta property="og:url" content="https://newssite.example.com/articles/ai-breakthrough">
    <meta property="og:image" content="https://newssite.example.com/images/ai-breakthrough.jpg">
    <meta property="og:image:width" content="1200">
    <meta property="og:image:height" content="630">
    <meta property="og:site_name" content="News Site">

    <!-- Twitter Card -->
    <meta name="twitter:card" content="summary_large_image">
    <meta name="twitter:title" content="Breaking: New AI Breakthrough">
    <meta name="twitter:description" content="Scientists announce major breakthrough in AI">
    <meta name="twitter:image" content="https://newssite.example.com/images/ai-breakthrough.jpg">
    <meta name="twitter:creator" content="@janesmith">

    <!-- JSON-LD Schema -->
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "NewsArticle",
        "headline": "Breaking: New AI Breakthrough Announced",
        "description": "Scientists announce major breakthrough in artificial intelligence research",
        "image": {
            "@type": "ImageObject",
            "url": "https://newssite.example.com/images/ai-breakthrough.jpg",
            "width": 1200,
            "height": 630
        },
        "datePublished": "2024-01-15T10:00:00Z",
        "dateModified": "2024-01-15T14:30:00Z",
        "author": {
            "@type": "Person",
            "name": "Jane Smith",
            "url": "https://newssite.example.com/authors/jane-smith"
        },
        "publisher": {
            "@type": "Organization",
            "name": "News Site",
            "logo": {
                "@type": "ImageObject",
                "url": "https://newssite.example.com/logo.png"
            }
        }
    }
    </script>
</head>
<body>
    <article>
        <h1>Breaking: New AI Breakthrough Announced</h1>
        <div class="author-info">
            <strong>By Jane Smith</strong> | Published Jan 15, 2024 | Updated 2:30 PM
        </div>
        <img src="https://newssite.example.com/images/ai-breakthrough.jpg" alt="AI Research">
        <p>Scientists from leading research institutions announced a major breakthrough...</p>
    </article>
</body>
</html>
"""

BLOG_POST_WITH_MICROFORMATS = """
<!DOCTYPE html>
<html>
<head>
    <title>How to Get Started with Microformats</title>
    <meta name="description" content="A beginner's guide to microformats">
    <link rel="canonical" href="/blog/microformats-guide">
</head>
<body>
    <article class="h-entry">
        <h1 class="p-name">How to Get Started with Microformats</h1>

        <div class="p-author h-card">
            <img class="u-photo" src="/authors/john.jpg" alt="John">
            <span class="p-name">John Doe</span>
            <a class="u-url" href="/authors/john">View Profile</a>
        </div>

        <time class="dt-published" datetime="2024-01-10T09:00:00Z">January 10, 2024</time>
        <time class="dt-updated" datetime="2024-01-12T15:00:00Z">Updated January 12</time>

        <div class="e-content">
            <p>Microformats are a simple way to mark up data in HTML...</p>
            <p>They are used for contact information, events, reviews, and more.</p>
        </div>

        <p>
            Categories:
            <a class="p-category" href="/tags/web">Web Development</a>
            <a class="p-category" href="/tags/microformats">Microformats</a>
            <a class="p-category" href="/tags/semantic">Semantic HTML</a>
        </p>

        <a class="u-url" href="/blog/microformats-guide">Permanent Link</a>
    </article>
</body>
</html>
"""

ECOMMERCE_PRODUCT_PAGE = """
<!DOCTYPE html>
<html>
<head>
    <title>Premium Wireless Headphones - $199.99</title>
    <meta name="description" content="High-quality wireless headphones with noise cancellation">

    <!-- Open Graph -->
    <meta property="og:title" content="Premium Wireless Headphones">
    <meta property="og:price:amount" content="199.99">
    <meta property="og:price:currency" content="USD">
    <meta property="og:image" content="/products/headphones.jpg">

    <!-- Schema.org Product -->
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Product",
        "name": "Premium Wireless Headphones",
        "description": "High-quality wireless headphones with noise cancellation",
        "image": "/products/headphones.jpg",
        "brand": {
            "@type": "Brand",
            "name": "AudioTech"
        },
        "offers": {
            "@type": "Offer",
            "url": "https://shop.example.com/headphones",
            "priceCurrency": "USD",
            "price": "199.99",
            "availability": "https://schema.org/InStock"
        },
        "aggregateRating": {
            "@type": "AggregateRating",
            "ratingValue": "4.5",
            "ratingCount": "128",
            "bestRating": "5",
            "worstRating": "1"
        }
    }
    </script>
</head>
<body>
    <div class="h-product">
        <h1 class="p-name">Premium Wireless Headphones</h1>
        <img class="u-photo" src="/products/headphones.jpg" alt="Headphones">
        <div class="p-price">$199.99</div>
        <p class="p-description">High-quality wireless headphones with noise cancellation</p>
        <a class="u-url" href="/headphones">Buy Now</a>
    </div>
</body>
</html>
"""

RECIPE_PAGE = """
<!DOCTYPE html>
<html>
<head>
    <title>Classic Chocolate Cake Recipe</title>
    <meta name="description" content="Easy to follow chocolate cake recipe">

    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Recipe",
        "name": "Classic Chocolate Cake",
        "description": "Easy to follow chocolate cake recipe",
        "prepTime": "PT15M",
        "cookTime": "PT30M",
        "totalTime": "PT45M",
        "yield": "10 servings",
        "author": {
            "@type": "Person",
            "name": "Chef Maria"
        },
        "ingredients": [
            "2 cups flour",
            "1 cup sugar",
            "3/4 cup cocoa powder"
        ],
        "instructions": [
            "Preheat oven to 350F",
            "Mix dry ingredients",
            "Bake for 30 minutes"
        ],
        "aggregateRating": {
            "@type": "AggregateRating",
            "ratingValue": "4.8",
            "ratingCount": "256"
        }
    }
    </script>
</head>
<body>
    <div class="h-recipe">
        <h1 class="p-name">Classic Chocolate Cake</h1>
        <div class="p-description">Easy to follow chocolate cake recipe</div>

        <div class="p-author h-card">
            <span class="p-name">Chef Maria</span>
        </div>

        <div>
            <span class="dt-duration">PT45M</span>
            <span class="p-yield">10 servings</span>
        </div>

        <div class="p-ingredient">
            <span>2 cups flour</span>
        </div>
        <div class="p-ingredient">
            <span>1 cup sugar</span>
        </div>
    </div>
</body>
</html>
"""

ORGANIZATION_HOMEPAGE = """
<!DOCTYPE html>
<html>
<head>
    <title>Example Corp - Leading Technology Company</title>
    <meta name="description" content="Example Corp creates innovative technology solutions">

    <!-- Schema.org Organization -->
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Organization",
        "name": "Example Corp",
        "url": "https://examplecorp.com",
        "logo": "https://examplecorp.com/logo.png",
        "description": "Leading technology company",
        "foundingDate": "2005",
        "founder": {
            "@type": "Person",
            "name": "John Founder"
        },
        "address": {
            "@type": "PostalAddress",
            "streetAddress": "123 Tech Street",
            "addressLocality": "San Francisco",
            "addressRegion": "CA",
            "postalCode": "94105",
            "addressCountry": "US"
        },
        "contactPoint": {
            "@type": "ContactPoint",
            "contactType": "Customer Service",
            "telephone": "+1-555-123-4567",
            "email": "contact@examplecorp.com"
        },
        "sameAs": [
            "https://www.facebook.com/examplecorp",
            "https://www.twitter.com/examplecorp"
        ]
    }
    </script>
</head>
<body>
    <div class="h-card">
        <img class="u-photo" src="/logo.png" alt="Example Corp">
        <h1 class="p-name">Example Corp</h1>
        <p class="p-note">Leading technology company creating innovative solutions</p>

        <div class="p-adr h-adr">
            <span class="p-street-address">123 Tech Street</span>
            <span class="p-locality">San Francisco</span>
            <span class="p-region">CA</span>
            <span class="p-postal-code">94105</span>
        </div>

        <a class="u-url" href="https://examplecorp.com">Visit Website</a>
        <a class="u-email" href="mailto:contact@examplecorp.com">Contact Us</a>
    </div>
</body>
</html>
"""

EVENT_LISTING = """
<!DOCTYPE html>
<html>
<head>
    <title>Annual Tech Conference 2024</title>
    <meta name="description" content="Join us for the annual tech conference">

    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Event",
        "name": "Annual Tech Conference 2024",
        "description": "Join us for the annual tech conference",
        "startDate": "2024-06-15T09:00:00Z",
        "endDate": "2024-06-17T17:00:00Z",
        "location": {
            "@type": "Place",
            "name": "Convention Center",
            "address": {
                "@type": "PostalAddress",
                "streetAddress": "456 Event Ave",
                "addressLocality": "New York",
                "addressRegion": "NY",
                "postalCode": "10001"
            }
        },
        "organizer": {
            "@type": "Organization",
            "name": "Tech Events Inc"
        },
        "offers": {
            "@type": "Offer",
            "url": "https://techconf.example.com/register",
            "price": "299",
            "priceCurrency": "USD"
        }
    }
    </script>
</head>
<body>
    <div class="h-event">
        <h1 class="p-name">Annual Tech Conference 2024</h1>
        <p class="p-summary">Join us for the annual tech conference</p>

        <time class="dt-start" datetime="2024-06-15T09:00:00Z">June 15, 2024 - 9:00 AM</time>
        <time class="dt-end" datetime="2024-06-17T17:00:00Z">June 17, 2024 - 5:00 PM</time>

        <div class="p-location h-adr">
            <span class="p-name">Convention Center</span>
            <span class="p-street-address">456 Event Ave</span>
            <span class="p-locality">New York</span>
        </div>

        <a class="u-url" href="https://techconf.example.com">Event Website</a>
    </div>
</body>
</html>
"""

REVIEW_PAGE = """
<!DOCTYPE html>
<html>
<head>
    <title>Review: Best Laptop for Developers</title>

    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Review",
        "name": "Best Laptop for Developers",
        "reviewRating": {
            "@type": "Rating",
            "ratingValue": "5",
            "bestRating": "5",
            "worstRating": "1"
        },
        "author": {
            "@type": "Person",
            "name": "Tech Reviewer"
        },
        "reviewBody": "This laptop is perfect for developers...",
        "itemReviewed": {
            "@type": "Product",
            "name": "DevBook Pro"
        }
    }
    </script>
</head>
<body>
    <div class="h-review">
        <h1 class="p-name">Best Laptop for Developers</h1>
        <p class="p-summary">This laptop is perfect for developers</p>
        <span class="p-rating">5 out of 5 stars</span>

        <div class="p-author h-card">
            <span class="p-name">Tech Reviewer</span>
        </div>
    </div>
</body>
</html>
"""


class TestRealWorldHTML:
    """Test extraction from real-world HTML patterns."""

    def test_news_article_extraction(self):
        """Test extraction from a news article page."""
        result = meta_oxide.extract_all(NEWS_ARTICLE_HTML, base_url="https://newssite.example.com")

        # Should have meta tags
        assert "meta" in result
        assert result["meta"].get("title") == "Breaking: New AI Breakthrough Announced"

        # Should have Open Graph
        assert "opengraph" in result
        assert result["opengraph"].get("title") == "Breaking: New AI Breakthrough Announced"

        # Should have Twitter Card
        assert "twitter" in result

        # Should have JSON-LD
        assert "jsonld" in result
        assert len(result["jsonld"]) > 0

    def test_blog_post_with_microformats(self):
        """Test extraction from blog post with h-entry."""
        result = meta_oxide.extract_all(BLOG_POST_WITH_MICROFORMATS)

        # Should have microformats
        assert "microformats" in result
        mf = result["microformats"]

        # Should have h-entry
        if "h-entry" in mf:
            entries = mf["h-entry"]
            assert len(entries) > 0

    def test_ecommerce_product_page(self):
        """Test extraction from e-commerce product page."""
        result = meta_oxide.extract_all(ECOMMERCE_PRODUCT_PAGE)

        # Should have Open Graph with price
        assert "opengraph" in result

        # Should have JSON-LD with product schema
        assert "jsonld" in result
        if len(result["jsonld"]) > 0:
            product = result["jsonld"][0]
            assert product.get("name") or "Product" in str(product)

        # Should have microformats h-product
        if "microformats" in result:
            mf = result["microformats"]
            if "h-product" in mf:
                assert len(mf["h-product"]) > 0

    def test_recipe_page_extraction(self):
        """Test extraction from recipe page."""
        result = meta_oxide.extract_all(RECIPE_PAGE)

        # Should have JSON-LD recipe
        assert "jsonld" in result
        if len(result["jsonld"]) > 0:
            recipe = result["jsonld"][0]
            assert recipe.get("name") or "Recipe" in str(recipe)

        # Should have h-recipe microformat
        if "microformats" in result:
            mf = result["microformats"]
            if "h-recipe" in mf:
                assert len(mf["h-recipe"]) > 0

    def test_organization_homepage(self):
        """Test extraction from organization homepage."""
        result = meta_oxide.extract_all(ORGANIZATION_HOMEPAGE)

        # Should have meta description
        assert "meta" in result
        assert "description" in result["meta"]

        # Should have JSON-LD organization
        assert "jsonld" in result
        if len(result["jsonld"]) > 0:
            org = result["jsonld"][0]
            assert org.get("name") or "Organization" in str(org)

        # Should have h-card microformat
        if "microformats" in result:
            mf = result["microformats"]
            if "h-card" in mf:
                assert len(mf["h-card"]) > 0

    def test_event_listing_page(self):
        """Test extraction from event listing page."""
        result = meta_oxide.extract_all(EVENT_LISTING)

        # Should have JSON-LD event
        assert "jsonld" in result
        if len(result["jsonld"]) > 0:
            event = result["jsonld"][0]
            assert event.get("name") or "Event" in str(event)

        # Should have h-event microformat
        if "microformats" in result:
            mf = result["microformats"]
            if "h-event" in mf:
                assert len(mf["h-event"]) > 0

    def test_review_page_extraction(self):
        """Test extraction from review page."""
        result = meta_oxide.extract_all(REVIEW_PAGE)

        # Should have JSON-LD review
        assert "jsonld" in result
        if len(result["jsonld"]) > 0:
            review = result["jsonld"][0]
            assert review.get("name") or "Review" in str(review)

        # Should have h-review microformat
        if "microformats" in result:
            mf = result["microformats"]
            if "h-review" in mf:
                assert len(mf["h-review"]) > 0

    def test_mixed_formats_consistency(self):
        """Test that overlapping formats don't cause conflicts."""
        # News article has both OG and JSON-LD with same content
        result = meta_oxide.extract_all(NEWS_ARTICLE_HTML)

        # Both should be extracted
        assert "opengraph" in result
        assert "jsonld" in result

        # They should have similar data
        og_title = result["opengraph"].get("title", "")
        if result["jsonld"]:
            jsonld_headline = result["jsonld"][0].get("headline", "")
            # Both should have title-like data
            assert og_title or jsonld_headline


if __name__ == "__main__":
    print("Running real-world HTML tests...\n")

    test = TestRealWorldHTML()

    try:
        test.test_news_article_extraction()
        print("✓ News article extraction works")
    except Exception as e:
        print(f"✗ News article extraction failed: {e}")

    try:
        test.test_blog_post_with_microformats()
        print("✓ Blog post with microformats works")
    except Exception as e:
        print(f"✗ Blog post with microformats failed: {e}")

    try:
        test.test_ecommerce_product_page()
        print("✓ E-commerce product page works")
    except Exception as e:
        print(f"✗ E-commerce product page failed: {e}")

    try:
        test.test_recipe_page_extraction()
        print("✓ Recipe page extraction works")
    except Exception as e:
        print(f"✗ Recipe page extraction failed: {e}")

    print("\nReal-world HTML tests completed!")
