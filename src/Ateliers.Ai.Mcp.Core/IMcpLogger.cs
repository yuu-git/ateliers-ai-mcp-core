using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// MCP ロガーを表します。
/// </summary>
public interface IMcpLogger : ILogger
{
    /// <summary>
    /// ログエントリを記録します。
    /// </summary>
    /// <param name="entry">ログエントリ</param>
    void Log(McpLogEntry entry);
}