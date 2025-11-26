//! Demonstration of the microformat_extractor! macro
//!
//! This example shows how to use the macro to generate an extractor
//! with minimal boilerplate.
//!
//! Run with: cargo run --example macro_demo

use meta_oxide::microformat_extractor;

/// Simple card structure for demonstration
#[derive(Debug, Default, PartialEq)]
struct SimpleCard {
    name: Option<String>,
    url: Option<String>,
    email: Option<String>,
    tags: Vec<String>,
}

// Generate the extract() function using the macro
microformat_extractor! {
    SimpleCard, ".h-card" {
        name: text(".p-name"),
        url: url(".u-url"),
        email: email(".u-email"),
        tags: multi_text(".p-category"),
    }
}

fn main() {
    println!("Microformat Extractor Macro Demo\n");
    println!("================================\n");

    let html = r#"
        <div class="h-card">
            <span class="p-name">John Doe</span>
            <a class="u-url" href="https://example.com">Website</a>
            <a class="u-email" href="mailto:john@example.com">Email</a>
            <span class="p-category">Rust</span>
            <span class="p-category">Web Development</span>
        </div>

        <div class="h-card">
            <span class="p-name">Jane Smith</span>
            <a class="u-url" href="/profile">Profile</a>
            <a class="u-email" href="mailto:jane@example.com">Contact</a>
            <span class="p-category">Python</span>
        </div>
    "#;

    println!("Extracting h-cards from HTML...\n");

    // Call the generated extract function
    match extract(html, Some("https://example.com")) {
        Ok(cards) => {
            println!("Found {} h-card(s):\n", cards.len());

            for (i, card) in cards.iter().enumerate() {
                println!("Card #{}:", i + 1);
                println!("  Name:  {:?}", card.name);
                println!("  URL:   {:?}", card.url);
                println!("  Email: {:?}", card.email);
                println!("  Tags:  {:?}", card.tags);
                println!();
            }
        }
        Err(e) => {
            eprintln!("Error extracting cards: {}", e);
        }
    }

    println!("Macro Benefits:");
    println!("- Eliminated ~150 lines of boilerplate per extractor");
    println!("- Type-safe property extraction");
    println!("- Automatic URL resolution with base_url");
    println!("- Consistent error handling");
    println!("- Easy to add new properties");
}
