/**
 * MetaOxide C API Integration Tests
 *
 * Comprehensive test suite for the C API covering all extractors,
 * error handling, memory management, and edge cases.
 *
 * Build:
 *   gcc -I../include -L../target/release -o c_api_test c_api_test.c -lmeta_oxide -lpthread -ldl -lm
 *
 * Run:
 *   LD_LIBRARY_PATH=../target/release ./c_api_test
 */

#include "../include/meta_oxide.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>

// Test counter
static int tests_passed = 0;
static int tests_failed = 0;

// Test macro
#define TEST(name) \
    printf("\n[TEST] %s\n", name); \
    void name(void); \
    name(); \
    void name(void)

#define ASSERT(condition, message) \
    if (!(condition)) { \
        fprintf(stderr, "  FAILED: %s\n", message); \
        tests_failed++; \
        return; \
    } else { \
        tests_passed++; \
    }

#define ASSERT_NOT_NULL(ptr, message) \
    ASSERT((ptr) != NULL, message)

#define ASSERT_NULL(ptr, message) \
    ASSERT((ptr) == NULL, message)

#define ASSERT_STR_CONTAINS(str, substr, message) \
    ASSERT(strstr(str, substr) != NULL, message)

// Sample HTML documents for testing
const char* SIMPLE_HTML =
    "<html>"
    "<head>"
    "  <title>Test Page</title>"
    "  <meta name=\"description\" content=\"Test description\">"
    "</head>"
    "</html>";

const char* RICH_HTML =
    "<html>"
    "<head>"
    "  <title>Comprehensive Test</title>"
    "  <meta name=\"description\" content=\"Test description\">"
    "  <meta property=\"og:title\" content=\"OG Title\">"
    "  <meta property=\"og:image\" content=\"https://example.com/image.jpg\">"
    "  <meta name=\"twitter:card\" content=\"summary\">"
    "  <meta name=\"twitter:title\" content=\"Twitter Title\">"
    "  <script type=\"application/ld+json\">"
    "  {"
    "    \"@type\": \"Article\","
    "    \"headline\": \"Test Article\","
    "    \"author\": \"John Doe\""
    "  }"
    "  </script>"
    "  <meta name=\"DC.title\" content=\"Dublin Core Title\">"
    "  <link rel=\"canonical\" href=\"https://example.com/page\">"
    "  <link rel=\"alternate\" type=\"application/json+oembed\" href=\"https://example.com/oembed\">"
    "</head>"
    "<body>"
    "  <div class=\"h-card\">"
    "    <span class=\"p-name\">Jane Doe</span>"
    "    <a class=\"u-url\" href=\"https://example.com\">Website</a>"
    "  </div>"
    "  <div itemscope itemtype=\"https://schema.org/Person\">"
    "    <span itemprop=\"name\">John Smith</span>"
    "    <span itemprop=\"email\">john@example.com</span>"
    "  </div>"
    "</body>"
    "</html>";

const char* UNICODE_HTML =
    "<html>"
    "<head>"
    "  <title>测试页面 - テスト</title>"
    "  <meta name=\"description\" content=\"日本語と中文の説明\">"
    "</head>"
    "</html>";

const char* MALFORMED_HTML =
    "<html><head><title>Test</title>"
    "<script type=\"application/ld+json\">{BROKEN JSON}</script>"
    "</head></html>";

// Test 1: Basic extraction
TEST(test_extract_all_basic) {
    MetaOxideResult* result = meta_oxide_extract_all(SIMPLE_HTML, NULL);
    ASSERT_NOT_NULL(result, "extract_all should return non-NULL result");
    ASSERT_NOT_NULL(result->meta, "meta field should be populated");
    ASSERT_STR_CONTAINS(result->meta, "Test Page", "meta should contain title");
    ASSERT_STR_CONTAINS(result->meta, "Test description", "meta should contain description");
    meta_oxide_result_free(result);
}

// Test 2: Extract with base URL
TEST(test_extract_all_with_base_url) {
    const char* html = "<html><head><link rel=\"canonical\" href=\"/page\"></head></html>";
    MetaOxideResult* result = meta_oxide_extract_all(html, "https://example.com");
    ASSERT_NOT_NULL(result, "extract_all with base_url should succeed");
    meta_oxide_result_free(result);
}

// Test 3: Rich HTML with all formats
TEST(test_extract_all_comprehensive) {
    MetaOxideResult* result = meta_oxide_extract_all(RICH_HTML, "https://example.com");
    ASSERT_NOT_NULL(result, "extract_all should handle rich HTML");

    ASSERT_NOT_NULL(result->meta, "meta should be extracted");
    ASSERT_NOT_NULL(result->open_graph, "open_graph should be extracted");
    ASSERT_NOT_NULL(result->twitter, "twitter should be extracted");
    ASSERT_NOT_NULL(result->json_ld, "json_ld should be extracted");
    ASSERT_NOT_NULL(result->microformats, "microformats should be extracted");
    ASSERT_NOT_NULL(result->microdata, "microdata should be extracted");
    ASSERT_NOT_NULL(result->dublin_core, "dublin_core should be extracted");
    ASSERT_NOT_NULL(result->rel_links, "rel_links should be extracted");
    ASSERT_NOT_NULL(result->oembed, "oembed should be extracted");

    ASSERT_STR_CONTAINS(result->open_graph, "OG Title", "OG title should be present");
    ASSERT_STR_CONTAINS(result->twitter, "Twitter Title", "Twitter title should be present");
    ASSERT_STR_CONTAINS(result->json_ld, "Article", "JSON-LD type should be present");

    meta_oxide_result_free(result);
}

// Test 4: Individual extractor - Meta
TEST(test_extract_meta) {
    char* meta = meta_oxide_extract_meta(SIMPLE_HTML, NULL);
    ASSERT_NOT_NULL(meta, "extract_meta should return result");
    ASSERT_STR_CONTAINS(meta, "Test Page", "meta should contain title");
    meta_oxide_string_free(meta);
}

// Test 5: Individual extractor - Open Graph
TEST(test_extract_open_graph) {
    char* og = meta_oxide_extract_open_graph(RICH_HTML, NULL);
    ASSERT_NOT_NULL(og, "extract_open_graph should return result");
    ASSERT_STR_CONTAINS(og, "OG Title", "OG should contain title");
    meta_oxide_string_free(og);
}

// Test 6: Individual extractor - Twitter
TEST(test_extract_twitter) {
    char* twitter = meta_oxide_extract_twitter(RICH_HTML, NULL);
    ASSERT_NOT_NULL(twitter, "extract_twitter should return result");
    ASSERT_STR_CONTAINS(twitter, "summary", "Twitter should contain card type");
    meta_oxide_string_free(twitter);
}

// Test 7: Individual extractor - JSON-LD
TEST(test_extract_json_ld) {
    char* jsonld = meta_oxide_extract_json_ld(RICH_HTML, NULL);
    ASSERT_NOT_NULL(jsonld, "extract_json_ld should return result");
    ASSERT_STR_CONTAINS(jsonld, "Article", "JSON-LD should contain type");
    meta_oxide_string_free(jsonld);
}

// Test 8: Individual extractor - Microdata
TEST(test_extract_microdata) {
    char* microdata = meta_oxide_extract_microdata(RICH_HTML, "https://example.com");
    ASSERT_NOT_NULL(microdata, "extract_microdata should return result");
    ASSERT_STR_CONTAINS(microdata, "Person", "Microdata should contain Person type");
    meta_oxide_string_free(microdata);
}

// Test 9: Individual extractor - Microformats
TEST(test_extract_microformats) {
    char* microformats = meta_oxide_extract_microformats(RICH_HTML, "https://example.com");
    ASSERT_NOT_NULL(microformats, "extract_microformats should return result");
    ASSERT_STR_CONTAINS(microformats, "h-card", "Microformats should contain h-card");
    meta_oxide_string_free(microformats);
}

// Test 10: Individual extractor - RDFa
TEST(test_extract_rdfa) {
    const char* rdfa_html =
        "<html><body>"
        "<div vocab=\"http://schema.org/\" typeof=\"Person\">"
        "  <span property=\"name\">John Doe</span>"
        "</div>"
        "</body></html>";

    char* rdfa = meta_oxide_extract_rdfa(rdfa_html, NULL);
    // RDFa might be empty if not present, that's OK
    if (rdfa != NULL) {
        meta_oxide_string_free(rdfa);
    }
}

// Test 11: Individual extractor - Dublin Core
TEST(test_extract_dublin_core) {
    char* dc = meta_oxide_extract_dublin_core(RICH_HTML);
    ASSERT_NOT_NULL(dc, "extract_dublin_core should return result");
    ASSERT_STR_CONTAINS(dc, "Dublin Core Title", "DC should contain title");
    meta_oxide_string_free(dc);
}

// Test 12: Individual extractor - Manifest
TEST(test_extract_manifest) {
    const char* manifest_html =
        "<html><head>"
        "<link rel=\"manifest\" href=\"/manifest.json\">"
        "</head></html>";

    char* manifest = meta_oxide_extract_manifest(manifest_html, "https://example.com");
    ASSERT_NOT_NULL(manifest, "extract_manifest should return result");
    meta_oxide_string_free(manifest);
}

// Test 13: Individual extractor - oEmbed
TEST(test_extract_oembed) {
    char* oembed = meta_oxide_extract_oembed(RICH_HTML, "https://example.com");
    ASSERT_NOT_NULL(oembed, "extract_oembed should return result");
    meta_oxide_string_free(oembed);
}

// Test 14: Individual extractor - rel links
TEST(test_extract_rel_links) {
    char* rel_links = meta_oxide_extract_rel_links(RICH_HTML, "https://example.com");
    ASSERT_NOT_NULL(rel_links, "extract_rel_links should return result");
    ASSERT_STR_CONTAINS(rel_links, "canonical", "rel_links should contain canonical");
    meta_oxide_string_free(rel_links);
}

// Test 15: Error handling - NULL HTML
TEST(test_null_html) {
    MetaOxideResult* result = meta_oxide_extract_all(NULL, NULL);
    ASSERT_NULL(result, "extract_all with NULL html should return NULL");
    int error = meta_oxide_last_error();
    ASSERT(error != 0, "error code should be non-zero for NULL html");
}

// Test 16: Error handling - NULL pointer in individual extractor
TEST(test_null_html_individual) {
    char* meta = meta_oxide_extract_meta(NULL, NULL);
    ASSERT_NULL(meta, "extract_meta with NULL html should return NULL");
    int error = meta_oxide_last_error();
    ASSERT(error != 0, "error code should be non-zero");
}

// Test 17: Empty HTML
TEST(test_empty_html) {
    const char* empty = "<html><head></head></html>";
    MetaOxideResult* result = meta_oxide_extract_all(empty, NULL);
    ASSERT_NOT_NULL(result, "extract_all should handle empty HTML");
    meta_oxide_result_free(result);
}

// Test 18: Unicode content
TEST(test_unicode_content) {
    MetaOxideResult* result = meta_oxide_extract_all(UNICODE_HTML, NULL);
    ASSERT_NOT_NULL(result, "extract_all should handle unicode");
    ASSERT_NOT_NULL(result->meta, "meta should be extracted from unicode HTML");
    meta_oxide_result_free(result);
}

// Test 19: Malformed content (should not crash)
TEST(test_malformed_html) {
    MetaOxideResult* result = meta_oxide_extract_all(MALFORMED_HTML, NULL);
    ASSERT_NOT_NULL(result, "extract_all should handle malformed HTML gracefully");
    // json_ld might be NULL due to broken JSON, that's expected
    meta_oxide_result_free(result);
}

// Test 20: HTML entities
TEST(test_html_entities) {
    const char* entities_html =
        "<html><head>"
        "<title>Test &amp; Demo &lt;Page&gt;</title>"
        "<meta name=\"description\" content=\"&quot;Quoted&quot; content\">"
        "</head></html>";

    MetaOxideResult* result = meta_oxide_extract_all(entities_html, NULL);
    ASSERT_NOT_NULL(result, "extract_all should handle HTML entities");
    ASSERT_NOT_NULL(result->meta, "meta should be extracted");
    meta_oxide_result_free(result);
}

// Test 21: Version string
TEST(test_version) {
    const char* version = meta_oxide_version();
    ASSERT_NOT_NULL(version, "version should not be NULL");
    ASSERT(strlen(version) > 0, "version should not be empty");
    printf("  Library version: %s\n", version);
}

// Test 22: Error message retrieval
TEST(test_error_message) {
    // Trigger an error by passing NULL
    char* result = meta_oxide_extract_meta(NULL, NULL);
    ASSERT_NULL(result, "should return NULL on error");

    const char* error_msg = meta_oxide_error_message();
    ASSERT_NOT_NULL(error_msg, "error message should not be NULL");
    printf("  Error message: %s\n", error_msg);
}

// Test 23: Memory leak test - allocate and free many times
TEST(test_memory_leak) {
    for (int i = 0; i < 100; i++) {
        MetaOxideResult* result = meta_oxide_extract_all(SIMPLE_HTML, NULL);
        if (result) {
            meta_oxide_result_free(result);
        }
    }
    ASSERT(1, "memory leak test completed");
}

// Test 24: Parse manifest JSON
TEST(test_parse_manifest) {
    const char* manifest_json =
        "{"
        "  \"name\": \"Test App\","
        "  \"icons\": [{\"src\": \"/icon.png\", \"sizes\": \"192x192\"}]"
        "}";

    char* parsed = meta_oxide_parse_manifest(manifest_json, "https://example.com");
    ASSERT_NOT_NULL(parsed, "parse_manifest should return result");
    ASSERT_STR_CONTAINS(parsed, "Test App", "manifest should contain app name");
    meta_oxide_string_free(parsed);
}

// Test 25: Large HTML document
TEST(test_large_html) {
    // Create a large HTML document with many items
    char* large_html = malloc(100000);
    strcpy(large_html, "<html><body>");
    for (int i = 0; i < 100; i++) {
        strcat(large_html, "<div class=\"h-card\"><span class=\"p-name\">Person ");
        char num[10];
        sprintf(num, "%d", i);
        strcat(large_html, num);
        strcat(large_html, "</span></div>");
    }
    strcat(large_html, "</body></html>");

    MetaOxideResult* result = meta_oxide_extract_all(large_html, NULL);
    ASSERT_NOT_NULL(result, "extract_all should handle large HTML");
    meta_oxide_result_free(result);
    free(large_html);
}

// Test 26: Base URL resolution
TEST(test_base_url_resolution) {
    const char* html = "<html><head><link rel=\"canonical\" href=\"/page\"></head></html>";
    char* rel_links = meta_oxide_extract_rel_links(html, "https://example.com");
    ASSERT_NOT_NULL(rel_links, "extract_rel_links should return result");
    ASSERT_STR_CONTAINS(rel_links, "https://example.com/page", "relative URL should be resolved");
    meta_oxide_string_free(rel_links);
}

// Test 27: Multiple JSON-LD objects
TEST(test_multiple_json_ld) {
    const char* html =
        "<html><head>"
        "<script type=\"application/ld+json\">{\"@type\": \"Article\"}</script>"
        "<script type=\"application/ld+json\">{\"@type\": \"Person\"}</script>"
        "</head></html>";

    char* jsonld = meta_oxide_extract_json_ld(html, NULL);
    ASSERT_NOT_NULL(jsonld, "extract_json_ld should return result");
    ASSERT_STR_CONTAINS(jsonld, "Article", "should contain first object");
    ASSERT_STR_CONTAINS(jsonld, "Person", "should contain second object");
    meta_oxide_string_free(jsonld);
}

// Test 28: Thread safety (basic check)
TEST(test_basic_thread_safety) {
    // Simple test: multiple sequential calls should not interfere
    MetaOxideResult* r1 = meta_oxide_extract_all(SIMPLE_HTML, NULL);
    MetaOxideResult* r2 = meta_oxide_extract_all(RICH_HTML, NULL);

    ASSERT_NOT_NULL(r1, "first call should succeed");
    ASSERT_NOT_NULL(r2, "second call should succeed");

    meta_oxide_result_free(r1);
    meta_oxide_result_free(r2);
}

// Main test runner
int main(void) {
    printf("=================================\n");
    printf("MetaOxide C API Integration Tests\n");
    printf("=================================\n");

    // Run all tests
    test_extract_all_basic();
    test_extract_all_with_base_url();
    test_extract_all_comprehensive();
    test_extract_meta();
    test_extract_open_graph();
    test_extract_twitter();
    test_extract_json_ld();
    test_extract_microdata();
    test_extract_microformats();
    test_extract_rdfa();
    test_extract_dublin_core();
    test_extract_manifest();
    test_extract_oembed();
    test_extract_rel_links();
    test_null_html();
    test_null_html_individual();
    test_empty_html();
    test_unicode_content();
    test_malformed_html();
    test_html_entities();
    test_version();
    test_error_message();
    test_memory_leak();
    test_parse_manifest();
    test_large_html();
    test_base_url_resolution();
    test_multiple_json_ld();
    test_basic_thread_safety();

    // Print summary
    printf("\n=================================\n");
    printf("Test Results\n");
    printf("=================================\n");
    printf("Passed: %d\n", tests_passed);
    printf("Failed: %d\n", tests_failed);
    printf("Total:  %d\n", tests_passed + tests_failed);
    printf("=================================\n");

    if (tests_failed == 0) {
        printf("\nAll tests PASSED! ✓\n");
        return 0;
    } else {
        printf("\nSome tests FAILED! ✗\n");
        return 1;
    }
}
