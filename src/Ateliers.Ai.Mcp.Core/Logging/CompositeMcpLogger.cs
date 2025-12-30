namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// 複数の MCP ロガーにログを転送する MCP ロガーを表します。
/// </summary>
public sealed class CompositeMcpLogger : IMcpLogger
{
    private readonly IReadOnlyList<IMcpLogger> _loggers;

    /// <summary>
    /// CompositeMcpLogger の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="loggers"> 転送先の MCP ロガーのコレクション </param>
    /// <exception cref="ArgumentNullException"> <paramref name="loggers"/> が null の場合 </exception>
    public CompositeMcpLogger(IEnumerable<IMcpLogger> loggers)
    {
        _loggers = loggers?.ToList()
            ?? throw new ArgumentNullException(nameof(loggers));
    }

    /// <inheritdoc/>
    public void Log(McpLogEntry entry)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.Log(entry);
            }
            catch
            {
                // ログ失敗でプロセスを止めない
                // （必要ならここで Console.Error にだけ吐く）
            }
        }
    }

    /// <inheritdoc/>
    public void Trace(string message)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Trace, message));

    /// <inheritdoc/>
    public void Debug(string message)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Debug, message));

    /// <inheritdoc/>
    public void Info(string message)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Information, message));

    /// <inheritdoc/>
    public void Warn(string message)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Warning, message));

    /// <inheritdoc/>
    public void Error(string message, Exception? exception = null)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Error, message, exception));

    /// <inheritdoc/>
    public void Critical(string message, Exception? exception = null)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Critical, message, exception));
}
