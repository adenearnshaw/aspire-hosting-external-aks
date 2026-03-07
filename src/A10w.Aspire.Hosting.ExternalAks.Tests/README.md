# A10w.Aspire.Hosting.ExternalAks.Tests

Comprehensive test suite for the A10w.Aspire.Hosting.ExternalAks package.

## Test Coverage

### Unit Tests

#### ExternalAksServiceOptionsTests
- ✅ Options validation for all required properties
- ✅ Port range validation (1-65535)
- ✅ Empty/null string validation
- ✅ Default values (RemotePort defaults to 80)
- ✅ Edge cases for valid port ranges

#### ExternalAksServiceResourceTests
- ✅ Resource creation with correct properties
- ✅ Connection string expression generation
- ✅ Interface implementation verification
- ✅ LocalPort property validation
- ✅ Relationship between resources

### Integration Tests

#### ExternalAksServiceBuilderExtensionsTests
- ✅ Resource builder creation
- ✅ Argument validation (null checks)
- ✅ Configuration callback validation
- ✅ Port-forward executable creation
- ✅ External service resource creation
- ✅ HTTP endpoint creation
- ✅ Custom remote port handling

#### ExternalAksServiceHealthCheckExtensionsTests
- ✅ Health check addition to resources
- ✅ Path validation
- ✅ Fluent API chaining
- ✅ Various health check paths

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run tests with verbose output
```bash
dotnet test --verbosity detailed
```

### Run specific test class
```bash
dotnet test --filter "FullyQualifiedName~ExternalAksServiceOptionsTests"
```

## Test Dependencies

- **xUnit** - Testing framework
- **FluentAssertions** - Assertion library for more readable tests
- **Aspire.Hosting.Testing** - Aspire testing utilities
- **coverlet.collector** - Code coverage collection

## CI/CD Integration

Tests are automatically run in GitHub Actions workflows:
- **PR Validation**: Runs all tests on every pull request
- **Release**: Runs all tests before creating a release

## Test Structure

```
src/
└── A10w.Aspire.Hosting.ExternalAks.Tests/
    ├── ExternalAksServiceOptionsTests.cs
    ├── ExternalAksServiceBuilderExtensionsTests.cs
    ├── ExternalAksServiceResourceTests.cs
    └── ExternalAksServiceHealthCheckExtensionsTests.cs
```

## Adding New Tests

When adding new functionality to the package:
1. Add corresponding unit tests for business logic
2. Add integration tests for builder/extension methods
3. Ensure test names clearly describe what they test
4. Use FluentAssertions for readable assertions
5. Follow the Arrange-Act-Assert pattern
