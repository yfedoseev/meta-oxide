#!/usr/bin/env python3
"""
Example: Extracting LocalBusiness JSON-LD data

This example demonstrates how to extract LocalBusiness structured data
from HTML pages. LocalBusiness is crucial for local SEO and Google Business Profile.
"""


import meta_oxide


def example_basic_business():
    """Extract a basic LocalBusiness"""
    print("=" * 60)
    print("Example 1: Basic LocalBusiness")
    print("=" * 60)

    html = """
    <html>
    <head>
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "LocalBusiness",
            "name": "Joe's Coffee Shop",
            "description": "Best artisan coffee in downtown",
            "telephone": "+1-555-123-4567",
            "email": "info@joescoffee.com",
            "url": "https://joescoffee.com"
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    businesses = meta_oxide.extract_jsonld(html)
    for business in businesses:
        print(f"Business Name: {business.get('name')}")
        print(f"Description: {business.get('description')}")
        print(f"Phone: {business.get('telephone')}")
        print(f"Email: {business.get('email')}")
        print(f"Website: {business.get('url')}")
    print()


def example_restaurant_with_details():
    """Extract a Restaurant with full details"""
    print("=" * 60)
    print("Example 2: Restaurant with Full Details")
    print("=" * 60)

    html = """
    <html>
    <head>
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "Restaurant",
            "name": "Bella Italia Ristorante",
            "description": "Authentic Italian cuisine in the heart of the city",
            "image": "https://example.com/bella-italia.jpg",
            "address": {
                "@type": "PostalAddress",
                "streetAddress": "123 Main Street",
                "addressLocality": "San Francisco",
                "addressRegion": "CA",
                "postalCode": "94102",
                "addressCountry": "US"
            },
            "telephone": "+1-415-555-1234",
            "email": "reservations@bellaitalia.com",
            "url": "https://bellaitalia.com",
            "geo": {
                "@type": "GeoCoordinates",
                "latitude": "37.7749",
                "longitude": "-122.4194"
            },
            "servesCuisine": ["Italian", "Mediterranean"],
            "priceRange": "$$$",
            "openingHoursSpecification": [
                {
                    "@type": "OpeningHoursSpecification",
                    "dayOfWeek": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"],
                    "opens": "17:00",
                    "closes": "23:00"
                },
                {
                    "@type": "OpeningHoursSpecification",
                    "dayOfWeek": ["Saturday", "Sunday"],
                    "opens": "12:00",
                    "closes": "23:00"
                }
            ],
            "aggregateRating": {
                "@type": "AggregateRating",
                "ratingValue": "4.8",
                "reviewCount": "342"
            }
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    businesses = meta_oxide.extract_jsonld(html)
    for business in businesses:
        print(f"Restaurant Name: {business.get('name')}")
        print(f"Type: {business.get('@type')}")
        print(f"Cuisine: {business.get('servesCuisine')}")
        print(f"Price Range: {business.get('priceRange')}")
        print(f"Phone: {business.get('telephone')}")
        print(f"Rating: {business.get('aggregateRating')}")
        print(f"Address: {business.get('address')}")
        print(f"Opening Hours: {business.get('openingHoursSpecification')}")
    print()


def example_store_with_reviews():
    """Extract a Store with customer reviews"""
    print("=" * 60)
    print("Example 3: Store with Customer Reviews")
    print("=" * 60)

    html = """
    <html>
    <head>
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "Store",
            "name": "Tech Gadget Store",
            "description": "Your one-stop shop for electronics",
            "address": {
                "@type": "PostalAddress",
                "streetAddress": "456 Tech Avenue",
                "addressLocality": "Austin",
                "addressRegion": "TX",
                "postalCode": "78701"
            },
            "telephone": "+1-512-555-9999",
            "aggregateRating": {
                "@type": "AggregateRating",
                "ratingValue": "4.5",
                "reviewCount": "89"
            },
            "review": [
                {
                    "@type": "Review",
                    "author": {
                        "@type": "Person",
                        "name": "Sarah Johnson"
                    },
                    "reviewRating": {
                        "@type": "Rating",
                        "ratingValue": "5"
                    },
                    "reviewBody": "Excellent service and great prices!"
                },
                {
                    "@type": "Review",
                    "author": {
                        "@type": "Person",
                        "name": "Mike Chen"
                    },
                    "reviewRating": {
                        "@type": "Rating",
                        "ratingValue": "4"
                    },
                    "reviewBody": "Good selection, helpful staff."
                }
            ]
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    businesses = meta_oxide.extract_jsonld(html)
    for business in businesses:
        print(f"Store Name: {business.get('name')}")
        print(f"Phone: {business.get('telephone')}")
        print(f"Aggregate Rating: {business.get('aggregateRating')}")
        print(f"Reviews: {business.get('review')}")
    print()


def example_extract_all_integration():
    """Extract LocalBusiness using extract_all()"""
    print("=" * 60)
    print("Example 4: Using extract_all() for Complete Data")
    print("=" * 60)

    html = """
    <html>
    <head>
        <title>Local Business Page</title>
        <meta name="description" content="Visit our coffee shop">
        <meta property="og:title" content="Joe's Coffee">
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "CafeOrCoffeeShop",
            "name": "Joe's Coffee House",
            "address": {
                "@type": "PostalAddress",
                "streetAddress": "789 Coffee Lane"
            },
            "telephone": "+1-555-COFFEE-1"
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    # Extract all metadata including LocalBusiness
    data = meta_oxide.extract_all(html)

    print("Meta Tags:")
    print(f"  Title: {data.get('meta', {}).get('title')}")
    print(f"  Description: {data.get('meta', {}).get('description')}")
    print()

    print("Open Graph:")
    print(f"  OG Title: {data.get('opengraph', {}).get('title')}")
    print()

    print("JSON-LD LocalBusiness:")
    for business in data.get("jsonld", []):
        if business.get("@type") in ["LocalBusiness", "CafeOrCoffeeShop", "Restaurant", "Store"]:
            print(f"  Business Type: {business.get('@type')}")
            print(f"  Name: {business.get('name')}")
            print(f"  Phone: {business.get('telephone')}")
    print()


def example_multiple_businesses():
    """Extract multiple LocalBusiness objects from a page"""
    print("=" * 60)
    print("Example 5: Multiple Businesses on One Page")
    print("=" * 60)

    html = """
    <html>
    <head>
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@graph": [
                {
                    "@type": "Restaurant",
                    "name": "Pizza Paradise",
                    "servesCuisine": ["Italian", "Pizza"],
                    "telephone": "+1-555-PIZZA-00"
                },
                {
                    "@type": "Restaurant",
                    "name": "Sushi Heaven",
                    "servesCuisine": ["Japanese", "Sushi"],
                    "telephone": "+1-555-SUSHI-1"
                }
            ]
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    businesses = meta_oxide.extract_jsonld(html)
    print(f"Found {len(businesses)} businesses:")
    for i, business in enumerate(businesses, 1):
        print(f"\n  Business {i}:")
        print(f"    Name: {business.get('name')}")
        print(f"    Type: {business.get('@type')}")
        print(f"    Cuisine: {business.get('servesCuisine')}")
        print(f"    Phone: {business.get('telephone')}")
    print()


if __name__ == "__main__":
    print("\n" + "=" * 60)
    print("LocalBusiness JSON-LD Extraction Examples")
    print("=" * 60 + "\n")

    example_basic_business()
    example_restaurant_with_details()
    example_store_with_reviews()
    example_extract_all_integration()
    example_multiple_businesses()

    print("=" * 60)
    print("All examples completed successfully!")
    print("=" * 60)
