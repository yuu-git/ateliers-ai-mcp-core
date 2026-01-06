using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ログ エントリを表します。
/// </summary>
public sealed class McpLogEntry : LogEntry
{
    /// <summary>
    /// ツール名を取得します。
    /// </summary>
    public string? ToolName { get; init; }
}