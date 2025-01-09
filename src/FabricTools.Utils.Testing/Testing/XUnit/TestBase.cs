using Xunit.Abstractions;
using Serilog;

namespace FabricTools.Utils.Testing.Xunit;

/// <summary>
/// Redirects test log output to the default <see cref="ILogger"/>.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Logs additional test output.
    /// </summary>
    protected readonly ILogger Log;

    /// <summary>
    /// Creates a new instance that logs to the given <see cref="ITestOutputHelper"/>.
    /// </summary>
    protected TestBase(ITestOutputHelper output)
    {
        this.Log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(new TestOutputSink(output))
            .CreateLogger();
    }

    /// <summary>
    /// Creates a new instance that logs to the given <see cref="IMessageSink"/>.
    /// </summary>
    protected TestBase(IMessageSink messageSink)
    {
        this.Log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(new TestOutputSink(messageSink))
            .CreateLogger();
    }

}
