using JetBrains.Annotations;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests.Fixtures;

/// <summary>
/// Creates an ILogger that's backed by the ITestOutputHelper
/// This makes it easy to include log entries from the SUT in the test output, if needed
/// </summary>
[UsedImplicitly]
public class TestOutputLoggerFixture
{
    public LoggerExternalScopeProvider Provider = new();
    
    public ILogger<T> CreateLogger<T>(ITestOutputHelper output) => new XUnitLogger<T>(output, Provider);
}