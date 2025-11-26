#!/usr/bin/env python3
"""
Demo script showing BreadcrumbList JSON-LD extraction

This demonstrates the new BreadcrumbList type support in meta_oxide.
"""

import meta_oxide

# Example 1: E-commerce breadcrumb
ecommerce_html = """
<html>
<head>
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "BreadcrumbList",
        "itemListElement": [
            {
                "@type": "ListItem",
                "position": 1,
                "name": "Home",
                "item": "https://shop.example.com"
            },
            {
                "@type": "ListItem",
                "position": 2,
                "name": "Electronics",
                "item": "https://shop.example.com/electronics"
            },
            {
                "@type": "ListItem",
                "position": 3,
                "name": "Laptops",
                "item": "https://shop.example.com/electronics/laptops"
            },
            {
                "@type": "ListItem",
                "position": 4,
                "name": "Gaming Laptops"
            }
        ]
    }
    </script>
</head>
<body>
    <h1>Gaming Laptops</h1>
</body>
</html>
"""

print("Example 1: E-commerce Breadcrumb")
print("=" * 50)
objects = meta_oxide.extract_jsonld(ecommerce_html)
breadcrumb = objects[0]
print(f"Type: {breadcrumb['@type']}")
print(f"Number of items: {len(breadcrumb['itemListElement'])}")
print("\nBreadcrumb trail:")
for item in breadcrumb["itemListElement"]:
    position = item["position"]
    name = item["name"]
    url = item.get("item", "(current page)")
    print(f"  {position}. {name} -> {url}")

# Example 2: Documentation breadcrumb with all fields
documentation_html = """
<html>
<head>
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "BreadcrumbList",
        "name": "Documentation Navigation",
        "numberOfItems": 3,
        "itemListElement": [
            {
                "@type": "ListItem",
                "position": 1,
                "name": "Docs",
                "item": "https://docs.example.com"
            },
            {
                "@type": "ListItem",
                "position": 2,
                "name": "API Reference",
                "item": "https://docs.example.com/api"
            },
            {
                "@type": "ListItem",
                "position": 3,
                "name": "Authentication",
                "item": "https://docs.example.com/api/auth"
            }
        ]
    }
    </script>
</head>
<body>
    <h1>Authentication</h1>
</body>
</html>
"""

print("\n\nExample 2: Documentation Breadcrumb (with metadata)")
print("=" * 50)
objects = meta_oxide.extract_jsonld(documentation_html)
breadcrumb = objects[0]
print(f"Type: {breadcrumb['@type']}")
print(f"Name: {breadcrumb.get('name', 'N/A')}")
print(f"Number of Items: {breadcrumb.get('numberOfItems', 'N/A')}")
print("\nBreadcrumb trail:")
for item in breadcrumb["itemListElement"]:
    print(f"  {item['position']}. {item['name']} -> {item['item']}")

# Example 3: Using extract_all() to get breadcrumb with other data
combined_html = """
<html>
<head>
    <title>Gaming Laptop - TechShop</title>
    <meta property="og:title" content="Best Gaming Laptop 2024">
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@graph": [
            {
                "@type": "BreadcrumbList",
                "itemListElement": [
                    {
                        "@type": "ListItem",
                        "position": 1,
                        "name": "Home",
                        "item": "https://shop.example.com"
                    },
                    {
                        "@type": "ListItem",
                        "position": 2,
                        "name": "Products",
                        "item": "https://shop.example.com/products"
                    }
                ]
            },
            {
                "@type": "Product",
                "name": "Gaming Laptop Pro",
                "sku": "LAPTOP-001"
            }
        ]
    }
    </script>
</head>
<body></body>
</html>
"""

print("\n\nExample 3: Combined extraction with extract_all()")
print("=" * 50)
data = meta_oxide.extract_all(combined_html)
print(f"Title: {data['meta']['title']}")
print(f"OG Title: {data['opengraph']['title']}")
print(f"\nJSON-LD objects found: {len(data['jsonld'])}")
for obj in data["jsonld"]:
    obj_type = obj["@type"]
    print(f"  - {obj_type}")
    if obj_type == "BreadcrumbList":
        print(f"    Trail: {' > '.join([item['name'] for item in obj['itemListElement']])}")
    elif obj_type == "Product":
        print(f"    Name: {obj['name']}")
        print(f"    SKU: {obj['sku']}")

print("\n" + "=" * 50)
print("BreadcrumbList support successfully implemented!")
print("=" * 50)
