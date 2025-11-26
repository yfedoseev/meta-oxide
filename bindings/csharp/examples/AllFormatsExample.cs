using System;
using System.Collections.Generic;
using MetaOxide;

namespace MetaOxide.Examples
{
    /// <summary>
    /// Comprehensive example demonstrating extraction of all 13 metadata formats.
    /// </summary>
    public class AllFormatsExample
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("MetaOxide - All Formats Example");
            Console.WriteLine("================================\n");

            // Create an HTML document with all metadata formats
            var html = CreateComprehensiveHtml();

            // Extract all metadata at once
            var result = Extractor.ExtractAll(html, "https://example.com");

            // Display results for each format
            Console.WriteLine($"Total formats found: {result.GetMetadataFormatCount()}/11\n");
            Console.WriteLine(new string('=', 70));

            DisplayMeta(result);
            DisplayOpenGraph(result);
            DisplayTwitter(result);
            DisplayJsonLd(result);
            DisplayMicrodata(result);
            DisplayMicroformats(result);
            DisplayRDFa(result);
            DisplayDublinCore(result);
            DisplayManifest(result);
            DisplayOEmbed(result);
            DisplayRelLinks(result);

            // Export to JSON
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("\nComplete JSON output:");
            Console.WriteLine(result.ToJson());
        }

        private static string CreateComprehensiveHtml()
        {
            return @"
<!DOCTYPE html>
<html vocab=""http://schema.org/"" prefix=""og: http://ogp.me/ns#"">
<head>
    <title>Comprehensive Metadata Example</title>

    <!-- Standard HTML meta tags -->
    <meta name=""description"" content=""A comprehensive example showcasing all metadata formats"">
    <meta name=""keywords"" content=""metadata,extraction,seo,structured-data"">
    <meta name=""author"" content=""MetaOxide Team"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">

    <!-- Open Graph -->
    <meta property=""og:title"" content=""MetaOxide Comprehensive Example"">
    <meta property=""og:type"" content=""website"">
    <meta property=""og:url"" content=""https://example.com/comprehensive"">
    <meta property=""og:image"" content=""https://example.com/image.jpg"">
    <meta property=""og:description"" content=""Demonstrating all 13 metadata formats"">

    <!-- Twitter Card -->
    <meta name=""twitter:card"" content=""summary_large_image"">
    <meta name=""twitter:site"" content=""@metaoxide"">
    <meta name=""twitter:creator"" content=""@developer"">
    <meta name=""twitter:title"" content=""MetaOxide Example"">
    <meta name=""twitter:description"" content=""Comprehensive metadata example"">

    <!-- Dublin Core -->
    <meta name=""DC.title"" content=""Dublin Core Title"">
    <meta name=""DC.creator"" content=""DC Author"">
    <meta name=""DC.subject"" content=""metadata, web, semantic"">
    <meta name=""DC.date"" content=""2024-01-15"">

    <!-- Web App Manifest -->
    <link rel=""manifest"" href=""/manifest.json"">

    <!-- oEmbed -->
    <link rel=""alternate"" type=""application/json+oembed""
          href=""https://example.com/oembed?url=https://example.com/page"">

    <!-- rel-* links -->
    <link rel=""canonical"" href=""https://example.com/canonical"">
    <link rel=""alternate"" href=""https://example.com/es"" hreflang=""es"">
    <link rel=""prev"" href=""https://example.com/page1"">
    <link rel=""next"" href=""https://example.com/page3"">

    <!-- JSON-LD -->
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""WebPage"",
        ""name"": ""Comprehensive Example Page"",
        ""description"": ""Showcasing all metadata formats"",
        ""author"": {
            ""@type"": ""Person"",
            ""name"": ""Example Author""
        }
    }
    </script>
</head>
<body>
    <!-- Microformats (h-card) -->
    <div class=""h-card"">
        <img class=""u-photo"" src=""https://example.com/photo.jpg"" alt=""Profile"" />
        <span class=""p-name"">John Doe</span>
        <a class=""u-email"" href=""mailto:john@example.com"">Email</a>
        <span class=""p-tel"">+1-555-1234</span>
        <div class=""p-adr h-adr"">
            <span class=""p-locality"">San Francisco</span>,
            <span class=""p-region"">CA</span>
        </div>
    </div>

    <!-- Microformats (h-entry) -->
    <article class=""h-entry"">
        <h2 class=""p-name"">Example Article</h2>
        <p class=""p-summary"">This is an example article with microformats.</p>
        <time class=""dt-published"" datetime=""2024-01-15"">January 15, 2024</time>
        <a class=""p-author h-card"" href=""https://example.com/author"">Author Name</a>
    </article>

    <!-- Microdata -->
    <div itemscope itemtype=""http://schema.org/Product"">
        <h3 itemprop=""name"">Example Product</h3>
        <img itemprop=""image"" src=""https://example.com/product.jpg"" alt=""Product"" />
        <p itemprop=""description"">A great product example.</p>
        <div itemprop=""offers"" itemscope itemtype=""http://schema.org/Offer"">
            <span itemprop=""priceCurrency"" content=""USD"">$</span>
            <span itemprop=""price"" content=""99.99"">99.99</span>
        </div>
    </div>

    <!-- RDFa -->
    <div vocab=""http://schema.org/"" typeof=""Person"">
        <span property=""name"">Jane Developer</span>
        <span property=""jobTitle"">Software Engineer</span>
        <a property=""url"" href=""https://jane.example.com"">Website</a>
    </div>
</body>
</html>";
        }

        private static void DisplayMeta(ExtractionResult result)
        {
            Console.WriteLine("\n1. STANDARD HTML META TAGS");
            Console.WriteLine(new string('-', 70));
            if (result.Meta != null)
            {
                foreach (var kvp in result.Meta)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayOpenGraph(ExtractionResult result)
        {
            Console.WriteLine("\n2. OPEN GRAPH (og:*)");
            Console.WriteLine(new string('-', 70));
            if (result.OpenGraph != null)
            {
                foreach (var kvp in result.OpenGraph)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayTwitter(ExtractionResult result)
        {
            Console.WriteLine("\n3. TWITTER CARD");
            Console.WriteLine(new string('-', 70));
            if (result.Twitter != null)
            {
                foreach (var kvp in result.Twitter)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayJsonLd(ExtractionResult result)
        {
            Console.WriteLine("\n4. JSON-LD (Schema.org)");
            Console.WriteLine(new string('-', 70));
            if (result.JsonLd != null && result.JsonLd.Count > 0)
            {
                Console.WriteLine($"   Found {result.JsonLd.Count} JSON-LD block(s)");
                for (int i = 0; i < result.JsonLd.Count; i++)
                {
                    Console.WriteLine($"   Block {i + 1}: {result.JsonLd[i]}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayMicrodata(ExtractionResult result)
        {
            Console.WriteLine("\n5. MICRODATA");
            Console.WriteLine(new string('-', 70));
            if (result.Microdata != null && result.Microdata.Count > 0)
            {
                Console.WriteLine($"   Found {result.Microdata.Count} microdata item(s)");
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayMicroformats(ExtractionResult result)
        {
            Console.WriteLine("\n6. MICROFORMATS");
            Console.WriteLine(new string('-', 70));
            if (result.Microformats != null)
            {
                foreach (var kvp in result.Microformats)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayRDFa(ExtractionResult result)
        {
            Console.WriteLine("\n7. RDFa");
            Console.WriteLine(new string('-', 70));
            if (result.RDFa != null && result.RDFa.Count > 0)
            {
                Console.WriteLine($"   Found {result.RDFa.Count} RDFa triple(s)");
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayDublinCore(ExtractionResult result)
        {
            Console.WriteLine("\n8. DUBLIN CORE");
            Console.WriteLine(new string('-', 70));
            if (result.DublinCore != null)
            {
                foreach (var kvp in result.DublinCore)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayManifest(ExtractionResult result)
        {
            Console.WriteLine("\n9. WEB APP MANIFEST");
            Console.WriteLine(new string('-', 70));
            if (result.Manifest != null)
            {
                foreach (var kvp in result.Manifest)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayOEmbed(ExtractionResult result)
        {
            Console.WriteLine("\n10. oEmbed");
            Console.WriteLine(new string('-', 70));
            if (result.OEmbed != null)
            {
                foreach (var kvp in result.OEmbed)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }

        private static void DisplayRelLinks(ExtractionResult result)
        {
            Console.WriteLine("\n11. REL-* LINKS");
            Console.WriteLine(new string('-', 70));
            if (result.RelLinks != null)
            {
                foreach (var kvp in result.RelLinks)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("   (None found)");
            }
        }
    }
}
