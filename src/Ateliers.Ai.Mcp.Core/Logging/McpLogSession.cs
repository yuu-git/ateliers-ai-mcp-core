namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ログ セッションを表します。
/// </summary>
public sealed class McpLogSession
{
    /// <summary>
    /// 相関 ID
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// ログ エントリのコレクション
    /// </summary>
    public IReadOnlyList<McpLogEntry> Entries { get; init; }
        = Array.Empty<McpLogEntry>();
}
