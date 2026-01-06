using System.Collections.Concurrent;
using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// メモリログを管理する MCP ロガー
/// </summary>
public sealed class InMemoryMcpLogger : InMemoryLogger, IMcpLogger, IMcpLogReader
{
    private readonly McpLoggerOptions _options;
    private readonly List<McpLogEntry> _mcpEntries = new();
    private readonly ConcurrentDictionary<string, List<McpLogEntry>> _logs = new();

    /// <summary>
    /// MCP ログ エントリの読み取り専用リストを取得します。
    /// </summary>
    public new IReadOnlyList<McpLogEntry> Entries => _mcpEntries;

    /// <summary>
    /// メモリロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">ロガーのオプション</param>
    public InMemoryMcpLogger(McpLoggerOptions options)
        : base(options)
    {
        _options = options;
    }

    /// <summary>
    /// MCP ログ エントリを記録します。
    /// </summary>
    /// <param name="entry">MCP ログ エントリ</param>
    public void Log(McpLogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        _mcpEntries.Add(entry);
        base.Log(entry);
        
        // 相関IDがある場合は _logs にも追加
        if (!string.IsNullOrEmpty(entry.CorrelationId))
        {
            Append(entry.CorrelationId, entry);
        }
    }

    /// <inheritdoc/>
    public override void Log(LogEntry entry)
    {
        if (entry is McpLogEntry mcpEntry)
        {
            Log(mcpEntry);
        }
        else
        {
            // 現在の MCP コンテキストから ToolName を取得
            var currentContext = Ai.Mcp.Context.McpExecutionContext.Current;
            
            Log(new McpLogEntry
            {
                Timestamp = entry.Timestamp,
                Level = entry.Level,
                LogText = entry.LogText,
                Message = entry.Message,
                Exception = entry.Exception,
                CorrelationId = entry.CorrelationId,
                Category = entry.Category,
                ToolName = currentContext?.ToolName,
                Properties = entry.Properties
            });
        }
    }

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
    /// 指定された相関IDに基づいてログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadByCorrelationId(string correlationId)
        => ReadByCorrelationId(correlationId);

    /// <inheritdoc/>
    public McpLogSession ReadLastSession()
    {
        var lastCorrelationId = _logs.Keys
            .Select(cid => new
            {
                CorrelationId = cid,
                LastTimestamp = _logs[cid].Max(e => e.Timestamp)
            })
            .OrderByDescending(x => x.LastTimestamp)
            .FirstOrDefault()?.CorrelationId;
        if (lastCorrelationId == null)
        {
            return new McpLogSession
            {
                CorrelationId = string.Empty,
                Entries = Array.Empty<McpLogEntry>()
            };
        }
        return ReadByCorrelationId(lastCorrelationId);
    }

    /// <summary>
    /// 最後のログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadLastSession()
        => ReadLastSession();

    /// <inheritdoc/>
    public McpLogSession ReadByCategory(string category)
    {
        var entries = _mcpEntries
            .Where(e => e.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true)
            .OrderBy(e => e.Timestamp)
            .ToList();

        return new McpLogSession
        {
            CorrelationId = string.Empty,
            Entries = entries
        };
    }

    /// <summary>
    /// 指定されたカテゴリに基づいてログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadByCategory(string category)
        => ReadByCategory(category);

    /// <inheritdoc/>
    public McpLogSession ReadByCorrelationIdAndCategory(string correlationId, string category)
    {
        var entries = _mcpEntries
            .Where(e => 
                e.CorrelationId?.Equals(correlationId, StringComparison.OrdinalIgnoreCase) == true &&
                e.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true)
            .OrderBy(e => e.Timestamp)
            .ToList();

        return new McpLogSession
        {
            CorrelationId = correlationId,
            Entries = entries
        };
    }

    /// <summary>
    /// 指定された相関IDとカテゴリに基づいてログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadByCorrelationIdAndCategory(string correlationId, string category)
        => ReadByCorrelationIdAndCategory(correlationId, category);

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
        _mcpEntries.Clear();
        base.Clear();
    }
}
