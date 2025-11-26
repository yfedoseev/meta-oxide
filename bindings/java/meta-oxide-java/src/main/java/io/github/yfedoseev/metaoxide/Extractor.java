package io.github.yfedoseev.metaoxide;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardCopyOption;
import java.util.List;
import java.util.Map;

/**
 * Main API for extracting metadata from HTML content.
 * <p>
 * This class provides static methods to extract various metadata formats from HTML:
 * </p>
 * <ul>
 *   <li>Standard HTML meta tags</li>
 *   <li>Open Graph Protocol (Facebook, LinkedIn)</li>
 *   <li>Twitter Cards</li>
 *   <li>JSON-LD (Schema.org)</li>
 *   <li>Microdata</li>
 *   <li>Microformats (h-card, h-entry, h-event, etc.)</li>
 *   <li>RDFa</li>
 *   <li>Dublin Core</li>
 *   <li>Web App Manifest</li>
 *   <li>oEmbed</li>
 *   <li>rel-* link relationships</li>
 * </ul>
 *
 * <h3>Example Usage:</h3>
 * <pre>{@code
 * String html = "<html><head><title>Test</title></head></html>";
 * ExtractionResult result = Extractor.extractAll(html, "https://example.com");
 * System.out.println("Title: " + result.meta.get("title"));
 * }</pre>
 *
 * <h3>Thread Safety:</h3>
 * All methods are thread-safe and can be called concurrently.
 *
 * @since 0.1.0
 */
public class Extractor {

    private static final ObjectMapper MAPPER = new ObjectMapper();
    private static boolean libraryLoaded = false;

    static {
        loadNativeLibrary();
    }

    /**
     * Load the native library from embedded resources or system library path.
     *
     * @throws UnsatisfiedLinkError if the library cannot be loaded
     */
    private static synchronized void loadNativeLibrary() {
        if (libraryLoaded) {
            return;
        }

        String osName = System.getProperty("os.name").toLowerCase();
        String osArch = System.getProperty("os.arch").toLowerCase();

        String libraryName;
        String resourcePath;

        // Determine library name and resource path based on OS and architecture
        if (osName.contains("win")) {
            libraryName = "meta_oxide.dll";
            resourcePath = "/native/win-x86_64/" + libraryName;
        } else if (osName.contains("mac")) {
            libraryName = "libmeta_oxide.dylib";
            if (osArch.contains("aarch64") || osArch.contains("arm")) {
                resourcePath = "/native/macos-aarch64/" + libraryName;
            } else {
                resourcePath = "/native/macos-x86_64/" + libraryName;
            }
        } else if (osName.contains("linux")) {
            libraryName = "libmeta_oxide.so";
            resourcePath = "/native/linux-x86_64/" + libraryName;
        } else {
            throw new UnsatisfiedLinkError("Unsupported operating system: " + osName);
        }

        try {
            // Try loading from embedded resources first
            InputStream is = Extractor.class.getResourceAsStream(resourcePath);
            if (is != null) {
                // Extract to temporary file and load
                Path tempFile = Files.createTempFile("meta_oxide_", libraryName.substring(libraryName.lastIndexOf('.')));
                tempFile.toFile().deleteOnExit();
                Files.copy(is, tempFile, StandardCopyOption.REPLACE_EXISTING);
                is.close();
                System.load(tempFile.toAbsolutePath().toString());
                libraryLoaded = true;
                return;
            }

            // Fall back to system library path
            System.loadLibrary("meta_oxide");
            libraryLoaded = true;
        } catch (IOException e) {
            throw new UnsatisfiedLinkError("Failed to load native library: " + e.getMessage());
        }
    }

    // ===== Native Method Declarations =====

    /**
     * Native method to extract all metadata formats at once.
     */
    private static native String nativeExtractAll(String html, String baseUrl);

    /**
     * Native method to extract standard HTML meta tags.
     */
    private static native String nativeExtractMeta(String html, String baseUrl);

    /**
     * Native method to extract Open Graph metadata.
     */
    private static native String nativeExtractOpenGraph(String html, String baseUrl);

    /**
     * Native method to extract Twitter Card metadata.
     */
    private static native String nativeExtractTwitter(String html, String baseUrl);

    /**
     * Native method to extract JSON-LD structured data.
     */
    private static native String nativeExtractJsonLd(String html, String baseUrl);

    /**
     * Native method to extract Microdata.
     */
    private static native String nativeExtractMicrodata(String html, String baseUrl);

    /**
     * Native method to extract Microformats.
     */
    private static native String nativeExtractMicroformats(String html, String baseUrl);

    /**
     * Native method to extract RDFa structured data.
     */
    private static native String nativeExtractRdfa(String html, String baseUrl);

    /**
     * Native method to extract Dublin Core metadata.
     */
    private static native String nativeExtractDublinCore(String html);

    /**
     * Native method to extract Web App Manifest link.
     */
    private static native String nativeExtractManifest(String html, String baseUrl);

    /**
     * Native method to parse Web App Manifest JSON.
     */
    private static native String nativeParseManifest(String json, String baseUrl);

    /**
     * Native method to extract oEmbed endpoint discovery.
     */
    private static native String nativeExtractOembed(String html, String baseUrl);

    /**
     * Native method to extract rel-* link relationships.
     */
    private static native String nativeExtractRelLinks(String html, String baseUrl);

    /**
     * Native method to get library version.
     */
    private static native String nativeVersion();

    // ===== Public API Methods =====

    /**
     * Extract ALL metadata from HTML in a single call (recommended!).
     * <p>
     * This is the most efficient way to extract metadata as it parses the HTML only once.
     * </p>
     *
     * @param html    the HTML content to extract from (required)
     * @param baseUrl the base URL for resolving relative URLs (optional, may be null or empty)
     * @return an ExtractionResult containing all extracted metadata
     * @throws MetaOxideException if extraction fails
     */
    public static ExtractionResult extractAll(String html, String baseUrl) throws MetaOxideException {
        if (html == null) {
            throw new MetaOxideException("HTML content cannot be null");
        }

        String jsonResult = nativeExtractAll(html, baseUrl == null ? "" : baseUrl);
        if (jsonResult == null) {
            throw new MetaOxideException("Failed to extract metadata");
        }

        try {
            @SuppressWarnings("unchecked")
            Map<String, Object> resultMap = MAPPER.readValue(jsonResult, Map.class);

            return new ExtractionResult(
                    getMapOrEmpty(resultMap, "meta"),
                    getMapOrEmpty(resultMap, "openGraph"),
                    getMapOrEmpty(resultMap, "twitter"),
                    getListOrEmpty(resultMap, "jsonLd"),
                    getListOrEmpty(resultMap, "microdata"),
                    getMapOrEmpty(resultMap, "microformats"),
                    getListOrEmpty(resultMap, "rdfa"),
                    getMapOrEmpty(resultMap, "dublinCore"),
                    getMapOrEmpty(resultMap, "manifest"),
                    getMapOrEmpty(resultMap, "oembed"),
                    getMapOrEmpty(resultMap, "relLinks")
            );
        } catch (IOException e) {
            throw new MetaOxideException("Failed to parse extraction result: " + e.getMessage(), e);
        }
    }

    /**
     * Extract ALL metadata from HTML without a base URL.
     *
     * @param html the HTML content to extract from
     * @return an ExtractionResult containing all extracted metadata
     * @throws MetaOxideException if extraction fails
     * @see #extractAll(String, String)
     */
    public static ExtractionResult extractAll(String html) throws MetaOxideException {
        return extractAll(html, null);
    }

    /**
     * Extract standard HTML meta tags (title, description, keywords, canonical, etc.).
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map of meta tag properties
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractMeta(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractMeta(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractMeta(String html) throws MetaOxideException {
        return extractMeta(html, null);
    }

    /**
     * Extract Open Graph Protocol metadata for social sharing.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map of Open Graph properties
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractOpenGraph(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractOpenGraph(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractOpenGraph(String html) throws MetaOxideException {
        return extractOpenGraph(html, null);
    }

    /**
     * Extract Twitter Cards metadata for Twitter/X link previews.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map of Twitter Card properties
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractTwitter(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractTwitter(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractTwitter(String html) throws MetaOxideException {
        return extractTwitter(html, null);
    }

    /**
     * Extract JSON-LD structured data (Schema.org) for search engines and AI.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a list of JSON-LD objects
     * @throws MetaOxideException if extraction fails
     */
    public static List<Object> extractJsonLd(String html, String baseUrl) throws MetaOxideException {
        return parseJsonArray(nativeExtractJsonLd(html, baseUrl == null ? "" : baseUrl));
    }

    public static List<Object> extractJsonLd(String html) throws MetaOxideException {
        return extractJsonLd(html, null);
    }

    /**
     * Extract Microdata structured data embedded in HTML attributes.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a list of Microdata items
     * @throws MetaOxideException if extraction fails
     */
    public static List<Object> extractMicrodata(String html, String baseUrl) throws MetaOxideException {
        return parseJsonArray(nativeExtractMicrodata(html, baseUrl == null ? "" : baseUrl));
    }

    public static List<Object> extractMicrodata(String html) throws MetaOxideException {
        return extractMicrodata(html, null);
    }

    /**
     * Extract Microformats (h-card, h-entry, h-event, etc.) for IndieWeb.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map of Microformats by type
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractMicroformats(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractMicroformats(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractMicroformats(String html) throws MetaOxideException {
        return extractMicroformats(html, null);
    }

    /**
     * Extract RDFa (Resource Description Framework in Attributes) for semantic web.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a list of RDFa items
     * @throws MetaOxideException if extraction fails
     */
    public static List<Object> extractRdfa(String html, String baseUrl) throws MetaOxideException {
        return parseJsonArray(nativeExtractRdfa(html, baseUrl == null ? "" : baseUrl));
    }

    public static List<Object> extractRdfa(String html) throws MetaOxideException {
        return extractRdfa(html, null);
    }

    /**
     * Extract Dublin Core metadata elements for academic and library content.
     *
     * @param html the HTML content
     * @return a map of Dublin Core properties
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractDublinCore(String html) throws MetaOxideException {
        return parseJsonObject(nativeExtractDublinCore(html));
    }

    /**
     * Extract Web App Manifest metadata for Progressive Web Apps.
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map with manifest link information
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractManifest(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractManifest(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractManifest(String html) throws MetaOxideException {
        return extractManifest(html, null);
    }

    /**
     * Parse Web App Manifest JSON content.
     *
     * @param manifestJson the manifest JSON content
     * @param baseUrl      the base URL for resolving relative URLs (optional)
     * @return a map of parsed manifest properties with URLs resolved
     * @throws MetaOxideException if parsing fails
     */
    public static Map<String, Object> parseManifest(String manifestJson, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeParseManifest(manifestJson, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> parseManifest(String manifestJson) throws MetaOxideException {
        return parseManifest(manifestJson, null);
    }

    /**
     * Extract oEmbed endpoint discovery for embedded content (YouTube, Twitter, etc.).
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map with oEmbed endpoint information
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractOembed(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractOembed(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractOembed(String html) throws MetaOxideException {
        return extractOembed(html, null);
    }

    /**
     * Extract rel-* link relationships (canonical, alternate, feed, etc.).
     *
     * @param html    the HTML content
     * @param baseUrl the base URL for resolving relative URLs (optional)
     * @return a map of link relationships grouped by rel type
     * @throws MetaOxideException if extraction fails
     */
    public static Map<String, Object> extractRelLinks(String html, String baseUrl) throws MetaOxideException {
        return parseJsonObject(nativeExtractRelLinks(html, baseUrl == null ? "" : baseUrl));
    }

    public static Map<String, Object> extractRelLinks(String html) throws MetaOxideException {
        return extractRelLinks(html, null);
    }

    /**
     * Get the version of the MetaOxide library.
     *
     * @return version string (e.g., "0.1.0")
     */
    public static String getVersion() {
        return nativeVersion();
    }

    // ===== Helper Methods =====

    @SuppressWarnings("unchecked")
    private static Map<String, Object> getMapOrEmpty(Map<String, Object> map, String key) {
        Object value = map.get(key);
        if (value instanceof Map) {
            return (Map<String, Object>) value;
        }
        return new java.util.HashMap<>();
    }

    @SuppressWarnings("unchecked")
    private static List<Object> getListOrEmpty(Map<String, Object> map, String key) {
        Object value = map.get(key);
        if (value instanceof List) {
            return (List<Object>) value;
        }
        return new java.util.ArrayList<>();
    }

    private static Map<String, Object> parseJsonObject(String json) throws MetaOxideException {
        if (json == null) {
            throw new MetaOxideException("Extraction failed - null result");
        }
        try {
            return MAPPER.readValue(json, new TypeReference<Map<String, Object>>() {
            });
        } catch (IOException e) {
            throw new MetaOxideException("Failed to parse JSON: " + e.getMessage(), e);
        }
    }

    private static List<Object> parseJsonArray(String json) throws MetaOxideException {
        if (json == null) {
            throw new MetaOxideException("Extraction failed - null result");
        }
        try {
            return MAPPER.readValue(json, new TypeReference<List<Object>>() {
            });
        } catch (IOException e) {
            throw new MetaOxideException("Failed to parse JSON: " + e.getMessage(), e);
        }
    }

    // Prevent instantiation
    private Extractor() {
    }
}
