using Xunit;

namespace Snapshot.Tests;

public class VerifyChecksTests
{
    [Fact]
    public Task Run() => VerifyChecks.Run();
}