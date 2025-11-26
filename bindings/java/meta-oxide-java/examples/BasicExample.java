package io.github.yfedoseev.metaoxide.examples;

import io.github.yfedoseev.metaoxide.Extractor;
import io.github.yfedoseev.metaoxide.ExtractionResult;
import io.github.yfedoseev.metaoxide.MetaOxideException;

import java.util.Map;

/**
 * Basic example demonstrating MetaOxide metadata extraction in Java.
 * <p>
 * This example shows how to:
 * - Extract all metadata formats at once
 * - Access individual metadata fields
 * - Handle errors properly
 * </p>
 */
public class BasicExample {

    public static void main(String[] args) {
        // Sample HTML with various metadata formats
        String html = "<!DOCTYPE html>\n" +
                "<html>\n" +
                "<head>\n" +
                "    <title>Amazing Article About Rust</title>\n" +
                "    <meta name=\"description\" content=\"Learn about Rust programming language\">\n" +
                "    <meta name=\"keywords\" content=\"rust, programming, systems\">\n" +
                "    <meta property=\"og:title\" content=\"Amazing Article About Rust\">\n" +
                "    <meta property=\"og:type\" content=\"article\">\n" +
                "    <meta property=\"og:url\" content=\"https://example.com/article\">\n" +
                "    <meta property=\"og:image\" content=\"https://example.com/image.jpg\">\n" +
                "    <meta name=\"twitter:card\" content=\"summary_large_image\">\n" +
                "    <meta name=\"twitter:title\" content=\"Amazing Article\">\n" +
                "    <script type=\"application/ld+json\">\n" +
                "    {\n" +
                "        \"@context\": \"https://schema.org\",\n" +
                "        \"@type\": \"Article\",\n" +
                "        \"headline\": \"Amazing Article About Rust\",\n" +
                "        \"author\": {\n" +
                "            \"@type\": \"Person\",\n" +
                "            \"name\": \"John Doe\"\n" +
                "        },\n" +
                "        \"datePublished\": \"2024-01-15\"\n" +
                "    }\n" +
                "    </script>\n" +
                "</head>\n" +
                "<body>\n" +
                "    <article class=\"h-entry\">\n" +
                "        <h1 class=\"p-name\">Getting Started with Rust</h1>\n" +
                "        <time class=\"dt-published\" datetime=\"2024-01-15\">January 15, 2024</time>\n" +
                "    </article>\n" +
                "</body>\n" +
                "</html>";

        try {
            // Extract ALL metadata at once (most efficient!)
            System.out.println("=== Extracting All Metadata ===\n");
            ExtractionResult result = Extractor.extractAll(html, "https://example.com");

            // Standard HTML meta tags
            System.out.println("Standard Meta Tags:");
            System.out.println("  Title: " + result.meta.get("title"));
            System.out.println("  Description: " + result.meta.get("description"));
            System.out.println("  Keywords: " + result.meta.get("keywords"));
            System.out.println();

            // Open Graph Protocol
            System.out.println("Open Graph:");
            System.out.println("  Title: " + result.openGraph.get("title"));
            System.out.println("  Type: " + result.openGraph.get("type"));
            System.out.println("  URL: " + result.openGraph.get("url"));
            System.out.println("  Image: " + result.openGraph.get("image"));
            System.out.println();

            // Twitter Cards
            System.out.println("Twitter Cards:");
            System.out.println("  Card: " + result.twitter.get("card"));
            System.out.println("  Title: " + result.twitter.get("title"));
            System.out.println();

            // JSON-LD
            System.out.println("JSON-LD:");
            if (!result.jsonLd.isEmpty()) {
                @SuppressWarnings("unchecked")
                Map<String, Object> article = (Map<String, Object>) result.jsonLd.get(0);
                System.out.println("  Type: " + article.get("@type"));
                System.out.println("  Headline: " + article.get("headline"));

                @SuppressWarnings("unchecked")
                Map<String, Object> author = (Map<String, Object>) article.get("author");
                if (author != null) {
                    System.out.println("  Author: " + author.get("name"));
                }
                System.out.println("  Published: " + article.get("datePublished"));
            }
            System.out.println();

            // Microformats
            System.out.println("Microformats:");
            if (result.microformats.containsKey("h-entry")) {
                System.out.println("  Found h-entry microformat");
            }
            System.out.println();

            // Summary
            System.out.println(result);
            System.out.println();

            // Extract individual formats (less efficient, but available)
            System.out.println("=== Extracting Individual Formats ===\n");

            Map<String, Object> meta = Extractor.extractMeta(html, "https://example.com");
            System.out.println("Meta tags count: " + meta.size());

            Map<String, Object> og = Extractor.extractOpenGraph(html, "https://example.com");
            System.out.println("Open Graph properties: " + og.size());

            // Library version
            System.out.println("\nMetaOxide version: " + Extractor.getVersion());

        } catch (MetaOxideException e) {
            System.err.println("Error extracting metadata: " + e.getMessage());
            System.err.println("Error code: " + e.getErrorCode());
            e.printStackTrace();
        }
    }
}
