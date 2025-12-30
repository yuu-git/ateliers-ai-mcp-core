namespace Ateliers.Ai.Mcp.Logging.Context;

/// <summary>
/// MCP 実行コンテキスト スコープを表します。
/// </summary>
public sealed class McpExecutionContextScope : IDisposable
{
    private readonly McpExecutionContext? _previous;

    /// <summary>
    /// McpExecutionContextScope の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="toolName"> ツール名 </param>
    public McpExecutionContextScope(string toolName)
    {
        _previous = McpExecutionContext.Current;

        McpExecutionContext.Current = new McpExecutionContext(
            correlationId: Guid.NewGuid().ToString("N"),
            toolName: toolName
        );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        McpExecutionContext.Current = _previous;
    }
}
