# Context7 MCP Documentation Lookup Instructions

## Overview

The Context7 MCP (via Upstash) provides access to comprehensive documentation and code examples from official sources. Use these tools to look up accurate, up-to-date information about libraries, frameworks, and APIs.

## Available Tools

### 1. `mcp_upstash_conte_resolve-library-id`
Resolves library names to Context7-compatible library IDs.

**When to use:**
- Finding the correct library ID for documentation lookup
- Discovering available documentation sources for a library
- Comparing multiple documentation sources by reputation and snippet count

**Parameters:**
- `libraryName`: The name of the library (e.g., "aspire", "react", "kubernetes")
- `query`: A detailed description of what you need (helps improve matching)

**Returns:**
- Library ID (format: `/org/project` or `/org/project/version`)
- Description and metadata
- Code snippet count
- Source reputation (High, Medium, Low, Unknown)
- Benchmark score (higher is better, max 100)
- Available versions

### 2. `mcp_upstash_conte_query-docs`
Queries documentation for specific topics within a library.

**When to use:**
- Looking up implementation patterns
- Finding code examples
- Understanding API usage
- Learning best practices
- Troubleshooting specific scenarios

**Parameters:**
- `libraryId`: The Context7 library ID from resolve-library-id
- `query`: Detailed question or topic description

**Returns:**
- Relevant documentation sections with source URLs
- Code examples and snippets
- Implementation guidance

## Aspire-Specific Library IDs

Use these pre-verified library IDs for .NET Aspire documentation:

### Primary Sources (Recommended)

1. **`/websites/learn_microsoft_en-us_dotnet_aspire`**
   - Microsoft Learn official documentation
   - 2,506 code snippets
   - High source reputation
   - Best for: Tutorials, getting started, comprehensive guides

2. **`/microsoft/aspire.dev`**
   - Official Aspire product site
   - 1,865 code snippets
   - Benchmark score: 73
   - Best for: Marketing info, integration catalog, community resources

3. **`/dotnet/aspire`**
   - GitHub repository documentation
   - 1,185 code snippets
   - Benchmark score: 71.5
   - Best for: Source code examples, implementation details

4. **`/dotnet/docs-aspire`**
   - Conceptual documentation
   - 482 code snippets
   - Benchmark score: 71.4
   - Best for: Architecture, concepts, design patterns

### Community Resources

5. **`/communitytoolkit/aspire`**
   - .NET Aspire Community Toolkit
   - 311 code snippets
   - Benchmark score: 64.2
   - Best for: Community integrations (Golang, Java, Node.js, Ollama)

## Common Aspire Query Topics

Use Context7 for these Aspire-related queries:

### Resource Orchestration
```
Query: "How to create custom resources that implement IResource interface, 
including resource builder extensions, custom resource types, and lifecycle management"
```

### Kubernetes Integration
```
Query: "Aspire Kubernetes hosting integration, manifest generation, 
deployment to AKS, and external cluster configuration"
```

### Service Discovery
```
Query: "Service discovery patterns in Aspire, connection strings, 
endpoint resolution, and service-to-service communication"
```

### External Services
```
Query: "Adding external services with AddExternalService, 
static URLs, dynamic configuration, and port forwarding"
```

### Hosting Extensions
```
Query: "Creating Aspire hosting extensions, service builder patterns, 
options configuration, and extensibility points"
```

### Deployment
```
Query: "Aspire deployment strategies, manifest publishing, 
container registry configuration, and production deployment"
```

## Best Practices

### 1. Start with Library Resolution
Always resolve the library ID first to ensure you're querying the right source:
```
resolve-library-id → query-docs
```

### 2. Be Specific in Queries
Provide detailed, specific queries rather than broad topics:
- ❌ "How does Aspire work?"
- ✅ "How to implement custom resource annotations in Aspire AppHost for external Kubernetes services"

### 3. Choose the Right Source
- For tutorials and learning: Use Microsoft Learn
- For implementation details: Use GitHub repository
- For community patterns: Use Community Toolkit

### 4. Include Context
Mention your specific scenario in the query:
```
"How to configure port forwarding for external AKS clusters in Aspire, 
including automatic tunnel setup and connection string generation for 
services running outside the local development environment"
```

### 5. Combine Multiple Sources
For comprehensive understanding, query multiple library IDs:
1. Microsoft Learn → Conceptual understanding
2. GitHub repository → Implementation examples
3. Community Toolkit → Advanced scenarios

## Example Workflow

### Scenario: Creating Custom Aspire Resources

```markdown
Step 1: Resolve library ID
Tool: mcp_upstash_conte_resolve-library-id
Input: 
  - libraryName: "aspire"
  - query: "custom resource implementation"

Step 2: Query documentation
Tool: mcp_upstash_conte_query-docs
Input:
  - libraryId: "/websites/learn_microsoft_en-us_dotnet_aspire"
  - query: "How to create custom Aspire resources that implement IResource, 
    including resource builder extensions, custom annotations, lifecycle hooks, 
    and integration with manifest generation"

Step 3: Apply learned patterns to implementation
```

## When NOT to Use Context7

- For runtime errors or debugging (check logs first)
- For version-specific bugs (check GitHub issues)
- For workspace-specific code (use semantic_search)
- For simple syntax questions (check IntelliSense)

## Integration with This Project

This project (`A10w.Aspire.Hosting.ExternalAks`) extends Aspire to support external AKS clusters. Use Context7 for:

1. **Understanding Aspire extensibility patterns**
   - How to implement `IResource`
   - Builder pattern conventions
   - Options configuration

2. **Learning Kubernetes integration**
   - Manifest generation
   - External service configuration
   - Port forwarding patterns

3. **Service discovery patterns**
   - Connection string formats
   - Endpoint resolution
   - Configuration injection

4. **Deployment strategies**
   - Publishing to Kubernetes
   - Azure integration
   - Production considerations

## Error Handling

If Context7 returns insufficient results:
1. Refine your query with more specific terms
2. Try a different library ID (e.g., switch from Learn to GitHub)
3. Break complex queries into smaller, focused queries
4. Include code patterns or method names in your query

## Additional Resources

After using Context7, cross-reference with:
- Official GitHub repository: https://github.com/dotnet/aspire
- Microsoft Learn: https://learn.microsoft.com/dotnet/aspire
- Community Toolkit: https://github.com/CommunityToolkit/Aspire
