namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ログ エントリを表します。
/// </summary>
public sealed class McpLogEntry
{
    /// <summary>
    /// ログ エントリのタイムスタンプを取得します。
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    /// <summary>
    /// ログ レベルを取得します。
    /// </summary>
    public McpLogLevel Level { get; init; }

    /// <summary>
    /// ログ メッセージを取得します。
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// 関連する例外を取得します。
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// 相関 ID を取得します。
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// ツール名を取得します。
    /// </summary>
    public string? ToolName { get; init; }

    /// <summary>
    /// 追加のプロパティを取得します。
    /// </summary>
    public IDictionary<string, object?>? Properties { get; init; }
}