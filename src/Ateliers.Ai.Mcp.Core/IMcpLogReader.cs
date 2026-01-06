using Ateliers.Ai.Mcp.Logging;
using Ateliers.Logging;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// MCP ログリーダーを表します。
/// </summary>
public interface IMcpLogReader : ILogReader
{
    /// <summary>
    /// 指定された相関IDに基づいてMCPログセッションを読み取ります。
    /// </summary>
    /// <param name="correlationId">読み取りする相関ID</param>
    /// <returns>指定された相関IDに基づくMCPログセッション</returns>
    new McpLogSession ReadByCorrelationId(string correlationId);

    /// <summary>
    /// 最後のMCPログセッションを読み取ります。
    /// </summary>
    /// <returns>最後のMCPログセッション</returns>
    new McpLogSession ReadLastSession();

    /// <summary>
    /// 指定されたカテゴリに基づいてMCPログセッションを読み取ります。
    /// </summary>
    /// <param name="category">読み取りするカテゴリ</param>
    /// <returns>指定されたカテゴリに基づくMCPログセッション</returns>
    new McpLogSession ReadByCategory(string category);

    /// <summary>
    /// 指定された相関IDとカテゴリに基づいてMCPログセッションを読み取ります。
    /// </summary>
    /// <param name="correlationId">読み取りする相関ID</param>
    /// <param name="category">読み取りするカテゴリ</param>
    /// <returns>指定された相関IDとカテゴリに基づくMCPログセッション</returns>
    new McpLogSession ReadByCorrelationIdAndCategory(string correlationId, string category);
}
