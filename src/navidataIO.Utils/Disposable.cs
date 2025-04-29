// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

namespace navidataIO.Utils;

/// <summary>
/// Represents a disposable object.
/// </summary>
public class Disposable : IDisposable
{
    private readonly Action? _onDispose;

    /// <summary>
    /// Instantiates a new instance of the <see cref="Disposable"/> class.
    /// </summary>
    /// <param name="onDispose">The method to invoke on <see cref="IDisposable.Dispose"/>.</param>
    public Disposable(Action? onDispose)
    {
        _onDispose = onDispose;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _onDispose?.Invoke();
    }
}