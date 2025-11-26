using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaOxide
{
    /// <summary>
    /// Contains all metadata extracted from an HTML document.
    /// </summary>
    /// <remarks>
    /// This class provides strongly-typed access to all 13 metadata formats
    /// supported by MetaOxide. Each property represents a different metadata
    /// standard and may be null if that format was not found in the document.
    /// </remarks>
    public class ExtractionResult
    {
        /// <summary>
        /// Standard HTML meta tags (name, content pairs).
        /// </summary>
        /// <example>
        /// { "description": "Page description", "keywords": "seo,metadata" }
        /// </example>
        [JsonProperty("meta")]
        public Dictionary<string, object>? Meta { get; set; }

        /// <summary>
        /// Open Graph metadata (og:* properties).
        /// </summary>
        /// <example>
        /// { "title": "Article Title", "type": "article", "image": "https://example.com/image.jpg" }
        /// </example>
        [JsonProperty("open_graph")]
        public Dictionary<string, object>? OpenGraph { get; set; }

        /// <summary>
        /// Twitter Card metadata (twitter:* properties).
        /// </summary>
        /// <example>
        /// { "card": "summary_large_image", "site": "@username", "creator": "@author" }
        /// </example>
        [JsonProperty("twitter")]
        public Dictionary<string, object>? Twitter { get; set; }

        /// <summary>
        /// JSON-LD structured data (Schema.org vocabulary).
        /// </summary>
        /// <remarks>
        /// Returns an array of JSON-LD objects, as documents may contain multiple
        /// script type="application/ld+json" blocks.
        /// </remarks>
        /// <example>
        /// [{ "@context": "https://schema.org", "@type": "Article", "headline": "..." }]
        /// </example>
        [JsonProperty("json_ld")]
        public List<object>? JsonLd { get; set; }

        /// <summary>
        /// Microdata items (itemscope/itemprop attributes).
        /// </summary>
        /// <example>
        /// [{ "type": ["http://schema.org/Product"], "properties": { "name": ["Product Name"] } }]
        /// </example>
        [JsonProperty("microdata")]
        public List<object>? Microdata { get; set; }

        /// <summary>
        /// Microformats data (h-card, h-entry, h-event, etc.).
        /// </summary>
        /// <remarks>
        /// Contains arrays for each microformat type found (h-card, h-entry, h-event, etc.).
        /// </remarks>
        /// <example>
        /// { "h-card": [{ "name": "John Doe", "email": "john@example.com" }] }
        /// </example>
        [JsonProperty("microformats")]
        public Dictionary<string, object>? Microformats { get; set; }

        /// <summary>
        /// RDFa structured data (resource, property, typeof attributes).
        /// </summary>
        /// <example>
        /// [{ "subject": "http://example.com", "predicate": "dc:title", "object": "Title" }]
        /// </example>
        [JsonProperty("rdfa")]
        public List<object>? RDFa { get; set; }

        /// <summary>
        /// Dublin Core metadata (DC.* meta tags).
        /// </summary>
        /// <example>
        /// { "title": "Document Title", "creator": "Author Name", "date": "2024-01-01" }
        /// </example>
        [JsonProperty("dublin_core")]
        public Dictionary<string, object>? DublinCore { get; set; }

        /// <summary>
        /// Web App Manifest discovery (link rel="manifest").
        /// </summary>
        /// <remarks>
        /// Contains the manifest URL and optionally the parsed manifest JSON.
        /// </remarks>
        /// <example>
        /// { "href": "/manifest.json", "name": "App Name", "short_name": "App" }
        /// </example>
        [JsonProperty("manifest")]
        public Dictionary<string, object>? Manifest { get; set; }

        /// <summary>
        /// oEmbed endpoint discovery (link rel="alternate" type="application/json+oembed").
        /// </summary>
        /// <example>
        /// { "href": "https://example.com/oembed?url=...", "type": "application/json+oembed" }
        /// </example>
        [JsonProperty("oembed")]
        public Dictionary<string, object>? OEmbed { get; set; }

        /// <summary>
        /// rel-* link relationships (canonical, alternate, prev, next, etc.).
        /// </summary>
        /// <example>
        /// { "canonical": "https://example.com/page", "alternate": ["https://example.com/page?lang=es"] }
        /// </example>
        [JsonProperty("rel_links")]
        public Dictionary<string, object>? RelLinks { get; set; }

        /// <summary>
        /// Initializes a new instance of the ExtractionResult class.
        /// </summary>
        public ExtractionResult()
        {
        }

        /// <summary>
        /// Creates an ExtractionResult from a native MetaOxideResult structure.
        /// </summary>
        /// <param name="nativeResult">The native result structure</param>
        /// <returns>A new ExtractionResult with deserialized data</returns>
        internal static ExtractionResult FromNative(MetaOxideInterop.MetaOxideResult nativeResult)
        {
            return new ExtractionResult
            {
                Meta = DeserializeDictionary(nativeResult.Meta),
                OpenGraph = DeserializeDictionary(nativeResult.OpenGraph),
                Twitter = DeserializeDictionary(nativeResult.Twitter),
                JsonLd = DeserializeArray(nativeResult.JsonLd),
                Microdata = DeserializeArray(nativeResult.Microdata),
                Microformats = DeserializeDictionary(nativeResult.Microformats),
                RDFa = DeserializeArray(nativeResult.RDFa),
                DublinCore = DeserializeDictionary(nativeResult.DublinCore),
                Manifest = DeserializeDictionary(nativeResult.Manifest),
                OEmbed = DeserializeDictionary(nativeResult.OEmbed),
                RelLinks = DeserializeDictionary(nativeResult.RelLinks)
            };
        }

        /// <summary>
        /// Deserializes a JSON string pointer into a dictionary.
        /// </summary>
        private static Dictionary<string, object>? DeserializeDictionary(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            string? json = MetaOxideInterop.PtrToStringUtf8(ptr);
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                // Use JObject for more flexible deserialization
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj?.ToObject<Dictionary<string, object>>();
            }
            catch (JsonException)
            {
                // If deserialization fails, return null
                return null;
            }
        }

        /// <summary>
        /// Deserializes a JSON string pointer into a list.
        /// </summary>
        private static List<object>? DeserializeArray(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            string? json = MetaOxideInterop.PtrToStringUtf8(ptr);
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                // Use JArray for more flexible deserialization
                var arr = JsonConvert.DeserializeObject<JArray>(json);
                return arr?.ToObject<List<object>>();
            }
            catch (JsonException)
            {
                // If deserialization fails, return null
                return null;
            }
        }

        /// <summary>
        /// Checks if the result contains any metadata.
        /// </summary>
        /// <returns>True if any metadata format was found, false otherwise</returns>
        public bool HasAnyMetadata()
        {
            return Meta != null
                || OpenGraph != null
                || Twitter != null
                || JsonLd?.Count > 0
                || Microdata?.Count > 0
                || Microformats != null
                || RDFa?.Count > 0
                || DublinCore != null
                || Manifest != null
                || OEmbed != null
                || RelLinks != null;
        }

        /// <summary>
        /// Gets a count of how many metadata formats were found.
        /// </summary>
        /// <returns>Number of metadata formats with data</returns>
        public int GetMetadataFormatCount()
        {
            int count = 0;
            if (Meta != null) count++;
            if (OpenGraph != null) count++;
            if (Twitter != null) count++;
            if (JsonLd?.Count > 0) count++;
            if (Microdata?.Count > 0) count++;
            if (Microformats != null) count++;
            if (RDFa?.Count > 0) count++;
            if (DublinCore != null) count++;
            if (Manifest != null) count++;
            if (OEmbed != null) count++;
            if (RelLinks != null) count++;
            return count;
        }

        /// <summary>
        /// Converts the result to a JSON string.
        /// </summary>
        /// <param name="formatting">Optional JSON formatting (indented by default)</param>
        /// <returns>JSON representation of the extraction result</returns>
        public string ToJson(Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(this, formatting);
        }

        /// <summary>
        /// Creates an ExtractionResult from a JSON string.
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>A new ExtractionResult instance</returns>
        /// <exception cref="JsonException">Thrown when JSON is invalid</exception>
        public static ExtractionResult FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ExtractionResult>(json)
                ?? throw new JsonException("Failed to deserialize ExtractionResult from JSON");
        }

        /// <summary>
        /// Returns a string representation of the extraction result.
        /// </summary>
        public override string ToString()
        {
            return $"ExtractionResult: {GetMetadataFormatCount()} format(s) with data";
        }
    }
}
