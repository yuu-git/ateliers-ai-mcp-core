using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// MCP ログリーダーを表します。
/// </summary>
public interface IMcpLogReader
{
    /// <summary>
    /// 指定された相関 ID に基づいて MCP ログ セッションを読み取ります。
    /// </summary>
    /// <param name="correlationId"> 読み取りする相関 ID </param>
    /// <returns></returns>
    McpLogSession ReadByCorrelationId(string correlationId);
}
