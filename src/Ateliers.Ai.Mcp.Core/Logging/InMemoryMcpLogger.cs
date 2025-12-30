namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// インメモリ MCP ロガーを表します。
/// </summary>
public sealed class InMemoryMcpLogger : IMcpLogger
{
    private readonly McpLoggerOptions _options;
    private readonly List<McpLogEntry> _entries = new();

    /// <summary>
    /// ログ エントリの読み取り専用リストを取得します。
    /// </summary>
    public IReadOnlyList<McpLogEntry> Entries => _entries;

    /// <summary>
    /// インメモリ MCP ロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options"></param>
    public InMemoryMcpLogger(McpLoggerOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// ログ エントリを記録します。
    /// </summary>
    /// <param name="entry"> ログ エントリ。</param>
    public void Log(McpLogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        _entries.Add(entry);
    }

    /// <inheritdoc/>
    public void Trace(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Trace, message));

    /// <inheritdoc/>
    public void Debug(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Debug, message));

    /// <inheritdoc/>
    public void Info(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Information, message));

    /// <inheritdoc/>
    public void Warn(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Warning, message));

    /// <inheritdoc/>
    public void Error(string message, Exception? ex = null)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Error, message, ex));

    /// <inheritdoc/>
    public void Critical(string message, Exception? ex = null)
        => Log(McpLogEntryFactory.Create(McpLogLevel.Critical, message, ex));
}
