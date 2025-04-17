namespace navidataIO.Utils.Testing;

/// <summary>
/// A component with a dynamically-generated test folder.
/// The test folder is deleted when the instance is disposed.
/// </summary>
public abstract class HasTestFolder : IDisposable
{
    /// <summary>
    /// A temporary folder available for a single test run or test fixture.
    /// </summary>
    public TempFolder TestFolder { get; } = new();

    /// <summary>
    /// Deletes the test folder when the instance is disposed.
    /// </summary>
    public virtual void Dispose()
    {
        (TestFolder as IDisposable).Dispose();
    }
}
