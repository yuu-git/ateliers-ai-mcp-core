using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// MCP ログリーダーを表します。
/// </summary>
public interface IMcpLogReader
{
    /// <summary>
    /// 指定された相関IDに基づいてMCPログセッションを読み取ります。
    /// </summary>
    /// <param name="correlationId"> 読み取りする相関ID </param>
    /// <returns> 指定された相関IDに基づくMCPログセッション </returns>
    McpLogSession ReadByCorrelationId(string correlationId);

    /// <summary>
    /// 最後のMCPログセッションを読み取ります。
    /// </summary>
    /// <returns> 最後のMCPログセッション </returns>
    McpLogSession ReadLastSession();
}
