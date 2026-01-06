using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// コンソールにログを出力する MCP ロガーを表します。
/// </summary>
public sealed class ConsoleMcpLogger : ConsoleLogger, IMcpLogger
{
    private readonly McpLoggerOptions _options;

    /// <summary>
    /// コンソール MCP ロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">ロガーのオプション</param>
    public ConsoleMcpLogger(McpLoggerOptions options)
        : base(options)
    {
        _options = options;
    }

    /// <summary>
    /// MCP ログエントリを記録します。
    /// </summary>
    /// <param name="entry">MCP ログエントリ</param>
    public void Log(McpLogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        var categoryPrefix = !string.IsNullOrEmpty(entry.Category) ? $"[{entry.Category}] " : "";
        var toolPrefix = !string.IsNullOrEmpty(entry.ToolName) ? $"[Tool:{entry.ToolName}] " : "";
        var prefix = $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] {categoryPrefix}{toolPrefix}";
        Console.WriteLine($"{prefix}{entry.LogText}");

        if (entry.Exception != null)
        {
            Console.WriteLine(entry.Exception);
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
}
