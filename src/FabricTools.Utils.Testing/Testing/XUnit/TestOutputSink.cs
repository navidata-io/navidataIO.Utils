using Serilog.Events;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FabricTools.Utils.Testing.Xunit;

/// <summary>
/// A <see cref="Serilog.Core.ILogEventSink"/> that writes to the test output, acting as
/// an adapter for either <see cref="ITestOutputHelper"/> or <see cref="IMessageSink"/>.
/// </summary>
public class TestOutputSink : Serilog.Core.ILogEventSink
{
    private readonly ITestOutputHelper? _testOutputHelper;
    private readonly IMessageSink? _messageSink;

    /// <summary>
    /// Creates a new <see cref="TestOutputSink"/> that writes to the given <see cref="ITestOutputHelper"/>.
    /// </summary>
    public TestOutputSink(ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Creates a new <see cref="TestOutputSink"/> that writes to the given <see cref="IMessageSink"/>.
    /// </summary>
    public TestOutputSink(IMessageSink messageSink)
    {
        this._messageSink = messageSink;
    }

    /// <inheritdoc />
    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage();

        _messageSink?.OnMessage(new DiagnosticMessage(message));
        _testOutputHelper?.WriteLine(message);
    }
}
