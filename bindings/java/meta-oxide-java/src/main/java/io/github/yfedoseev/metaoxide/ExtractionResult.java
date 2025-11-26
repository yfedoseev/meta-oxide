package io.github.yfedoseev.metaoxide;

import java.util.List;
import java.util.Map;

/**
 * Complete extraction result containing all metadata formats extracted from HTML.
 * <p>
 * This class represents the result of calling {@link Extractor#extractAll(String, String)},
 * containing all 13 supported metadata formats:
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
 * @since 0.1.0
 */
public class ExtractionResult {

    /**
     * Standard HTML meta tags (title, description, keywords, canonical, etc.)
     */
    public final Map<String, Object> meta;

    /**
     * Open Graph Protocol metadata for social sharing (Facebook, LinkedIn, WhatsApp)
     */
    public final Map<String, Object> openGraph;

    /**
     * Twitter Cards metadata for Twitter/X link previews
     */
    public final Map<String, Object> twitter;

    /**
     * JSON-LD structured data (Schema.org) for search engines and AI
     */
    public final List<Object> jsonLd;

    /**
     * Microdata structured data embedded in HTML attributes
     */
    public final List<Object> microdata;

    /**
     * Microformats (h-card, h-entry, h-event, etc.) for IndieWeb
     */
    public final Map<String, Object> microformats;

    /**
     * RDFa (Resource Description Framework in Attributes) for semantic web
     */
    public final List<Object> rdfa;

    /**
     * Dublin Core metadata elements for academic and library content
     */
    public final Map<String, Object> dublinCore;

    /**
     * Web App Manifest metadata for Progressive Web Apps
     */
    public final Map<String, Object> manifest;

    /**
     * oEmbed endpoint discovery for embedded content (YouTube, Twitter, etc.)
     */
    public final Map<String, Object> oembed;

    /**
     * rel-* link relationships (canonical, alternate, feed, etc.)
     */
    public final Map<String, Object> relLinks;

    /**
     * Constructs a new ExtractionResult with all metadata fields.
     *
     * @param meta         standard HTML meta tags
     * @param openGraph    Open Graph metadata
     * @param twitter      Twitter Cards metadata
     * @param jsonLd       JSON-LD structured data
     * @param microdata    Microdata structured data
     * @param microformats Microformats data
     * @param rdfa         RDFa structured data
     * @param dublinCore   Dublin Core metadata
     * @param manifest     Web App Manifest metadata
     * @param oembed       oEmbed endpoint information
     * @param relLinks     rel-* link relationships
     */
    public ExtractionResult(
            Map<String, Object> meta,
            Map<String, Object> openGraph,
            Map<String, Object> twitter,
            List<Object> jsonLd,
            List<Object> microdata,
            Map<String, Object> microformats,
            List<Object> rdfa,
            Map<String, Object> dublinCore,
            Map<String, Object> manifest,
            Map<String, Object> oembed,
            Map<String, Object> relLinks) {
        this.meta = meta;
        this.openGraph = openGraph;
        this.twitter = twitter;
        this.jsonLd = jsonLd;
        this.microdata = microdata;
        this.microformats = microformats;
        this.rdfa = rdfa;
        this.dublinCore = dublinCore;
        this.manifest = manifest;
        this.oembed = oembed;
        this.relLinks = relLinks;
    }

    /**
     * Returns a string representation of this extraction result showing which formats have data.
     *
     * @return a summary string
     */
    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder("ExtractionResult{");
        if (meta != null && !meta.isEmpty()) {
            sb.append("meta=").append(meta.size()).append(" items, ");
        }
        if (openGraph != null && !openGraph.isEmpty()) {
            sb.append("openGraph=").append(openGraph.size()).append(" items, ");
        }
        if (twitter != null && !twitter.isEmpty()) {
            sb.append("twitter=").append(twitter.size()).append(" items, ");
        }
        if (jsonLd != null && !jsonLd.isEmpty()) {
            sb.append("jsonLd=").append(jsonLd.size()).append(" items, ");
        }
        if (microdata != null && !microdata.isEmpty()) {
            sb.append("microdata=").append(microdata.size()).append(" items, ");
        }
        if (microformats != null && !microformats.isEmpty()) {
            sb.append("microformats=").append(microformats.size()).append(" types, ");
        }
        if (rdfa != null && !rdfa.isEmpty()) {
            sb.append("rdfa=").append(rdfa.size()).append(" items, ");
        }
        if (dublinCore != null && !dublinCore.isEmpty()) {
            sb.append("dublinCore=").append(dublinCore.size()).append(" items, ");
        }
        if (manifest != null && !manifest.isEmpty()) {
            sb.append("manifest=present, ");
        }
        if (oembed != null && !oembed.isEmpty()) {
            sb.append("oembed=present, ");
        }
        if (relLinks != null && !relLinks.isEmpty()) {
            sb.append("relLinks=").append(relLinks.size()).append(" items, ");
        }

        // Remove trailing ", " if present
        if (sb.length() > 18 && sb.substring(sb.length() - 2).equals(", ")) {
            sb.setLength(sb.length() - 2);
        }

        sb.append("}");
        return sb.toString();
    }
}
