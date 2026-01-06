namespace Ateliers.Ai.Mcp.Context;

/// <summary>
/// MCP 実行コンテキストを表します。
/// </summary>
public sealed class McpExecutionContext : Ateliers.Context.ExecutionContext, IMcpExecutionContext
{
    /// <summary>
    /// 現在の MCP 実行コンテキストを取得します。
    /// </summary>
    public new static McpExecutionContext? Current => Ateliers.Context.ExecutionContext.Current as McpExecutionContext;

    /// <summary>
    /// ツール名を取得します。
    /// </summary>
    public string? ToolName { get; init; }

    /// <summary>
    /// MCP 実行コンテキスト の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="correlationId">相関 ID</param>
    /// <param name="toolName">ツール名</param>
    public McpExecutionContext(string correlationId, string? toolName)
        : base(correlationId, toolName)
    {
        ToolName = toolName;
    }

    /// <inheritdoc/>
    public McpExecutionContextScope BeginTool(string toolName)
    {
        return new McpExecutionContextScope(toolName);
    }
}
