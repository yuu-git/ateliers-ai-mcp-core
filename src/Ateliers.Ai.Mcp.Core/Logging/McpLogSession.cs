using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ログ セッションを表します。
/// </summary>
public sealed class McpLogSession : LogSession
{
    /// <summary>
    /// ログ エントリのコレクション（MCP固有）
    /// </summary>
    public new IReadOnlyList<McpLogEntry> Entries
    {
        get => base.Entries.Cast<McpLogEntry>().ToList();
        init => base.Entries = value;
    }
}
