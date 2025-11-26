#!/usr/bin/env python3
"""
Example demonstrating Recipe JSON-LD extraction using meta_oxide

This example shows how to extract Schema.org Recipe structured data
from HTML pages, which is commonly used by food blogs and recipe websites.
"""

import json

import meta_oxide


def example_basic_recipe():
    """Extract a basic recipe with minimal fields"""
    html = """
    <html>
    <head>
        <title>Simple Pancake Recipe</title>
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "Recipe",
            "name": "Classic Pancakes",
            "description": "Fluffy homemade pancakes that are perfect for breakfast",
            "image": "https://example.com/pancakes.jpg"
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    print("=" * 60)
    print("Example 1: Basic Recipe")
    print("=" * 60)

    recipes = meta_oxide.extract_jsonld(html)
    for recipe in recipes:
        if recipe.get("@type") == "Recipe":
            print(f"Name: {recipe.get('name')}")
            print(f"Description: {recipe.get('description')}")
            print(f"Image: {recipe.get('image')}")
    print()


def example_complete_recipe():
    """Extract a complete recipe with all fields"""
    html = """
    <html>
    <head>
        <title>Grandma's Apple Pie Recipe</title>
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "Recipe",
            "name": "Grandma's Apple Pie",
            "description": "A classic homemade apple pie recipe passed down through generations",
            "image": [
                "https://example.com/apple-pie-1.jpg",
                "https://example.com/apple-pie-2.jpg"
            ],
            "author": {
                "@type": "Person",
                "name": "Jane Smith"
            },
            "datePublished": "2024-01-15",
            "prepTime": "PT30M",
            "cookTime": "PT1H",
            "totalTime": "PT1H30M",
            "recipeYield": "8 servings",
            "recipeCategory": "Dessert",
            "recipeCuisine": "American",
            "recipeIngredient": [
                "6 cups thinly sliced apples",
                "3/4 cup white sugar",
                "2 tablespoons all-purpose flour",
                "3/4 teaspoon ground cinnamon",
                "1/4 teaspoon ground nutmeg",
                "1 recipe pastry for a 9 inch double crust pie"
            ],
            "recipeInstructions": [
                "Preheat oven to 425 degrees F (220 degrees C)",
                "Combine sugar, flour, cinnamon, and nutmeg in a bowl",
                "Mix in apples until evenly coated",
                "Place bottom crust in pie pan and fill with apple mixture",
                "Cover with top crust and seal edges",
                "Bake for 40-50 minutes until crust is golden brown"
            ],
            "nutrition": {
                "@type": "NutritionInformation",
                "calories": "410 calories",
                "carbohydrateContent": "58g",
                "fatContent": "19g",
                "proteinContent": "4g"
            },
            "aggregateRating": {
                "@type": "AggregateRating",
                "ratingValue": "4.9",
                "reviewCount": "523"
            }
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    print("=" * 60)
    print("Example 2: Complete Recipe with All Fields")
    print("=" * 60)

    recipes = meta_oxide.extract_jsonld(html)
    for recipe in recipes:
        if recipe.get("@type") == "Recipe":
            print(f"Name: {recipe.get('name')}")
            print(f"Description: {recipe.get('description')}")
            print(f"Category: {recipe.get('recipeCategory')}")
            print(f"Cuisine: {recipe.get('recipeCuisine')}")
            print(f"Prep Time: {recipe.get('prepTime')}")
            print(f"Cook Time: {recipe.get('cookTime')}")
            print(f"Total Time: {recipe.get('totalTime')}")
            print(f"Yield: {recipe.get('recipeYield')}")
            print(f"Date Published: {recipe.get('datePublished')}")

            # Parse complex fields that are JSON strings
            if "recipeIngredient" in recipe:
                ingredients = json.loads(recipe["recipeIngredient"])
                print(f"\nIngredients ({len(ingredients)}):")
                for i, ingredient in enumerate(ingredients, 1):
                    print(f"  {i}. {ingredient}")

            if "recipeInstructions" in recipe:
                instructions = json.loads(recipe["recipeInstructions"])
                print(f"\nInstructions ({len(instructions)} steps):")
                for i, step in enumerate(instructions, 1):
                    print(f"  {i}. {step}")

            if "author" in recipe:
                author = json.loads(recipe["author"])
                print(f"\nAuthor: {author.get('name')}")

            if "nutrition" in recipe:
                nutrition = json.loads(recipe["nutrition"])
                print("\nNutrition Info:")
                print(f"  Calories: {nutrition.get('calories')}")
                print(f"  Carbs: {nutrition.get('carbohydrateContent')}")
                print(f"  Fat: {nutrition.get('fatContent')}")
                print(f"  Protein: {nutrition.get('proteinContent')}")

            if "aggregateRating" in recipe:
                rating = json.loads(recipe["aggregateRating"])
                print(
                    f"\nRating: {rating.get('ratingValue')} ⭐ ({rating.get('reviewCount')} reviews)"
                )
    print()


def example_extract_all():
    """Extract recipe using extract_all() which includes all metadata"""
    html = """
    <html>
    <head>
        <title>Quick Pasta Recipe</title>
        <meta name="description" content="Easy pasta recipe">
        <meta property="og:title" content="Quick Pasta">
        <script type="application/ld+json">
        {
            "@context": "https://schema.org",
            "@type": "Recipe",
            "name": "Quick Spaghetti Carbonara",
            "description": "A fast and delicious Italian pasta dish",
            "prepTime": "PT10M",
            "cookTime": "PT15M",
            "recipeYield": "4 servings",
            "recipeIngredient": [
                "400g spaghetti",
                "200g pancetta",
                "4 eggs",
                "100g parmesan cheese"
            ]
        }
        </script>
    </head>
    <body></body>
    </html>
    """

    print("=" * 60)
    print("Example 3: Using extract_all()")
    print("=" * 60)

    data = meta_oxide.extract_all(html)

    print("Available metadata types:")
    for key in data.keys():
        print(f"  - {key}")

    print("\nPage title (from meta):", data["meta"].get("title"))
    print("Open Graph title:", data["opengraph"].get("title"))

    print("\nJSON-LD Recipe:")
    for recipe in data.get("jsonld", []):
        if recipe.get("@type") == "Recipe":
            print(f"  Name: {recipe.get('name')}")
            print(f"  Prep Time: {recipe.get('prepTime')}")
            print(f"  Cook Time: {recipe.get('cookTime')}")
            print(f"  Yield: {recipe.get('recipeYield')}")
    print()


def main():
    """Run all examples"""
    print("\n" + "=" * 60)
    print("JSON-LD Recipe Extraction Examples")
    print("=" * 60 + "\n")

    example_basic_recipe()
    example_complete_recipe()
    example_extract_all()

    print("=" * 60)
    print("All examples completed successfully!")
    print("=" * 60 + "\n")


if __name__ == "__main__":
    main()
