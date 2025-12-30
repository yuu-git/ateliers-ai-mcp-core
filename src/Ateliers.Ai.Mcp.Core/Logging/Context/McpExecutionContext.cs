namespace Ateliers.Ai.Mcp.Logging.Context;

/// <summary>
/// MCP 実行コンテキストを表します。
/// </summary>
public sealed class McpExecutionContext
{
    private static readonly AsyncLocal<McpExecutionContext?> _current = new();

    /// <summary>
    /// 現在の MCP 実行コンテキストを取得または設定します。
    /// </summary>
    public static McpExecutionContext? Current
    {
        get => _current.Value;
        internal set => _current.Value = value;
    }

    /// <summary>
    /// 相関 ID を取得します。
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// ツール名を取得します。
    /// </summary>
    public string? ToolName { get; }

    /// <summary>
    /// MCP 実行コンテキスト の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="correlationId"> 相関 ID </param>
    /// <param name="toolName"> ツール名 </param>
    public McpExecutionContext(string correlationId, string? toolName)
    {
        CorrelationId = correlationId;
        ToolName = toolName;
    }
}
