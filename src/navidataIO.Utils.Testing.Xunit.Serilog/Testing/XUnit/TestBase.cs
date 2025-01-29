using Xunit.Abstractions;
using Serilog;
using Serilog.Core;

namespace navidataIO.Utils.Testing.Xunit;

/// <summary>
/// Redirects test log output to the default <see cref="ILogger"/>.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Logs additional test output.
    /// </summary>
    /// <remarks>The globally shared Logger is used to capture log output from external components logging to the same instance.</remarks>
    protected readonly ILogger Log;

    /// <summary>
    /// Gets the <see cref="LoggingLevelSwitch"/> that controls the minimum log level for this instance.
    /// </summary>
    protected LoggingLevelSwitch LevelSwitch { get; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Debug);

    /// <summary>
    /// Creates a new instance that logs to the given <see cref="ITestOutputHelper"/>.
    /// </summary>
    protected TestBase(ITestOutputHelper output, bool useSharedLogger = false) : this(new TestOutputSink(output), useSharedLogger)
    { }

    /// <summary>
    /// Creates a new instance that logs to the given <see cref="IMessageSink"/>.
    /// </summary>
    protected TestBase(IMessageSink messageSink, bool useSharedLogger = false): this(new TestOutputSink(messageSink), useSharedLogger)
    { }

    private TestBase(ILogEventSink sink, bool useSharedLogger)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .WriteTo.Sink(sink)
            .CreateLogger();

        if (useSharedLogger)
            Serilog.Log.Logger = logger;
        
        this.Log = logger;
    }

}
