/**
 * MetaOxide C API Example
 *
 * This example demonstrates how to use the MetaOxide C API to extract
 * metadata from HTML documents.
 *
 * Build:
 *   gcc -I../include -L../target/release -o example c_api_example.c -lmeta_oxide -lpthread -ldl -lm
 *
 * Run:
 *   LD_LIBRARY_PATH=../target/release ./example
 */

#include "../include/meta_oxide.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// Pretty print JSON (simple indentation)
void print_json(const char* label, const char* json) {
    if (json == NULL) {
        printf("%s: (none)\n", label);
        return;
    }

    printf("%s:\n", label);
    printf("  %s\n", json);
}

// Example 1: Extract all metadata at once
void example_extract_all() {
    printf("\n=== Example 1: Extract All Metadata ===\n");

    const char* html =
        "<html>"
        "<head>"
        "  <title>MetaOxide Example Page</title>"
        "  <meta name=\"description\" content=\"A comprehensive metadata extraction example\">"
        "  <meta property=\"og:title\" content=\"Open Graph Title\">"
        "  <meta property=\"og:image\" content=\"https://example.com/image.jpg\">"
        "  <meta name=\"twitter:card\" content=\"summary_large_image\">"
        "  <meta name=\"twitter:creator\" content=\"@metaoxide\">"
        "  <script type=\"application/ld+json\">"
        "  {"
        "    \"@context\": \"https://schema.org\","
        "    \"@type\": \"Article\","
        "    \"headline\": \"How to Extract Metadata\","
        "    \"author\": {"
        "      \"@type\": \"Person\","
        "      \"name\": \"John Doe\""
        "    },"
        "    \"datePublished\": \"2025-01-15\""
        "  }"
        "  </script>"
        "  <meta name=\"DC.title\" content=\"Dublin Core Title\">"
        "  <meta name=\"DC.creator\" content=\"Jane Smith\">"
        "  <link rel=\"canonical\" href=\"https://example.com/article\">"
        "</head>"
        "<body>"
        "  <article class=\"h-entry\">"
        "    <h1 class=\"p-name\">Blog Post Title</h1>"
        "    <p class=\"p-summary\">A short summary of the blog post.</p>"
        "    <a class=\"u-url\" href=\"https://example.com/blog/post\">Permalink</a>"
        "  </article>"
        "</body>"
        "</html>";

    // Extract all metadata
    MetaOxideResult* result = meta_oxide_extract_all(html, "https://example.com");

    if (result == NULL) {
        fprintf(stderr, "Error extracting metadata: %s\n", meta_oxide_error_message());
        return;
    }

    // Print all extracted metadata
    print_json("Standard Meta Tags", result->meta);
    print_json("Open Graph", result->open_graph);
    print_json("Twitter Cards", result->twitter);
    print_json("JSON-LD", result->json_ld);
    print_json("Microformats", result->microformats);
    print_json("Dublin Core", result->dublin_core);
    print_json("rel-* Links", result->rel_links);

    // Clean up
    meta_oxide_result_free(result);
}

// Example 2: Extract specific metadata types
void example_extract_specific() {
    printf("\n=== Example 2: Extract Specific Metadata ===\n");

    const char* html =
        "<html>"
        "<head>"
        "  <title>Product Page</title>"
        "  <meta property=\"og:type\" content=\"product\">"
        "  <meta property=\"og:title\" content=\"Amazing Product\">"
        "  <meta property=\"og:price:amount\" content=\"29.99\">"
        "  <meta property=\"og:price:currency\" content=\"USD\">"
        "</head>"
        "</html>";

    // Extract only Open Graph metadata
    char* og = meta_oxide_extract_open_graph(html, NULL);
    if (og) {
        printf("Open Graph metadata:\n  %s\n", og);
        meta_oxide_string_free(og);
    }

    // Extract only Twitter metadata
    char* twitter = meta_oxide_extract_twitter(html, NULL);
    if (twitter) {
        printf("Twitter metadata:\n  %s\n", twitter);
        meta_oxide_string_free(twitter);
    }
}

// Example 3: Handle errors gracefully
void example_error_handling() {
    printf("\n=== Example 3: Error Handling ===\n");

    // Try to extract from NULL HTML (should fail)
    char* result = meta_oxide_extract_meta(NULL, NULL);

    if (result == NULL) {
        int error_code = meta_oxide_last_error();
        const char* error_msg = meta_oxide_error_message();
        printf("Expected error occurred:\n");
        printf("  Error code: %d\n", error_code);
        printf("  Error message: %s\n", error_msg);
    } else {
        printf("Unexpected: should have failed\n");
        meta_oxide_string_free(result);
    }
}

// Example 4: Extract JSON-LD structured data
void example_json_ld() {
    printf("\n=== Example 4: JSON-LD Structured Data ===\n");

    const char* html =
        "<html>"
        "<head>"
        "  <script type=\"application/ld+json\">"
        "  {"
        "    \"@context\": \"https://schema.org\","
        "    \"@type\": \"LocalBusiness\","
        "    \"name\": \"Example Restaurant\","
        "    \"address\": {"
        "      \"@type\": \"PostalAddress\","
        "      \"streetAddress\": \"123 Main St\","
        "      \"addressLocality\": \"Springfield\","
        "      \"postalCode\": \"12345\""
        "    },"
        "    \"telephone\": \"+1-555-1234\","
        "    \"openingHours\": [\"Mo-Sa 11:00-21:00\", \"Su 12:00-20:00\"]"
        "  }"
        "  </script>"
        "</head>"
        "</html>";

    char* jsonld = meta_oxide_extract_json_ld(html, NULL);
    if (jsonld) {
        printf("JSON-LD data:\n  %s\n", jsonld);
        meta_oxide_string_free(jsonld);
    }
}

// Example 5: Extract Microformats
void example_microformats() {
    printf("\n=== Example 5: Microformats ===\n");

    const char* html =
        "<html>"
        "<body>"
        "  <div class=\"h-card\">"
        "    <img class=\"u-photo\" src=\"https://example.com/photo.jpg\" alt=\"Photo\">"
        "    <a class=\"p-name u-url\" href=\"https://example.com\">Jane Doe</a>"
        "    <p class=\"p-org\">Acme Corp</p>"
        "    <p class=\"p-tel\">+1-555-9876</p>"
        "    <a class=\"u-email\" href=\"mailto:jane@example.com\">jane@example.com</a>"
        "  </div>"
        "</body>"
        "</html>";

    char* microformats = meta_oxide_extract_microformats(html, "https://example.com");
    if (microformats) {
        printf("Microformats data:\n  %s\n", microformats);
        meta_oxide_string_free(microformats);
    }
}

// Example 6: Extract Microdata
void example_microdata() {
    printf("\n=== Example 6: Microdata ===\n");

    const char* html =
        "<html>"
        "<body>"
        "  <div itemscope itemtype=\"https://schema.org/Movie\">"
        "    <h1 itemprop=\"name\">Avatar</h1>"
        "    <span itemprop=\"director\" itemscope itemtype=\"https://schema.org/Person\">"
        "      <span itemprop=\"name\">James Cameron</span>"
        "    </span>"
        "    <span itemprop=\"genre\">Science Fiction</span>"
        "    <a itemprop=\"trailer\" href=\"https://example.com/trailer\">Watch Trailer</a>"
        "  </div>"
        "</body>"
        "</html>";

    char* microdata = meta_oxide_extract_microdata(html, "https://example.com");
    if (microdata) {
        printf("Microdata:\n  %s\n", microdata);
        meta_oxide_string_free(microdata);
    }
}

// Example 7: Web App Manifest
void example_manifest() {
    printf("\n=== Example 7: Web App Manifest ===\n");

    const char* html =
        "<html>"
        "<head>"
        "  <link rel=\"manifest\" href=\"/manifest.json\">"
        "</head>"
        "</html>";

    char* manifest = meta_oxide_extract_manifest(html, "https://example.com");
    if (manifest) {
        printf("Manifest discovery:\n  %s\n", manifest);
        meta_oxide_string_free(manifest);
    }

    // Parse a manifest JSON
    const char* manifest_json =
        "{"
        "  \"name\": \"Example PWA\","
        "  \"short_name\": \"PWA\","
        "  \"start_url\": \"/\","
        "  \"display\": \"standalone\","
        "  \"icons\": ["
        "    {"
        "      \"src\": \"/icon-192.png\","
        "      \"sizes\": \"192x192\","
        "      \"type\": \"image/png\""
        "    }"
        "  ]"
        "}";

    char* parsed = meta_oxide_parse_manifest(manifest_json, "https://example.com");
    if (parsed) {
        printf("\nParsed manifest:\n  %s\n", parsed);
        meta_oxide_string_free(parsed);
    }
}

// Example 8: Base URL resolution
void example_base_url() {
    printf("\n=== Example 8: Base URL Resolution ===\n");

    const char* html =
        "<html>"
        "<head>"
        "  <link rel=\"canonical\" href=\"/articles/example\">"
        "  <meta property=\"og:image\" content=\"/images/featured.jpg\">"
        "</head>"
        "</html>";

    // Without base URL
    printf("Without base URL:\n");
    char* links1 = meta_oxide_extract_rel_links(html, NULL);
    if (links1) {
        printf("  %s\n", links1);
        meta_oxide_string_free(links1);
    }

    // With base URL (URLs will be resolved)
    printf("\nWith base URL (https://example.com):\n");
    char* links2 = meta_oxide_extract_rel_links(html, "https://example.com");
    if (links2) {
        printf("  %s\n", links2);
        meta_oxide_string_free(links2);
    }
}

// Example 9: Library version
void example_version() {
    printf("\n=== Example 9: Library Version ===\n");
    const char* version = meta_oxide_version();
    printf("MetaOxide C Library Version: %s\n", version);
}

// Main function
int main(int argc, char* argv[]) {
    printf("╔═══════════════════════════════════════════════╗\n");
    printf("║     MetaOxide C API Usage Examples           ║\n");
    printf("║  Comprehensive Metadata Extraction Library   ║\n");
    printf("╚═══════════════════════════════════════════════╝\n");

    // Run all examples
    example_version();
    example_extract_all();
    example_extract_specific();
    example_json_ld();
    example_microformats();
    example_microdata();
    example_manifest();
    example_base_url();
    example_error_handling();

    printf("\n╔═══════════════════════════════════════════════╗\n");
    printf("║  All examples completed successfully!        ║\n");
    printf("╚═══════════════════════════════════════════════╝\n");

    return 0;
}
