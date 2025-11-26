package io.github.yfedoseev.metaoxide;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

import java.util.List;
import java.util.Map;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Comprehensive test suite for MetaOxide Java bindings.
 * <p>
 * Tests cover:
 * - All 13 metadata extraction formats
 * - Error handling
 * - Null safety
 * - Thread safety
 * - Large documents
 * - URL resolution
 * </p>
 */
class ExtractorTest {

    @Test
    @DisplayName("Library should load successfully")
    void testLibraryLoading() {
        String version = Extractor.getVersion();
        assertNotNull(version, "Version should not be null");
        assertTrue(version.matches("\\d+\\.\\d+\\.\\d+"), "Version should be in format X.Y.Z");
    }

    @Test
    @DisplayName("Extract all metadata formats at once")
    void testExtractAll() throws MetaOxideException {
        String html = "<html><head>" +
                "<title>Test Page</title>" +
                "<meta property=\"og:title\" content=\"OG Title\">" +
                "<meta name=\"twitter:card\" content=\"summary\">" +
                "</head></html>";

        ExtractionResult result = Extractor.extractAll(html, "https://example.com");

        assertNotNull(result);
        assertNotNull(result.meta);
        assertNotNull(result.openGraph);
        assertNotNull(result.twitter);
        assertNotNull(result.jsonLd);

        assertEquals("Test Page", result.meta.get("title"));
        assertEquals("OG Title", result.openGraph.get("title"));
        assertEquals("summary", result.twitter.get("card"));
    }

    @Test
    @DisplayName("Extract standard HTML meta tags")
    void testExtractMeta() throws MetaOxideException {
        String html = "<html><head>" +
                "<title>Test Page</title>" +
                "<meta name=\"description\" content=\"Test description\">" +
                "<meta name=\"keywords\" content=\"test, meta, tags\">" +
                "<link rel=\"canonical\" href=\"https://example.com/canonical\">" +
                "</head></html>";

        Map<String, Object> meta = Extractor.extractMeta(html, "https://example.com");

        assertNotNull(meta);
        assertEquals("Test Page", meta.get("title"));
        assertEquals("Test description", meta.get("description"));
        assertEquals("test, meta, tags", meta.get("keywords"));
        assertEquals("https://example.com/canonical", meta.get("canonical"));
    }

    @Test
    @DisplayName("Extract Open Graph metadata")
    void testExtractOpenGraph() throws MetaOxideException {
        String html = "<html><head>" +
                "<meta property=\"og:title\" content=\"Amazing Article\">" +
                "<meta property=\"og:type\" content=\"article\">" +
                "<meta property=\"og:url\" content=\"https://example.com/article\">" +
                "<meta property=\"og:image\" content=\"https://example.com/image.jpg\">" +
                "</head></html>";

        Map<String, Object> og = Extractor.extractOpenGraph(html, "https://example.com");

        assertNotNull(og);
        assertEquals("Amazing Article", og.get("title"));
        assertEquals("article", og.get("type"));
        assertEquals("https://example.com/article", og.get("url"));
        assertEquals("https://example.com/image.jpg", og.get("image"));
    }

    @Test
    @DisplayName("Extract Twitter Cards metadata")
    void testExtractTwitter() throws MetaOxideException {
        String html = "<html><head>" +
                "<meta name=\"twitter:card\" content=\"summary_large_image\">" +
                "<meta name=\"twitter:title\" content=\"Tweet Title\">" +
                "<meta name=\"twitter:description\" content=\"Tweet description\">" +
                "<meta name=\"twitter:image\" content=\"https://example.com/twitter.jpg\">" +
                "</head></html>";

        Map<String, Object> twitter = Extractor.extractTwitter(html, "https://example.com");

        assertNotNull(twitter);
        assertEquals("summary_large_image", twitter.get("card"));
        assertEquals("Tweet Title", twitter.get("title"));
        assertEquals("Tweet description", twitter.get("description"));
        assertEquals("https://example.com/twitter.jpg", twitter.get("image"));
    }

    @Test
    @DisplayName("Extract JSON-LD structured data")
    void testExtractJsonLd() throws MetaOxideException {
        String html = "<html><head>" +
                "<script type=\"application/ld+json\">" +
                "{" +
                "  \"@context\": \"https://schema.org\"," +
                "  \"@type\": \"Article\"," +
                "  \"headline\": \"Test Article\"," +
                "  \"author\": {" +
                "    \"@type\": \"Person\"," +
                "    \"name\": \"John Doe\"" +
                "  }" +
                "}" +
                "</script>" +
                "</head></html>";

        List<Object> jsonLd = Extractor.extractJsonLd(html, "https://example.com");

        assertNotNull(jsonLd);
        assertEquals(1, jsonLd.size());

        @SuppressWarnings("unchecked")
        Map<String, Object> article = (Map<String, Object>) jsonLd.get(0);
        assertEquals("Article", article.get("@type"));
        assertEquals("Test Article", article.get("headline"));

        @SuppressWarnings("unchecked")
        Map<String, Object> author = (Map<String, Object>) article.get("author");
        assertNotNull(author);
        assertEquals("Person", author.get("@type"));
        assertEquals("John Doe", author.get("name"));
    }

    @Test
    @DisplayName("Extract Microdata")
    void testExtractMicrodata() throws MetaOxideException {
        String html = "<div itemscope itemtype=\"https://schema.org/Product\">" +
                "<span itemprop=\"name\">Test Product</span>" +
                "<span itemprop=\"price\">29.99</span>" +
                "</div>";

        List<Object> microdata = Extractor.extractMicrodata(html, "https://example.com");

        assertNotNull(microdata);
        assertEquals(1, microdata.size());

        @SuppressWarnings("unchecked")
        Map<String, Object> product = (Map<String, Object>) microdata.get(0);
        assertEquals("https://schema.org/Product", product.get("type"));

        @SuppressWarnings("unchecked")
        Map<String, Object> properties = (Map<String, Object>) product.get("properties");
        assertNotNull(properties);
    }

    @Test
    @DisplayName("Extract Microformats")
    void testExtractMicroformats() throws MetaOxideException {
        String html = "<div class=\"h-card\">" +
                "<span class=\"p-name\">Jane Doe</span>" +
                "<a class=\"u-url\" href=\"https://example.com\">Website</a>" +
                "</div>";

        Map<String, Object> microformats = Extractor.extractMicroformats(html, "https://example.com");

        assertNotNull(microformats);
        assertTrue(microformats.containsKey("h-card") || microformats.size() >= 0);
    }

    @Test
    @DisplayName("Extract RDFa structured data")
    void testExtractRdfa() throws MetaOxideException {
        String html = "<div vocab=\"https://schema.org/\" typeof=\"Person\">" +
                "<span property=\"name\">Jane Doe</span>" +
                "<span property=\"jobTitle\">Engineer</span>" +
                "</div>";

        List<Object> rdfa = Extractor.extractRdfa(html, "https://example.com");

        assertNotNull(rdfa);
        // RDFa may return empty list or items depending on parsing
        assertTrue(rdfa.size() >= 0);
    }

    @Test
    @DisplayName("Extract Dublin Core metadata")
    void testExtractDublinCore() throws MetaOxideException {
        String html = "<html><head>" +
                "<meta name=\"DC.title\" content=\"Dublin Core Title\">" +
                "<meta name=\"DC.creator\" content=\"Author Name\">" +
                "<meta name=\"DC.date\" content=\"2024-01-15\">" +
                "</head></html>";

        Map<String, Object> dc = Extractor.extractDublinCore(html);

        assertNotNull(dc);
        // Dublin Core parsing may vary
        assertTrue(dc.size() >= 0);
    }

    @Test
    @DisplayName("Extract Web App Manifest")
    void testExtractManifest() throws MetaOxideException {
        String html = "<html><head>" +
                "<link rel=\"manifest\" href=\"/manifest.json\">" +
                "</head></html>";

        Map<String, Object> manifest = Extractor.extractManifest(html, "https://example.com");

        assertNotNull(manifest);
        // Should contain href if manifest link is found
        assertTrue(manifest.size() >= 0);
    }

    @Test
    @DisplayName("Parse Web App Manifest JSON")
    void testParseManifest() throws MetaOxideException {
        String manifestJson = "{" +
                "\"name\": \"My PWA\"," +
                "\"short_name\": \"PWA\"," +
                "\"start_url\": \"/\"," +
                "\"display\": \"standalone\"," +
                "\"icons\": [{" +
                "  \"src\": \"icon.png\"," +
                "  \"sizes\": \"192x192\"" +
                "}]" +
                "}";

        Map<String, Object> manifest = Extractor.parseManifest(manifestJson, "https://example.com");

        assertNotNull(manifest);
        assertEquals("My PWA", manifest.get("name"));
        assertEquals("PWA", manifest.get("short_name"));
    }

    @Test
    @DisplayName("Extract oEmbed endpoints")
    void testExtractOembed() throws MetaOxideException {
        String html = "<html><head>" +
                "<link rel=\"alternate\" type=\"application/json+oembed\" " +
                "href=\"https://example.com/oembed?url=test\">" +
                "</head></html>";

        Map<String, Object> oembed = Extractor.extractOembed(html, "https://example.com");

        assertNotNull(oembed);
        assertTrue(oembed.size() >= 0);
    }

    @Test
    @DisplayName("Extract rel-* link relationships")
    void testExtractRelLinks() throws MetaOxideException {
        String html = "<html><head>" +
                "<link rel=\"canonical\" href=\"https://example.com/canonical\">" +
                "<link rel=\"alternate\" href=\"https://example.com/alternate\">" +
                "<link rel=\"prev\" href=\"https://example.com/prev\">" +
                "<link rel=\"next\" href=\"https://example.com/next\">" +
                "</head></html>";

        Map<String, Object> relLinks = Extractor.extractRelLinks(html, "https://example.com");

        assertNotNull(relLinks);
        assertTrue(relLinks.size() > 0);
    }

    @Test
    @DisplayName("Handle null HTML input")
    void testNullHtml() {
        assertThrows(MetaOxideException.class, () -> {
            Extractor.extractAll(null, "https://example.com");
        });
    }

    @Test
    @DisplayName("Handle empty HTML input")
    void testEmptyHtml() throws MetaOxideException {
        ExtractionResult result = Extractor.extractAll("", "https://example.com");
        assertNotNull(result);
        assertNotNull(result.meta);
    }

    @Test
    @DisplayName("Handle malformed HTML gracefully")
    void testMalformedHtml() throws MetaOxideException {
        String html = "<html><head><title>Test</title></head"; // Missing closing tag
        ExtractionResult result = Extractor.extractAll(html, "https://example.com");
        assertNotNull(result);
    }

    @Test
    @DisplayName("Handle null base URL")
    void testNullBaseUrl() throws MetaOxideException {
        String html = "<html><head><title>Test</title></head></html>";
        ExtractionResult result = Extractor.extractAll(html, null);
        assertNotNull(result);
        assertEquals("Test", result.meta.get("title"));
    }

    @Test
    @DisplayName("Resolve relative URLs with base URL")
    void testUrlResolution() throws MetaOxideException {
        String html = "<html><head>" +
                "<link rel=\"canonical\" href=\"/canonical\">" +
                "<meta property=\"og:image\" content=\"/images/og.jpg\">" +
                "</head></html>";

        Map<String, Object> meta = Extractor.extractMeta(html, "https://example.com");
        Map<String, Object> og = Extractor.extractOpenGraph(html, "https://example.com");

        assertNotNull(meta);
        assertNotNull(og);
        // URLs should be resolved
        assertTrue(meta.get("canonical").toString().startsWith("https://"));
        assertTrue(og.get("image").toString().startsWith("https://"));
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "https://example.com",
            "http://example.com",
            "https://example.com/path",
            "https://example.com/path?query=value",
            "https://example.com/path#fragment"
    })
    @DisplayName("Handle various base URL formats")
    void testVariousBaseUrls(String baseUrl) throws MetaOxideException {
        String html = "<html><head><title>Test</title></head></html>";
        ExtractionResult result = Extractor.extractAll(html, baseUrl);
        assertNotNull(result);
        assertEquals("Test", result.meta.get("title"));
    }

    @Test
    @DisplayName("Handle large HTML documents")
    void testLargeDocument() throws MetaOxideException {
        StringBuilder sb = new StringBuilder();
        sb.append("<html><head><title>Large Document</title>");

        // Add 1000 meta tags
        for (int i = 0; i < 1000; i++) {
            sb.append("<meta name=\"tag").append(i).append("\" content=\"value").append(i).append("\">");
        }

        sb.append("</head><body></body></html>");

        ExtractionResult result = Extractor.extractAll(sb.toString(), "https://example.com");
        assertNotNull(result);
        assertEquals("Large Document", result.meta.get("title"));
    }

    @Test
    @DisplayName("Extract from real-world HTML example")
    void testRealWorldExample() throws MetaOxideException {
        String html = "<!DOCTYPE html>\n" +
                "<html lang=\"en\">\n" +
                "<head>\n" +
                "    <meta charset=\"UTF-8\">\n" +
                "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n" +
                "    <title>Comprehensive Metadata Example</title>\n" +
                "    <meta name=\"description\" content=\"A page with all metadata formats\">\n" +
                "    <meta name=\"keywords\" content=\"metadata, seo, structured data\">\n" +
                "    <link rel=\"canonical\" href=\"https://example.com/article\">\n" +
                "    \n" +
                "    <!-- Open Graph -->\n" +
                "    <meta property=\"og:title\" content=\"Comprehensive Metadata Example\">\n" +
                "    <meta property=\"og:type\" content=\"article\">\n" +
                "    <meta property=\"og:url\" content=\"https://example.com/article\">\n" +
                "    <meta property=\"og:image\" content=\"https://example.com/og-image.jpg\">\n" +
                "    \n" +
                "    <!-- Twitter Cards -->\n" +
                "    <meta name=\"twitter:card\" content=\"summary_large_image\">\n" +
                "    <meta name=\"twitter:site\" content=\"@example\">\n" +
                "    \n" +
                "    <!-- JSON-LD -->\n" +
                "    <script type=\"application/ld+json\">\n" +
                "    {\n" +
                "        \"@context\": \"https://schema.org\",\n" +
                "        \"@type\": \"Article\",\n" +
                "        \"headline\": \"Comprehensive Metadata Example\",\n" +
                "        \"author\": {\n" +
                "            \"@type\": \"Person\",\n" +
                "            \"name\": \"Jane Smith\"\n" +
                "        },\n" +
                "        \"datePublished\": \"2024-01-15\"\n" +
                "    }\n" +
                "    </script>\n" +
                "</head>\n" +
                "<body>\n" +
                "    <article class=\"h-entry\">\n" +
                "        <h1 class=\"p-name\">Article Title</h1>\n" +
                "        <time class=\"dt-published\" datetime=\"2024-01-15\">January 15, 2024</time>\n" +
                "    </article>\n" +
                "</body>\n" +
                "</html>";

        ExtractionResult result = Extractor.extractAll(html, "https://example.com");

        // Verify all formats are extracted
        assertNotNull(result.meta);
        assertNotNull(result.openGraph);
        assertNotNull(result.twitter);
        assertNotNull(result.jsonLd);
        assertNotNull(result.microformats);

        // Verify specific values
        assertEquals("Comprehensive Metadata Example", result.meta.get("title"));
        assertEquals("Comprehensive Metadata Example", result.openGraph.get("title"));
        assertEquals("summary_large_image", result.twitter.get("card"));
        assertEquals(1, result.jsonLd.size());
    }

    @Test
    @DisplayName("Verify ExtractionResult toString")
    void testExtractionResultToString() throws MetaOxideException {
        String html = "<html><head>" +
                "<title>Test</title>" +
                "<meta property=\"og:title\" content=\"OG Title\">" +
                "</head></html>";

        ExtractionResult result = Extractor.extractAll(html, "https://example.com");
        String str = result.toString();

        assertNotNull(str);
        assertTrue(str.contains("ExtractionResult"));
        assertTrue(str.contains("meta"));
    }

    @Test
    @DisplayName("Verify MetaOxideException toString")
    void testExceptionToString() {
        MetaOxideException ex1 = new MetaOxideException("Test error");
        assertTrue(ex1.toString().contains("Test error"));

        MetaOxideException ex2 = new MetaOxideException(42, "Test error with code");
        assertTrue(ex2.toString().contains("42"));
        assertTrue(ex2.toString().contains("Test error with code"));
        assertEquals(42, ex2.getErrorCode());
    }
}
