#!/bin/bash

# MetaOxide WASM Build Script
# Builds WASM module for all targets

set -e

echo "🚀 Building MetaOxide WASM..."
echo ""

# Check for required tools
if ! command -v wasm-pack &> /dev/null; then
    echo "❌ Error: wasm-pack not found"
    echo "Install with: cargo install wasm-pack"
    exit 1
fi

if ! command -v npm &> /dev/null; then
    echo "❌ Error: npm not found"
    echo "Please install Node.js from https://nodejs.org/"
    exit 1
fi

# Install dependencies
echo "📦 Installing npm dependencies..."
npm install

echo ""
echo "🔨 Building WASM for web target..."
wasm-pack build --target web --out-dir pkg

echo ""
echo "🔨 Building WASM for Node.js target..."
wasm-pack build --target nodejs --out-dir pkg-node

echo ""
echo "🔨 Building WASM for bundler target..."
wasm-pack build --target bundler --out-dir pkg-bundler

echo ""
echo "📝 Compiling TypeScript..."
npx tsc

echo ""
echo "✅ Build complete!"
echo ""
echo "📊 Build artifacts:"
echo "  - pkg/          (Web target)"
echo "  - pkg-node/     (Node.js target)"
echo "  - pkg-bundler/  (Bundler target)"
echo "  - dist/         (TypeScript compiled)"
echo ""
echo "🧪 Run tests with: npm test"
echo "📦 Publish with: npm publish"
