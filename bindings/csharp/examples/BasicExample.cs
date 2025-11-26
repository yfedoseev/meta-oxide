using System;
using System.Net.Http;
using System.Threading.Tasks;
using MetaOxide;

namespace MetaOxide.Examples
{
    /// <summary>
    /// Basic example demonstrating metadata extraction with MetaOxide.
    /// </summary>
    public class BasicExample
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("MetaOxide Basic Example");
            Console.WriteLine("=======================\n");

            // Example 1: Extract from local HTML string
            Console.WriteLine("1. Extract from local HTML:");
            ExtractFromString();

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Example 2: Extract from URL
            Console.WriteLine("2. Extract from URL:");
            await ExtractFromUrl("https://example.com");

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Example 3: Extract specific formats
            Console.WriteLine("3. Extract specific formats:");
            ExtractSpecificFormats();
        }

        private static void ExtractFromString()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Example Page</title>
    <meta name=""description"" content=""This is an example page demonstrating MetaOxide"">
    <meta name=""keywords"" content=""example,metadata,extraction"">
    <meta property=""og:title"" content=""Example Page - Open Graph"">
    <meta property=""og:type"" content=""website"">
    <meta property=""og:url"" content=""https://example.com"">
    <meta name=""twitter:card"" content=""summary_large_image"">
    <meta name=""twitter:title"" content=""Example Page - Twitter"">
</head>
<body>
    <h1>Welcome</h1>
</body>
</html>";

            try
            {
                // Extract all metadata
                var result = Extractor.ExtractAll(html, "https://example.com");

                Console.WriteLine($"Found {result.GetMetadataFormatCount()} metadata formats\n");

                // Display meta tags
                if (result.Meta != null)
                {
                    Console.WriteLine("Standard Meta Tags:");
                    foreach (var kvp in result.Meta)
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }

                // Display Open Graph
                if (result.OpenGraph != null)
                {
                    Console.WriteLine("\nOpen Graph:");
                    foreach (var kvp in result.OpenGraph)
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }

                // Display Twitter Card
                if (result.Twitter != null)
                {
                    Console.WriteLine("\nTwitter Card:");
                    foreach (var kvp in result.Twitter)
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }
            }
            catch (MetaOxideException ex)
            {
                Console.WriteLine($"Error: {ex.GetFriendlyMessage()}");
            }
        }

        private static async Task ExtractFromUrl(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MetaOxide Example/1.0");

            try
            {
                // Fetch HTML
                var html = await httpClient.GetStringAsync(url);

                // Extract metadata
                var result = Extractor.ExtractAll(html, url);

                Console.WriteLine($"Extracted metadata from: {url}");
                Console.WriteLine($"Found {result.GetMetadataFormatCount()} metadata formats");

                // Display summary
                if (result.Meta != null && result.Meta.ContainsKey("description"))
                {
                    Console.WriteLine($"\nDescription: {result.Meta["description"]}");
                }

                if (result.OpenGraph != null && result.OpenGraph.ContainsKey("title"))
                {
                    Console.WriteLine($"OG Title: {result.OpenGraph["title"]}");
                }

                // Export to JSON
                var json = result.ToJson();
                Console.WriteLine($"\nJSON output ({json.Length} characters)");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
            }
            catch (MetaOxideException ex)
            {
                Console.WriteLine($"Extraction Error: {ex.GetFriendlyMessage()}");
            }
        }

        private static void ExtractSpecificFormats()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta property=""og:title"" content=""Open Graph Title"">
    <meta property=""og:description"" content=""OG Description"">
    <meta name=""twitter:card"" content=""summary"">
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Article"",
        ""headline"": ""Example Article"",
        ""author"": ""John Doe""
    }
    </script>
</head>
</html>";

            // Extract only Open Graph
            var openGraph = Extractor.ExtractOpenGraph(html);
            Console.WriteLine("Open Graph only:");
            if (openGraph != null)
            {
                foreach (var kvp in openGraph)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }

            // Extract only Twitter Card
            var twitter = Extractor.ExtractTwitter(html);
            Console.WriteLine("\nTwitter Card only:");
            if (twitter != null)
            {
                foreach (var kvp in twitter)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }

            // Extract only JSON-LD
            var jsonLd = Extractor.ExtractJsonLd(html);
            Console.WriteLine("\nJSON-LD only:");
            if (jsonLd != null)
            {
                Console.WriteLine($"  Found {jsonLd.Count} JSON-LD item(s)");
            }
        }
    }
}
