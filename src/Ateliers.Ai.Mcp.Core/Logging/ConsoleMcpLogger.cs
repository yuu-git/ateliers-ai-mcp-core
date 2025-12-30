namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// コンソールにログを出力する MCP ロガーを表します。
/// </summary>
public sealed class ConsoleMcpLogger : IMcpLogger
{
    private readonly McpLoggerOptions _options;

    /// <summary>
    /// コンソール MCP ロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options"> ロガーのオプション </param>
    public ConsoleMcpLogger(McpLoggerOptions options)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public void Log(McpLogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        var prefix = $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}]";
        Console.WriteLine($"{prefix} {entry.Message}");

        if (entry.Exception != null)
        {
            Console.WriteLine(entry.Exception);
        }
    }

    /// <inheritdoc/>
    public void Trace(string message) => Log(new() { Level = McpLogLevel.Trace, Message = message });

    /// <inheritdoc/>
    public void Debug(string message) => Log(new() { Level = McpLogLevel.Debug, Message = message });

    /// <inheritdoc/>
    public void Info(string message) => Log(new() { Level = McpLogLevel.Information, Message = message });

    /// <inheritdoc/>
    public void Warn(string message) => Log(new() { Level = McpLogLevel.Warning, Message = message });

    /// <inheritdoc/>
    public void Error(string message, Exception? ex = null) => Log(new() { Level = McpLogLevel.Error, Message = message, Exception = ex });

    /// <inheritdoc/>
    public void Critical(string message, Exception? ex = null) => Log(new() { Level = McpLogLevel.Critical, Message = message, Exception = ex });
}
