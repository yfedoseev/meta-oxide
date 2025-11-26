# MetaOxide Performance Tuning Guide

Comprehensive guide to optimizing MetaOxide for maximum performance across all language bindings.

## Table of Contents

- [General Principles](#general-principles)
- [Language-Specific Optimizations](#language-specific-optimizations)
- [Caching Strategies](#caching-strategies)
- [Parallel Processing](#parallel-processing)
- [Memory Optimization](#memory-optimization)
- [Production Deployment](#production-deployment)

## General Principles

### 1. Extract Only What You Need

**Bad**: Extract everything when you only need social metadata
```rust
let metadata = extractor.extract_all()?;  // Extracts 13 formats
let og_title = metadata.get("og:title");
```

**Good**: Use specific extractors
```rust
let og = extractor.extract_opengraph()?;  // Only Open Graph
let og_title = og.title;
```

**Performance Impact**: 3-5x faster for selective extraction

### 2. Reuse HTTP Connections

**Bad**: Create new HTTP client for each request
```python
# Each request creates new connection
response = requests.get(url)
```

**Good**: Reuse session
```python
session = requests.Session()
# Reuses connections
response = session.get(url)
```

**Performance Impact**: 2-3x faster HTTP requests

### 3. Avoid Unnecessary Parsing

**Bad**: Parse same HTML multiple times
```javascript
const extractor1 = new MetaOxide(html, url);
const og = extractor1.extractOpenGraph();

const extractor2 = new MetaOxide(html, url);  // Parses again!
const twitter = extractor2.extractTwitterCard();
```

**Good**: Parse once, extract multiple
```javascript
const extractor = new MetaOxide(html, url);  // Parse once
const og = extractor.extractOpenGraph();
const twitter = extractor.extractTwitterCard();
```

**Performance Impact**: 10-20x faster

## Language-Specific Optimizations

### Rust

#### Use Rayon for Parallel Processing

```rust
use rayon::prelude::*;
use meta_oxide::MetaOxide;

let urls: Vec<String> = vec![/* ... */];

let results: Vec<_> = urls.par_iter()
    .map(|url| {
        let html = fetch_html(url).unwrap();
        let extractor = MetaOxide::new(&html, url).unwrap();
        extractor.extract_all().unwrap()
    })
    .collect();
```

**Performance**: Process 100,000 docs/sec on 16-core CPU

#### Avoid Cloning Large Strings

```rust
// Bad: Clones HTML string
fn process(html: String) {
    let extractor = MetaOxide::new(&html, url).unwrap();
}

// Good: Use references
fn process(html: &str) {
    let extractor = MetaOxide::new(html, url).unwrap();
}
```

#### Use Arc for Shared Data

```rust
use std::sync::Arc;
use std::thread;

let html = Arc::new(fetch_html(url));

let handles: Vec<_> = (0..4).map(|_| {
    let html_clone = Arc::clone(&html);
    thread::spawn(move || {
        let extractor = MetaOxide::new(&html_clone, url).unwrap();
        extractor.extract_all()
    })
}).collect();
```

### Python

#### Use Connection Pooling

```python
import requests
from requests.adapters import HTTPAdapter
from urllib3.util.retry import Retry

session = requests.Session()
retry = Retry(total=3, backoff_factor=0.1)
adapter = HTTPAdapter(max_retries=retry, pool_connections=100, pool_maxsize=100)
session.mount('http://', adapter)
session.mount('https://', adapter)

# Fast parallel requests
from concurrent.futures import ThreadPoolExecutor

with ThreadPoolExecutor(max_workers=10) as executor:
    futures = [executor.submit(extract_url, url) for url in urls]
    results = [f.result() for f in futures]
```

**Performance**: 10x faster than sequential requests

#### Use Process Pool for CPU-Intensive Work

```python
from multiprocessing import Pool
from meta_oxide import MetaOxide

def extract_metadata(args):
    html, url = args
    extractor = MetaOxide(html, url)
    return extractor.extract_all()

with Pool(processes=8) as pool:
    results = pool.map(extract_metadata, [(html, url) for html, url in data])
```

**Performance**: Scales linearly with CPU cores

#### Cache Results

```python
from functools import lru_cache
import hashlib

@lru_cache(maxsize=1000)
def extract_cached(html_hash: str, url: str) -> dict:
    html = get_html_from_cache(html_hash)
    extractor = MetaOxide(html, url)
    return extractor.extract_all()

# Usage
html_hash = hashlib.md5(html.encode()).hexdigest()
metadata = extract_cached(html_hash, url)
```

### Go

#### Use Worker Pools

```go
func extractWithWorkerPool(urls []string, workers int) []Metadata {
    jobs := make(chan string, len(urls))
    results := make(chan Metadata, len(urls))

    // Start workers
    var wg sync.WaitGroup
    for w := 0; w < workers; w++ {
        wg.Add(1)
        go func() {
            defer wg.Done()
            for url := range jobs {
                metadata, err := extractFromURL(url)
                if err == nil {
                    results <- metadata
                }
            }
        }()
    }

    // Send jobs
    for _, url := range urls {
        jobs <- url
    }
    close(jobs)

    // Wait and collect
    wg.Wait()
    close(results)

    var allResults []Metadata
    for result := range results {
        allResults = append(allResults, result)
    }

    return allResults
}
```

**Performance**: Process 50,000 URLs/sec with 100 workers

#### Reuse HTTP Client

```go
var client = &http.Client{
    Transport: &http.Transport{
        MaxIdleConns:        100,
        MaxIdleConnsPerHost: 100,
        IdleConnTimeout:     90 * time.Second,
    },
    Timeout: 10 * time.Second,
}

func extractFromURL(url string) (Metadata, error) {
    resp, err := client.Get(url)  // Reuses connections
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()

    body, _ := io.ReadAll(resp.Body)
    extractor, _ := metaoxide.NewExtractor(string(body), url)
    defer extractor.Free()

    return extractor.ExtractAll()
}
```

#### Use sync.Pool for Extractors

```go
var extractorPool = sync.Pool{
    New: func() interface{} {
        return &ExtractorWrapper{}
    },
}

type ExtractorWrapper struct {
    extractor *metaoxide.Extractor
}

func processWithPool(html, url string) Metadata {
    wrapper := extractorPool.Get().(*ExtractorWrapper)
    defer extractorPool.Put(wrapper)

    extractor, _ := metaoxide.NewExtractor(html, url)
    defer extractor.Free()

    return extractor.ExtractAll()
}
```

### Node.js

#### Use Worker Threads

```javascript
const { Worker } = require('worker_threads');

function extractInWorker(html, url) {
    return new Promise((resolve, reject) => {
        const worker = new Worker('./extract-worker.js', {
            workerData: { html, url }
        });

        worker.on('message', resolve);
        worker.on('error', reject);
        worker.on('exit', (code) => {
            if (code !== 0) {
                reject(new Error(`Worker stopped with exit code ${code}`));
            }
        });
    });
}

// extract-worker.js
const { parentPort, workerData } = require('worker_threads');
const { MetaOxide } = require('meta-oxide');

const { html, url } = workerData;
const extractor = new MetaOxide(html, url);
const metadata = extractor.extractAll();

parentPort.postMessage(metadata);
```

#### Connection Pooling with axios

```javascript
const axios = require('axios');
const http = require('http');
const https = require('https');

const client = axios.create({
    httpAgent: new http.Agent({ keepAlive: true, maxSockets: 100 }),
    httpsAgent: new https.Agent({ keepAlive: true, maxSockets: 100 }),
    timeout: 10000
});

async function extractFromURL(url) {
    const response = await client.get(url);
    const extractor = new MetaOxide(response.data, url);
    return extractor.extractAll();
}
```

#### Cluster Mode for Multi-Core

```javascript
const cluster = require('cluster');
const os = require('os');

if (cluster.isMaster) {
    const numCPUs = os.cpus().length;

    for (let i = 0; i < numCPUs; i++) {
        cluster.fork();
    }
} else {
    // Worker process
    const app = require('./app');
    app.listen(3000);
}
```

**Performance**: Utilize all CPU cores

### Java

#### Use Virtual Threads (Java 21+)

```java
import java.util.concurrent.Executors;

try (var executor = Executors.newVirtualThreadPerTaskExecutor()) {
    List<Future<Metadata>> futures = urls.stream()
        .map(url -> executor.submit(() -> extractFromURL(url)))
        .toList();

    List<Metadata> results = futures.stream()
        .map(f -> {
            try { return f.get(); }
            catch (Exception e) { return null; }
        })
        .filter(Objects::nonNull)
        .toList();
}
```

**Performance**: Handle millions of concurrent requests

#### Connection Pooling

```java
import java.net.http.HttpClient;
import java.time.Duration;

private static final HttpClient client = HttpClient.newBuilder()
    .connectTimeout(Duration.ofSeconds(10))
    .build();

public static Metadata extractFromURL(String url) throws Exception {
    HttpRequest request = HttpRequest.newBuilder()
        .uri(URI.create(url))
        .build();

    HttpResponse<String> response = client.send(request,
        HttpResponse.BodyHandlers.ofString());

    try (MetaOxide extractor = new MetaOxide(response.body(), url)) {
        return extractor.extractAll();
    }
}
```

#### Parallel Streams

```java
List<Metadata> results = urls.parallelStream()
    .map(url -> {
        try {
            return extractFromURL(url);
        } catch (Exception e) {
            return null;
        }
    })
    .filter(Objects::nonNull)
    .toList();
```

### C#

#### Use IHttpClientFactory

```csharp
// Startup.cs
services.AddHttpClient("metadata")
    .ConfigureHttpClient(client => {
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        MaxConnectionsPerServer = 100
    });

// Service
public class MetadataService {
    private readonly HttpClient _client;

    public MetadataService(IHttpClientFactory clientFactory) {
        _client = clientFactory.CreateClient("metadata");
    }

    public async Task<Dictionary<string, object>> ExtractAsync(string url) {
        string html = await _client.GetStringAsync(url);
        using var extractor = new MetaOxideExtractor(html, url);
        return extractor.ExtractAll();
    }
}
```

#### Parallel.ForEachAsync

```csharp
await Parallel.ForEachAsync(urls, new ParallelOptions {
    MaxDegreeOfParallelism = 10
}, async (url, ct) => {
    var metadata = await ExtractFromURLAsync(url);
    // Process metadata
});
```

#### Memory Pool

```csharp
using System.Buffers;

var pool = ArrayPool<byte>.Shared;
byte[] buffer = pool.Rent(1024 * 1024);  // 1MB buffer

try {
    // Use buffer
}
finally {
    pool.Return(buffer);
}
```

### WebAssembly

#### Initialize Once

```javascript
import init, { MetaOxide } from 'meta-oxide-wasm';

let wasmInitialized = false;
let initPromise = null;

async function ensureInit() {
    if (wasmInitialized) return;

    if (!initPromise) {
        initPromise = init();
    }

    await initPromise;
    wasmInitialized = true;
}

// Usage
await ensureInit();  // Only initializes once
```

#### Web Workers for Concurrency

```javascript
// main.js
const workers = [];
for (let i = 0; i < 4; i++) {
    workers.push(new Worker('extract-worker.js', { type: 'module' }));
}

let currentWorker = 0;

function extract(html, url) {
    return new Promise((resolve) => {
        const worker = workers[currentWorker];
        currentWorker = (currentWorker + 1) % workers.length;

        worker.postMessage({ html, url });
        worker.onmessage = (e) => resolve(e.data);
    });
}
```

## Caching Strategies

### Redis Cache (Python)

```python
import redis
import hashlib
import json
from meta_oxide import MetaOxide

redis_client = redis.Redis(host='localhost', port=6379, db=0)

def extract_with_cache(html: str, url: str) -> dict:
    # Create cache key
    cache_key = f"metadata:{hashlib.md5(html.encode()).hexdigest()}"

    # Check cache
    cached = redis_client.get(cache_key)
    if cached:
        return json.loads(cached)

    # Extract and cache
    extractor = MetaOxide(html, url)
    metadata = extractor.extract_all()

    redis_client.setex(cache_key, 3600, json.dumps(metadata))  # 1 hour TTL

    return metadata
```

### In-Memory LRU Cache (Node.js)

```javascript
const LRU = require('lru-cache');
const { MetaOxide } = require('meta-oxide');
const crypto = require('crypto');

const cache = new LRU({
    max: 10000,  // Max items
    maxAge: 1000 * 60 * 60  // 1 hour
});

function extractWithCache(html, url) {
    const key = crypto.createHash('md5').update(html).digest('hex');

    if (cache.has(key)) {
        return cache.get(key);
    }

    const extractor = new MetaOxide(html, url);
    const metadata = extractor.extractAll();

    cache.set(key, metadata);

    return metadata;
}
```

## Parallel Processing

### Optimal Worker Count

General formula: **Workers = CPU_Cores × 2** (for I/O-bound tasks)

```python
import multiprocessing

optimal_workers = multiprocessing.cpu_count() * 2
```

### Batch Processing

Process in batches to avoid memory issues:

```python
def process_in_batches(urls, batch_size=1000):
    results = []

    for i in range(0, len(urls), batch_size):
        batch = urls[i:i+batch_size]

        with ThreadPoolExecutor(max_workers=20) as executor:
            batch_results = list(executor.map(extract_from_url, batch))

        results.extend(batch_results)

    return results
```

## Memory Optimization

### Limit Concurrent Extractions

```go
// Semaphore pattern
sem := make(chan struct{}, 100)  // Max 100 concurrent

for _, url := range urls {
    sem <- struct{}{}  // Acquire
    go func(u string) {
        defer func() { <-sem }()  // Release

        extract(u)
    }(url)
}
```

### Stream Processing

For very large HTML documents:

```rust
// Process HTML in streaming fashion
use std::io::BufReader;

let reader = BufReader::new(file);
// MetaOxide handles streaming internally
```

## Production Deployment

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: metadata-extractor
spec:
  replicas: 4
  template:
    spec:
      containers:
      - name: extractor
        image: metaoxide-api:latest
        resources:
          requests:
            memory: "256Mi"
            cpu: "500m"
          limits:
            memory: "512Mi"
            cpu: "1000m"
        env:
        - name: WORKER_COUNT
          value: "4"
```

### Load Balancing

Distribute requests across multiple instances:

```nginx
upstream metaoxide_backend {
    least_conn;  # Route to least busy server

    server backend1:3000 max_fails=3 fail_timeout=30s;
    server backend2:3000 max_fails=3 fail_timeout=30s;
    server backend3:3000 max_fails=3 fail_timeout=30s;
    server backend4:3000 max_fails=3 fail_timeout=30s;
}
```

### Monitoring

Track key metrics:

```python
from prometheus_client import Counter, Histogram

extraction_duration = Histogram('extraction_duration_seconds',
                               'Time spent extracting metadata')
extraction_errors = Counter('extraction_errors_total',
                           'Total extraction errors')

@extraction_duration.time()
def extract_metadata(html, url):
    try:
        extractor = MetaOxide(html, url)
        return extractor.extract_all()
    except Exception as e:
        extraction_errors.inc()
        raise
```

## Performance Checklist

- [ ] Extract only needed metadata formats
- [ ] Reuse HTTP connections
- [ ] Implement caching (Redis, Memcached)
- [ ] Use parallel processing (workers, threads, processes)
- [ ] Set appropriate timeouts
- [ ] Monitor memory usage
- [ ] Profile hot paths
- [ ] Use connection pooling
- [ ] Implement rate limiting
- [ ] Set up load balancing
- [ ] Configure auto-scaling
- [ ] Monitor with metrics

## See Also

- [Benchmarks](/docs/performance/benchmarks.md)
- [Architecture Overview](/docs/architecture/architecture-overview.md)
- [Examples](/examples/real-world/)
