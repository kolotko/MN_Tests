using Xunit;

namespace PerformanceTestsApi.Tests;

[CollectionDefinition("Test collection")]
public class SharedTestCollection
{
    public const string ApiEndpoint = "http://localhost:5024/example";
}