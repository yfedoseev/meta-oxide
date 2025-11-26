using System;
using System.Collections.Generic;
using MetaOxide;
using Newtonsoft.Json.Linq;

namespace MetaOxide.Examples
{
    /// <summary>
    /// Example demonstrating JSON-LD (Schema.org) structured data extraction.
    /// </summary>
    public class JsonLdExample
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("MetaOxide JSON-LD Example");
            Console.WriteLine("=========================\n");

            // Example 1: Extract Article schema
            Console.WriteLine("1. Article Schema:");
            ExtractArticle();

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Example 2: Extract Product schema
            Console.WriteLine("2. Product Schema:");
            ExtractProduct();

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Example 3: Extract Organization schema
            Console.WriteLine("3. Organization Schema:");
            ExtractOrganization();

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Example 4: Multiple JSON-LD blocks
            Console.WriteLine("4. Multiple JSON-LD blocks:");
            ExtractMultiple();
        }

        private static void ExtractArticle()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Article"",
        ""headline"": ""How to Use MetaOxide for Metadata Extraction"",
        ""datePublished"": ""2024-01-15"",
        ""dateModified"": ""2024-01-20"",
        ""author"": {
            ""@type"": ""Person"",
            ""name"": ""Jane Developer""
        },
        ""publisher"": {
            ""@type"": ""Organization"",
            ""name"": ""Tech Blog"",
            ""logo"": {
                ""@type"": ""ImageObject"",
                ""url"": ""https://example.com/logo.png""
            }
        },
        ""description"": ""Learn how to extract structured data from web pages using MetaOxide"",
        ""image"": ""https://example.com/article-image.jpg""
    }
    </script>
</head>
</html>";

            var jsonLdList = Extractor.ExtractJsonLd(html);
            if (jsonLdList == null || jsonLdList.Count == 0)
            {
                Console.WriteLine("No JSON-LD found");
                return;
            }

            // Parse the first JSON-LD item
            var article = JObject.FromObject(jsonLdList[0]);

            Console.WriteLine($"Type: {article["@type"]}");
            Console.WriteLine($"Headline: {article["headline"]}");
            Console.WriteLine($"Published: {article["datePublished"]}");
            Console.WriteLine($"Modified: {article["dateModified"]}");

            var author = article["author"] as JObject;
            if (author != null)
            {
                Console.WriteLine($"Author: {author["name"]}");
            }

            Console.WriteLine($"Description: {article["description"]}");
        }

        private static void ExtractProduct()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Product"",
        ""name"": ""MetaOxide Pro License"",
        ""image"": ""https://example.com/product.jpg"",
        ""description"": ""Professional metadata extraction library"",
        ""brand"": {
            ""@type"": ""Brand"",
            ""name"": ""MetaOxide""
        },
        ""offers"": {
            ""@type"": ""Offer"",
            ""price"": ""99.00"",
            ""priceCurrency"": ""USD"",
            ""availability"": ""https://schema.org/InStock""
        },
        ""aggregateRating"": {
            ""@type"": ""AggregateRating"",
            ""ratingValue"": ""4.8"",
            ""reviewCount"": ""245""
        }
    }
    </script>
</head>
</html>";

            var jsonLdList = Extractor.ExtractJsonLd(html);
            if (jsonLdList == null || jsonLdList.Count == 0)
            {
                Console.WriteLine("No JSON-LD found");
                return;
            }

            var product = JObject.FromObject(jsonLdList[0]);

            Console.WriteLine($"Product: {product["name"]}");
            Console.WriteLine($"Description: {product["description"]}");

            var offers = product["offers"] as JObject;
            if (offers != null)
            {
                Console.WriteLine($"Price: {offers["priceCurrency"]} {offers["price"]}");
            }

            var rating = product["aggregateRating"] as JObject;
            if (rating != null)
            {
                Console.WriteLine($"Rating: {rating["ratingValue"]} ({rating["reviewCount"]} reviews)");
            }
        }

        private static void ExtractOrganization()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Organization"",
        ""name"": ""MetaOxide Inc."",
        ""url"": ""https://metaoxide.com"",
        ""logo"": ""https://metaoxide.com/logo.png"",
        ""contactPoint"": {
            ""@type"": ""ContactPoint"",
            ""telephone"": ""+1-555-1234"",
            ""contactType"": ""customer service""
        },
        ""sameAs"": [
            ""https://twitter.com/metaoxide"",
            ""https://github.com/metaoxide""
        ]
    }
    </script>
</head>
</html>";

            var jsonLdList = Extractor.ExtractJsonLd(html);
            if (jsonLdList == null || jsonLdList.Count == 0)
            {
                Console.WriteLine("No JSON-LD found");
                return;
            }

            var org = JObject.FromObject(jsonLdList[0]);

            Console.WriteLine($"Organization: {org["name"]}");
            Console.WriteLine($"URL: {org["url"]}");

            var contact = org["contactPoint"] as JObject;
            if (contact != null)
            {
                Console.WriteLine($"Phone: {contact["telephone"]}");
            }

            var sameAs = org["sameAs"] as JArray;
            if (sameAs != null && sameAs.Count > 0)
            {
                Console.WriteLine("Social profiles:");
                foreach (var url in sameAs)
                {
                    Console.WriteLine($"  - {url}");
                }
            }
        }

        private static void ExtractMultiple()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""Organization"",
        ""name"": ""Example Corp""
    }
    </script>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""BreadcrumbList"",
        ""itemListElement"": [
            {
                ""@type"": ""ListItem"",
                ""position"": 1,
                ""name"": ""Home"",
                ""item"": ""https://example.com""
            },
            {
                ""@type"": ""ListItem"",
                ""position"": 2,
                ""name"": ""Products"",
                ""item"": ""https://example.com/products""
            }
        ]
    }
    </script>
    <script type=""application/ld+json"">
    {
        ""@context"": ""https://schema.org"",
        ""@type"": ""WebSite"",
        ""name"": ""Example Site"",
        ""url"": ""https://example.com"",
        ""potentialAction"": {
            ""@type"": ""SearchAction"",
            ""target"": ""https://example.com/search?q={search_term_string}"",
            ""query-input"": ""required name=search_term_string""
        }
    }
    </script>
</head>
</html>";

            var jsonLdList = Extractor.ExtractJsonLd(html);
            if (jsonLdList == null || jsonLdList.Count == 0)
            {
                Console.WriteLine("No JSON-LD found");
                return;
            }

            Console.WriteLine($"Found {jsonLdList.Count} JSON-LD blocks:\n");

            for (int i = 0; i < jsonLdList.Count; i++)
            {
                var item = JObject.FromObject(jsonLdList[i]);
                Console.WriteLine($"Block {i + 1}:");
                Console.WriteLine($"  Type: {item["@type"]}");
                if (item.ContainsKey("name"))
                {
                    Console.WriteLine($"  Name: {item["name"]}");
                }
                Console.WriteLine();
            }
        }
    }
}
