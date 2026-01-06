namespace Ateliers.Ai.Mcp.Context;

/// <summary>
/// MCP 実行コンテキスト スコープを表します。
/// </summary>
public sealed class McpExecutionContextScope : IDisposable
{
    private readonly McpExecutionContext? _previous;

    /// <summary>
    /// McpExecutionContextScope の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="toolName">ツール名</param>
    public McpExecutionContextScope(string toolName)
    {
        _previous = McpExecutionContext.Current;

        var newContext = new McpExecutionContext(
            correlationId: Guid.NewGuid().ToString("N"),
            toolName: toolName
        );

        SetContext(newContext);
    }

    private static void SetContext(McpExecutionContext? context)
    {
        typeof(Ateliers.Context.ExecutionContext)
            .GetMethod("SetCurrent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.Invoke(null, [context]);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        SetContext(_previous);
        GC.SuppressFinalize(this);
    }
}
