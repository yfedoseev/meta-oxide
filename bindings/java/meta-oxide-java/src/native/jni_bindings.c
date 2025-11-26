/**
 * JNI Bindings for MetaOxide
 *
 * This file implements the JNI bridge between Java and the MetaOxide C library.
 * It handles:
 * - Type conversions between Java and C
 * - Memory management for strings and structs
 * - Error handling and exception throwing
 * - Thread-safe operations
 *
 * @version 0.1.0
 * @license MIT OR Apache-2.0
 */

#include <jni.h>
#include <string.h>
#include <stdlib.h>
#include "../../../../../../include/meta_oxide.h"

// ===== Helper Functions =====

/**
 * Throw a MetaOxideException with the given message.
 */
static void throw_exception(JNIEnv *env, const char *message) {
    jclass exception_class = (*env)->FindClass(env, "io/github/yfedoseev/metaoxide/MetaOxideException");
    if (exception_class != NULL) {
        (*env)->ThrowNew(env, exception_class, message);
        (*env)->DeleteLocalRef(env, exception_class);
    }
}

/**
 * Throw a MetaOxideException with the given error code and message.
 */
static void throw_exception_with_code(JNIEnv *env, int error_code, const char *message) {
    jclass exception_class = (*env)->FindClass(env, "io/github/yfedoseev/metaoxide/MetaOxideException");
    if (exception_class == NULL) {
        return;
    }

    jmethodID constructor = (*env)->GetMethodID(env, exception_class, "<init>", "(ILjava/lang/String;)V");
    if (constructor == NULL) {
        (*env)->DeleteLocalRef(env, exception_class);
        return;
    }

    jstring j_message = (*env)->NewStringUTF(env, message);
    if (j_message == NULL) {
        (*env)->DeleteLocalRef(env, exception_class);
        return;
    }

    jobject exception = (*env)->NewObject(env, exception_class, constructor, error_code, j_message);
    if (exception != NULL) {
        (*env)->Throw(env, (jthrowable) exception);
        (*env)->DeleteLocalRef(env, exception);
    }

    (*env)->DeleteLocalRef(env, j_message);
    (*env)->DeleteLocalRef(env, exception_class);
}

/**
 * Get the last error from MetaOxide and throw a Java exception.
 */
static void throw_last_error(JNIEnv *env) {
    int error_code = meta_oxide_last_error();
    const char *error_message = meta_oxide_error_message();

    if (error_message != NULL && strlen(error_message) > 0) {
        throw_exception_with_code(env, error_code, error_message);
    } else {
        throw_exception(env, "Unknown error occurred in MetaOxide");
    }
}

/**
 * Convert a Java string to a C string.
 * Returns NULL if the Java string is null or empty.
 * The caller must free the returned string.
 */
static char *java_string_to_c(JNIEnv *env, jstring j_str) {
    if (j_str == NULL) {
        return NULL;
    }

    const char *utf_str = (*env)->GetStringUTFChars(env, j_str, NULL);
    if (utf_str == NULL) {
        return NULL;
    }

    // Check if string is empty
    if (strlen(utf_str) == 0) {
        (*env)->ReleaseStringUTFChars(env, j_str, utf_str);
        return NULL;
    }

    // Create a copy
    char *result = strdup(utf_str);
    (*env)->ReleaseStringUTFChars(env, j_str, utf_str);

    return result;
}

/**
 * Convert a C string to a Java string.
 * Returns NULL if the C string is NULL.
 */
static jstring c_string_to_java(JNIEnv *env, const char *c_str) {
    if (c_str == NULL) {
        return NULL;
    }
    return (*env)->NewStringUTF(env, c_str);
}

// ===== JNI Native Method Implementations =====

/**
 * Extract all metadata formats at once.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractAll(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    // Convert Java strings to C strings
    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);

    // Call C function
    struct MetaOxideResult *result = meta_oxide_extract_all(c_html, c_base_url);

    // Release Java string
    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) {
        free(c_base_url);
    }

    // Check for error
    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    // Build JSON result manually (simple concatenation)
    // Estimate size: all 11 fields + JSON structure
    size_t total_size = 1024; // Base size for structure
    if (result->meta != NULL) total_size += strlen(result->meta);
    if (result->open_graph != NULL) total_size += strlen(result->open_graph);
    if (result->twitter != NULL) total_size += strlen(result->twitter);
    if (result->json_ld != NULL) total_size += strlen(result->json_ld);
    if (result->microdata != NULL) total_size += strlen(result->microdata);
    if (result->microformats != NULL) total_size += strlen(result->microformats);
    if (result->rdfa != NULL) total_size += strlen(result->rdfa);
    if (result->dublin_core != NULL) total_size += strlen(result->dublin_core);
    if (result->manifest != NULL) total_size += strlen(result->manifest);
    if (result->oembed != NULL) total_size += strlen(result->oembed);
    if (result->rel_links != NULL) total_size += strlen(result->rel_links);

    char *json = (char *) malloc(total_size);
    if (json == NULL) {
        meta_oxide_result_free(result);
        throw_exception(env, "Out of memory");
        return NULL;
    }

    // Build JSON object
    strcpy(json, "{");

    if (result->meta != NULL) {
        strcat(json, "\"meta\":");
        strcat(json, result->meta);
    } else {
        strcat(json, "\"meta\":{}");
    }

    strcat(json, ",\"openGraph\":");
    if (result->open_graph != NULL) {
        strcat(json, result->open_graph);
    } else {
        strcat(json, "{}");
    }

    strcat(json, ",\"twitter\":");
    if (result->twitter != NULL) {
        strcat(json, result->twitter);
    } else {
        strcat(json, "{}");
    }

    strcat(json, ",\"jsonLd\":");
    if (result->json_ld != NULL) {
        strcat(json, result->json_ld);
    } else {
        strcat(json, "[]");
    }

    strcat(json, ",\"microdata\":");
    if (result->microdata != NULL) {
        strcat(json, result->microdata);
    } else {
        strcat(json, "[]");
    }

    strcat(json, ",\"microformats\":");
    if (result->microformats != NULL) {
        strcat(json, result->microformats);
    } else {
        strcat(json, "{}");
    }

    strcat(json, ",\"rdfa\":");
    if (result->rdfa != NULL) {
        strcat(json, result->rdfa);
    } else {
        strcat(json, "[]");
    }

    strcat(json, ",\"dublinCore\":");
    if (result->dublin_core != NULL) {
        strcat(json, result->dublin_core);
    } else {
        strcat(json, "{}");
    }

    strcat(json, ",\"manifest\":");
    if (result->manifest != NULL) {
        strcat(json, result->manifest);
    } else {
        strcat(json, "{}");
    }

    strcat(json, ",\"oembed\":");
    if (result->oembed != NULL) {
        strcat(json, result->oembed);
    } else {
        strcat(json, "{}");
    }

    strcat(json, ",\"relLinks\":");
    if (result->rel_links != NULL) {
        strcat(json, result->rel_links);
    } else {
        strcat(json, "{}");
    }

    strcat(json, "}");

    // Create Java string
    jstring j_result = c_string_to_java(env, json);

    // Free resources
    free(json);
    meta_oxide_result_free(result);

    return j_result;
}

/**
 * Extract standard HTML meta tags.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractMeta(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_meta(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract Open Graph metadata.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractOpenGraph(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_open_graph(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract Twitter Card metadata.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractTwitter(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_twitter(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract JSON-LD structured data.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractJsonLd(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_json_ld(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract Microdata.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractMicrodata(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_microdata(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract Microformats.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractMicroformats(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_microformats(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract RDFa structured data.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractRdfa(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_rdfa(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract Dublin Core metadata.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractDublinCore(
        JNIEnv *env, jclass cls, jstring html) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *result = meta_oxide_extract_dublin_core(c_html);
    (*env)->ReleaseStringUTFChars(env, html, c_html);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract Web App Manifest link.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractManifest(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_manifest(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Parse Web App Manifest JSON.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeParseManifest(
        JNIEnv *env, jclass cls, jstring json, jstring base_url) {

    const char *c_json = (*env)->GetStringUTFChars(env, json, NULL);
    if (c_json == NULL) {
        throw_exception(env, "Failed to convert JSON string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_parse_manifest(c_json, c_base_url);

    (*env)->ReleaseStringUTFChars(env, json, c_json);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract oEmbed endpoint discovery.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractOembed(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_oembed(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Extract rel-* link relationships.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeExtractRelLinks(
        JNIEnv *env, jclass cls, jstring html, jstring base_url) {

    const char *c_html = (*env)->GetStringUTFChars(env, html, NULL);
    if (c_html == NULL) {
        throw_exception(env, "Failed to convert HTML string");
        return NULL;
    }

    char *c_base_url = java_string_to_c(env, base_url);
    char *result = meta_oxide_extract_rel_links(c_html, c_base_url);

    (*env)->ReleaseStringUTFChars(env, html, c_html);
    if (c_base_url != NULL) free(c_base_url);

    if (result == NULL) {
        throw_last_error(env);
        return NULL;
    }

    jstring j_result = c_string_to_java(env, result);
    meta_oxide_string_free(result);
    return j_result;
}

/**
 * Get library version.
 */
JNIEXPORT jstring JNICALL
Java_io_github_yfedoseev_metaoxide_Extractor_nativeVersion(
        JNIEnv *env, jclass cls) {
    const char *version = meta_oxide_version();
    return c_string_to_java(env, version);
}
