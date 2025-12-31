using System.Collections.Concurrent;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// メモリログを管理する MCP ロガー
/// </summary>
public sealed class InMemoryMcpLogger : IMcpLogger, IMcpLogReader
{
    private readonly McpLoggerOptions _options;
    private readonly List<McpLogEntry> _entries = new();
    private readonly ConcurrentDictionary<string, List<McpLogEntry>> _logs = new();

    /// <summary>
    /// ログ エントリの読み取り専用リストを取得します。
    /// </summary>
    public IReadOnlyList<McpLogEntry> Entries => _entries;

    /// <summary>
    /// メモリロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options"> ロガーのオプション </param>
    public InMemoryMcpLogger(McpLoggerOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// ログ エントリを記録します。
    /// </summary>
    /// <param name="entry"> ログ エントリ </param>
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

    /// <inheritdoc/>
    public McpLogSession ReadByCorrelationId(string correlationId)
    {
        if (!_logs.TryGetValue(correlationId, out var entries))
        {
            return new McpLogSession
            {
                CorrelationId = correlationId,
                Entries = Array.Empty<McpLogEntry>()
            };
        }

        lock (entries)
        {
            return new McpLogSession
            {
                CorrelationId = correlationId,
                Entries = entries
                    .OrderBy(e => e.Timestamp)
                    .ToList()
            };
        }
    }

    /// <summary>
    /// 指定された相関 ID に対してログ エントリを追加します。
    /// </summary>
    internal void Append(string correlationId, McpLogEntry entry)
    {
        var list = _logs.GetOrAdd(
            correlationId,
            _ => new List<McpLogEntry>());

        lock (list)
        {
            list.Add(entry);
        }
    }

    /// <summary>
    /// 指定された相関 ID のログをクリアします。
    /// </summary>
    internal void Clear(string correlationId)
    {
        _logs.TryRemove(correlationId, out _);
    }

    /// <summary>
    /// すべてのログをクリアします。
    /// </summary>
    internal void ClearAll()
    {
        _logs.Clear();
    }
}
