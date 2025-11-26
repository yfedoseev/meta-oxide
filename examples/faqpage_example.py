#!/usr/bin/env python3
"""
Example: Extracting FAQPage JSON-LD data

This example demonstrates how to extract FAQPage structured data
from HTML using meta_oxide. FAQPage is commonly used for FAQ sections
and can improve search engine visibility with rich results.
"""

import meta_oxide

# Sample HTML with FAQPage JSON-LD
html = """
<!DOCTYPE html>
<html>
<head>
    <title>Frequently Asked Questions - Example Corp</title>
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "FAQPage",
        "name": "Customer Support FAQ",
        "description": "Common questions about our products and services",
        "url": "https://example.com/faq",
        "datePublished": "2024-01-15",
        "author": {
            "@type": "Organization",
            "name": "Example Corp",
            "url": "https://example.com"
        },
        "mainEntity": [
            {
                "@type": "Question",
                "name": "What is your return policy?",
                "acceptedAnswer": {
                    "@type": "Answer",
                    "text": "We accept returns within 30 days of purchase for a full refund. Items must be in original condition with tags attached."
                }
            },
            {
                "@type": "Question",
                "name": "How long does shipping take?",
                "acceptedAnswer": {
                    "@type": "Answer",
                    "text": "Standard shipping takes 5-7 business days. Express shipping takes 2-3 business days. Free shipping is available on orders over $50."
                }
            },
            {
                "@type": "Question",
                "name": "Do you ship internationally?",
                "acceptedAnswer": {
                    "@type": "Answer",
                    "text": "Yes, we ship to over 50 countries worldwide. International shipping times vary by destination."
                }
            },
            {
                "@type": "Question",
                "name": "How can I track my order?",
                "acceptedAnswer": {
                    "@type": "Answer",
                    "text": "Once your order ships, you will receive a tracking number via email. You can use this to track your package on our website or the carrier's website."
                }
            }
        ]
    }
    </script>
</head>
<body>
    <h1>Frequently Asked Questions</h1>
    <div class="faq-content">
        <!-- FAQ content here -->
    </div>
</body>
</html>
"""


def main():
    print("=" * 70)
    print("FAQPage JSON-LD Extraction Example")
    print("=" * 70)
    print()

    # Extract JSON-LD data
    jsonld_objects = meta_oxide.extract_jsonld(html)

    print(f"Found {len(jsonld_objects)} JSON-LD object(s)")
    print()

    for i, obj in enumerate(jsonld_objects, 1):
        print(f"Object {i}:")
        print(f"  Type: {obj.get('@type')}")

        if obj.get("@type") == "FAQPage":
            print(f"  Name: {obj.get('name')}")
            print(f"  Description: {obj.get('description')}")
            print(f"  URL: {obj.get('url')}")
            print(f"  Date Published: {obj.get('datePublished')}")

            # Author information
            if "author" in obj:
                author = obj["author"]
                if isinstance(author, str):
                    print(f"  Author: {author}")
                else:
                    author_str = str(author)
                    if "@type" in author_str:
                        print("  Author Type: Organization")
                        if "name" in author_str:
                            print("  Author: (see full object)")

            # Questions count
            main_entity = str(obj.get("mainEntity", ""))
            if "Question" in main_entity:
                # Count occurrences of "@type": "Question"
                import re

                question_count = len(re.findall(r'"@type"\s*:\s*"Question"', main_entity))
                print(f"  Number of Questions: {question_count}")

        print()

    # Also demonstrate extract_all()
    print("-" * 70)
    print("Using extract_all() - extracts FAQPage with other metadata")
    print("-" * 70)
    print()

    all_data = meta_oxide.extract_all(html)

    print("Available data types:")
    for key in all_data.keys():
        print(f"  - {key}")

    print()

    if "jsonld" in all_data and len(all_data["jsonld"]) > 0:
        faq = all_data["jsonld"][0]
        print("FAQPage from extract_all():")
        print(f"  Type: {faq.get('@type')}")
        print(f"  Name: {faq.get('name')}")

        # Show meta tags for context
        if "meta" in all_data:
            meta = all_data["meta"]
            print()
            print("Related meta tags:")
            print(f"  Title: {meta.get('title')}")
            print(f"  Description: {meta.get('description')}")

    print()
    print("=" * 70)
    print("Benefits of FAQPage Schema:")
    print("=" * 70)
    print(
        """
1. Rich Results: FAQs can appear directly in Google search results
2. Voice Search: Helps voice assistants provide direct answers
3. Featured Snippets: Increases chances of appearing in featured snippets
4. User Experience: Provides clear, structured information to searchers
5. Click-Through Rate: Enhanced visibility can improve CTR
    """
    )


if __name__ == "__main__":
    main()
