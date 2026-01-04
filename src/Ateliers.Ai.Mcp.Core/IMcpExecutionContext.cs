using Ateliers.Ai.Mcp.Context;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// MCP 実行コンテキストを管理するインターフェイス
/// </summary>
public interface IMcpExecutionContext
{
    /// <summary>
    /// 新しいツール実行コンテキストスコープを開始します。
    /// </summary>
    /// <param name="toolName"> ツール名 </param>
    /// <returns> MCP 実行コンテキストスコープ </returns>
    McpExecutionContextScope BeginTool(string toolName);
}
