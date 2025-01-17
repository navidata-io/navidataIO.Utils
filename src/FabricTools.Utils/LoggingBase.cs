// Copyright (c) 2024 pbi-tools Ltd, London

using Microsoft.Extensions.Logging.Abstractions;

namespace FabricTools.Utils;

/// <summary>
/// Base class, providing access to a <see cref="ILoggerFactory"/> as well as a local <see cref="ILogger"/> instance.
/// </summary>
public abstract class LoggingBase
{
    /// <summary>
    /// Gets the <see cref="ILoggerFactory"/> for this instance.
    /// </summary>
    protected readonly ILoggerFactory LoggerFactory;

    /// <summary>
    /// Gets the <see cref="ILogger"/> for this instance.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Instantiates a new instance of the <see cref="LoggingBase"/> class.
    /// </summary>
    /// <param name="loggerFactory">An optional <see cref="ILoggerFactory"/>, used to create the <see cref="ILogger"/> for this instance.</param>
    protected LoggingBase(ILoggerFactory? loggerFactory)
    {
        Logger = (LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance)
            .CreateLogger(this.GetType());
    }
}