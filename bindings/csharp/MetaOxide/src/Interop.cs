using System;
using System.Runtime.InteropServices;

namespace MetaOxide
{
    /// <summary>
    /// P/Invoke declarations for the native MetaOxide library.
    /// This class provides low-level access to the Rust core functionality.
    /// </summary>
    /// <remarks>
    /// All P/Invoke functions use the C calling convention and ANSI character encoding.
    /// Memory management is handled through explicit free functions.
    /// Thread-local error state is used for error reporting.
    /// </remarks>
    internal static class MetaOxideInterop
    {
        private const string LibraryName = "meta_oxide";

        /// <summary>
        /// Native result structure containing all extracted metadata as JSON strings.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MetaOxideResult
        {
            /// <summary>Standard HTML meta tags (JSON object)</summary>
            public IntPtr Meta;

            /// <summary>Open Graph metadata (JSON object)</summary>
            public IntPtr OpenGraph;

            /// <summary>Twitter Card metadata (JSON object)</summary>
            public IntPtr Twitter;

            /// <summary>JSON-LD structured data (JSON array)</summary>
            public IntPtr JsonLd;

            /// <summary>Microdata items (JSON array)</summary>
            public IntPtr Microdata;

            /// <summary>Microformats data (JSON object)</summary>
            public IntPtr Microformats;

            /// <summary>RDFa structured data (JSON array)</summary>
            public IntPtr RDFa;

            /// <summary>Dublin Core metadata (JSON object)</summary>
            public IntPtr DublinCore;

            /// <summary>Web App Manifest discovery (JSON object)</summary>
            public IntPtr Manifest;

            /// <summary>oEmbed endpoint discovery (JSON object)</summary>
            public IntPtr OEmbed;

            /// <summary>rel-* link relationships (JSON object)</summary>
            public IntPtr RelLinks;
        }

        /// <summary>
        /// Native manifest discovery result with URL and parsed content.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ManifestDiscovery
        {
            /// <summary>Manifest URL (may be null)</summary>
            public IntPtr Href;

            /// <summary>Full manifest JSON (may be null)</summary>
            public IntPtr Manifest;
        }

        /// <summary>
        /// Error codes returned by FFI functions.
        /// </summary>
        internal enum MetaOxideError
        {
            /// <summary>No error occurred</summary>
            Ok = 0,

            /// <summary>HTML parsing error</summary>
            ParseError = 1,

            /// <summary>Invalid URL format</summary>
            InvalidUrl = 2,

            /// <summary>Invalid UTF-8 string</summary>
            InvalidUtf8 = 3,

            /// <summary>Memory allocation error</summary>
            MemoryError = 4,

            /// <summary>JSON serialization error</summary>
            JsonError = 5,

            /// <summary>NULL pointer passed as argument</summary>
            NullPointer = 6,
        }

        #region Extraction Functions

        /// <summary>
        /// Extract ALL metadata from HTML.
        /// </summary>
        /// <param name="html">HTML content (must not be null)</param>
        /// <param name="baseUrl">Optional base URL for resolving relative URLs (can be null)</param>
        /// <returns>Pointer to MetaOxideResult structure, or IntPtr.Zero on error</returns>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_all(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract standard HTML meta tags.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_meta(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract Open Graph metadata.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_open_graph(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract Twitter Card metadata.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_twitter(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract JSON-LD structured data.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_json_ld(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract Microdata items.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_microdata(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract Microformats data (h-card, h-entry, h-event, etc.).
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_microformats(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract RDFa structured data.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_rdfa(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract Dublin Core metadata.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_dublin_core(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract Web App Manifest discovery data.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_manifest(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract oEmbed endpoint discovery data.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_oembed(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        /// <summary>
        /// Extract rel-* link relationships.
        /// </summary>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr meta_oxide_extract_rel_links(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string html,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string? baseUrl);

        #endregion

        #region Memory Management Functions

        /// <summary>
        /// Free a MetaOxideResult structure and all its string fields.
        /// </summary>
        /// <param name="result">Pointer to the result structure to free</param>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void meta_oxide_result_free(IntPtr result);

        /// <summary>
        /// Free a single string allocated by the library.
        /// </summary>
        /// <param name="str">Pointer to the string to free</param>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void meta_oxide_string_free(IntPtr str);

        /// <summary>
        /// Free a ManifestDiscovery structure and its string fields.
        /// </summary>
        /// <param name="discovery">Pointer to the discovery structure to free</param>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void meta_oxide_manifest_discovery_free(IntPtr discovery);

        #endregion

        #region Error Handling Functions

        /// <summary>
        /// Get the last error code that occurred on this thread.
        /// </summary>
        /// <returns>Error code from MetaOxideError enum</returns>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int meta_oxide_last_error();

        /// <summary>
        /// Get a human-readable error message for the last error.
        /// </summary>
        /// <returns>Pointer to error message string (must be freed with meta_oxide_string_free)</returns>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr meta_oxide_error_message();

        /// <summary>
        /// Get a human-readable description for an error code.
        /// </summary>
        /// <param name="error">Error code</param>
        /// <returns>Pointer to error description string (must be freed with meta_oxide_string_free)</returns>
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr meta_oxide_error_description(int error);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert a native UTF-8 string pointer to a managed string.
        /// </summary>
        /// <param name="ptr">Pointer to UTF-8 string</param>
        /// <returns>Managed string, or null if ptr is IntPtr.Zero</returns>
        internal static string? PtrToStringUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            // Count bytes until null terminator
            int length = 0;
            unsafe
            {
                byte* p = (byte*)ptr;
                while (*p != 0)
                {
                    length++;
                    p++;
                }
            }

            if (length == 0)
                return string.Empty;

            // Convert UTF-8 bytes to string
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        #endregion
    }
}
