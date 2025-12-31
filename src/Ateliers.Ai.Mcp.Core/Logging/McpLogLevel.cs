namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ログレベル
/// </summary>
public enum McpLogLevel
{
    /// <summary> 不明なログレベル </summary>
    UNKNOWN,
    /// <summary>トレースログ </summary>
    Trace,
    /// <summary> デバッグログ </summary>
    Debug,
    /// <summary> 情報ログ </summary>
    Information,
    /// <summary> 警告ログ </summary>
    Warning,
    /// <summary> エラーログ </summary>
    Error,
    /// <summary> 重大ログ </summary>
    Critical
}