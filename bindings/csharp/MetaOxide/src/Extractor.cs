using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaOxide
{
    /// <summary>
    /// Main API for extracting metadata from HTML documents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Extractor class provides a simple, high-performance API for extracting
    /// metadata from HTML content. It supports 13 different metadata formats:
    /// </para>
    /// <list type="bullet">
    /// <item>Standard HTML meta tags</item>
    /// <item>Open Graph (og:*)</item>
    /// <item>Twitter Cards (twitter:*)</item>
    /// <item>JSON-LD (Schema.org)</item>
    /// <item>Microdata</item>
    /// <item>Microformats (h-card, h-entry, h-event, etc.)</item>
    /// <item>RDFa</item>
    /// <item>Dublin Core</item>
    /// <item>Web App Manifest</item>
    /// <item>oEmbed</item>
    /// <item>rel-* link relationships</item>
    /// </list>
    /// <para>
    /// All extraction methods are thread-safe and can be called concurrently.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var html = "&lt;html&gt;&lt;head&gt;&lt;title&gt;Test&lt;/title&gt;&lt;/head&gt;&lt;/html&gt;";
    /// var result = Extractor.ExtractAll(html);
    /// Console.WriteLine($"Found {result.GetMetadataFormatCount()} metadata formats");
    /// </code>
    /// </example>
    public static class Extractor
    {
        /// <summary>
        /// Static constructor ensures the native library is loaded before any operations.
        /// </summary>
        static Extractor()
        {
            NativeLibraryLoader.EnsureLoaded();
        }

        #region Extract All Metadata

        /// <summary>
        /// Extract ALL metadata from HTML in a single operation.
        /// </summary>
        /// <param name="html">The HTML content to parse (required)</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>An ExtractionResult containing all extracted metadata</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        /// <example>
        /// <code>
        /// var html = await httpClient.GetStringAsync("https://example.com");
        /// var result = Extractor.ExtractAll(html, "https://example.com");
        ///
        /// if (result.OpenGraph != null)
        /// {
        ///     Console.WriteLine($"Title: {result.OpenGraph["title"]}");
        /// }
        /// </code>
        /// </example>
        public static ExtractionResult ExtractAll(string html, string? baseUrl = null)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentNullException(nameof(html), "HTML content cannot be null or empty");

            IntPtr resultPtr = MetaOxideInterop.meta_oxide_extract_all(html, baseUrl);

            if (resultPtr == IntPtr.Zero)
                throw MetaOxideException.FromLastError();

            try
            {
                var nativeResult = Marshal.PtrToStructure<MetaOxideInterop.MetaOxideResult>(resultPtr);
                return ExtractionResult.FromNative(nativeResult);
            }
            finally
            {
                MetaOxideInterop.meta_oxide_result_free(resultPtr);
            }
        }

        #endregion

        #region Extract Individual Formats

        /// <summary>
        /// Extract standard HTML meta tags as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary of meta tag name/content pairs, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractMeta(string html, string? baseUrl = null)
        {
            string? json = ExtractMetaJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract Open Graph metadata as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary of Open Graph properties, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractOpenGraph(string html, string? baseUrl = null)
        {
            string? json = ExtractOpenGraphJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract Twitter Card metadata as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary of Twitter Card properties, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractTwitter(string html, string? baseUrl = null)
        {
            string? json = ExtractTwitterJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract JSON-LD structured data as a list of objects.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>List of JSON-LD objects, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static List<object>? ExtractJsonLd(string html, string? baseUrl = null)
        {
            string? json = ExtractJsonLdJson(html, baseUrl);
            return DeserializeArray(json);
        }

        /// <summary>
        /// Extract Microdata items as a list of objects.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>List of Microdata items, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static List<object>? ExtractMicrodata(string html, string? baseUrl = null)
        {
            string? json = ExtractMicrodataJson(html, baseUrl);
            return DeserializeArray(json);
        }

        /// <summary>
        /// Extract Microformats data as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary containing arrays for each microformat type (h-card, h-entry, etc.), or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractMicroformats(string html, string? baseUrl = null)
        {
            string? json = ExtractMicroformatsJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract RDFa structured data as a list of triples.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>List of RDFa triples, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static List<object>? ExtractRDFa(string html, string? baseUrl = null)
        {
            string? json = ExtractRDFaJson(html, baseUrl);
            return DeserializeArray(json);
        }

        /// <summary>
        /// Extract Dublin Core metadata as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary of Dublin Core properties, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractDublinCore(string html, string? baseUrl = null)
        {
            string? json = ExtractDublinCoreJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract Web App Manifest discovery data as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary containing manifest URL and parsed content, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractManifest(string html, string? baseUrl = null)
        {
            string? json = ExtractManifestJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract oEmbed endpoint discovery data as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary containing oEmbed endpoint information, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractOEmbed(string html, string? baseUrl = null)
        {
            string? json = ExtractOEmbedJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        /// <summary>
        /// Extract rel-* link relationships as a dictionary.
        /// </summary>
        /// <param name="html">The HTML content to parse</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs</param>
        /// <returns>Dictionary of link relationships, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown when html is null or empty</exception>
        /// <exception cref="MetaOxideException">Thrown when extraction fails</exception>
        public static Dictionary<string, object>? ExtractRelLinks(string html, string? baseUrl = null)
        {
            string? json = ExtractRelLinksJson(html, baseUrl);
            return DeserializeDictionary(json);
        }

        #endregion

        #region JSON Extraction Methods

        /// <summary>
        /// Extract standard HTML meta tags as a JSON string.
        /// </summary>
        public static string? ExtractMetaJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_meta);
        }

        /// <summary>
        /// Extract Open Graph metadata as a JSON string.
        /// </summary>
        public static string? ExtractOpenGraphJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_open_graph);
        }

        /// <summary>
        /// Extract Twitter Card metadata as a JSON string.
        /// </summary>
        public static string? ExtractTwitterJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_twitter);
        }

        /// <summary>
        /// Extract JSON-LD structured data as a JSON string.
        /// </summary>
        public static string? ExtractJsonLdJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_json_ld);
        }

        /// <summary>
        /// Extract Microdata items as a JSON string.
        /// </summary>
        public static string? ExtractMicrodataJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_microdata);
        }

        /// <summary>
        /// Extract Microformats data as a JSON string.
        /// </summary>
        public static string? ExtractMicroformatsJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_microformats);
        }

        /// <summary>
        /// Extract RDFa structured data as a JSON string.
        /// </summary>
        public static string? ExtractRDFaJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_rdfa);
        }

        /// <summary>
        /// Extract Dublin Core metadata as a JSON string.
        /// </summary>
        public static string? ExtractDublinCoreJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_dublin_core);
        }

        /// <summary>
        /// Extract Web App Manifest discovery data as a JSON string.
        /// </summary>
        public static string? ExtractManifestJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_manifest);
        }

        /// <summary>
        /// Extract oEmbed endpoint discovery data as a JSON string.
        /// </summary>
        public static string? ExtractOEmbedJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_oembed);
        }

        /// <summary>
        /// Extract rel-* link relationships as a JSON string.
        /// </summary>
        public static string? ExtractRelLinksJson(string html, string? baseUrl = null)
        {
            return ExtractSingle(html, baseUrl, MetaOxideInterop.meta_oxide_extract_rel_links);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generic method for calling single extraction functions.
        /// </summary>
        private static string? ExtractSingle(
            string html,
            string? baseUrl,
            Func<string, string?, IntPtr> extractFunc)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentNullException(nameof(html), "HTML content cannot be null or empty");

            IntPtr resultPtr = extractFunc(html, baseUrl);

            if (resultPtr == IntPtr.Zero)
            {
                // Check if there was an error or just no data
                int errorCode = MetaOxideInterop.meta_oxide_last_error();
                if (errorCode != 0)
                {
                    throw MetaOxideException.FromLastError();
                }
                return null;
            }

            try
            {
                return MetaOxideInterop.PtrToStringUtf8(resultPtr);
            }
            finally
            {
                MetaOxideInterop.meta_oxide_string_free(resultPtr);
            }
        }

        /// <summary>
        /// Deserialize a JSON string into a dictionary.
        /// </summary>
        private static Dictionary<string, object>? DeserializeDictionary(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj?.ToObject<Dictionary<string, object>>();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Deserialize a JSON string into a list.
        /// </summary>
        private static List<object>? DeserializeArray(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var arr = JsonConvert.DeserializeObject<JArray>(json);
                return arr?.ToObject<List<object>>();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        #endregion

        #region Diagnostic Methods

        /// <summary>
        /// Gets diagnostic information about the native library and platform.
        /// </summary>
        /// <returns>Diagnostic information string</returns>
        public static string GetDiagnosticInfo()
        {
            return NativeLibraryLoader.GetDiagnosticInfo();
        }

        #endregion
    }
}
