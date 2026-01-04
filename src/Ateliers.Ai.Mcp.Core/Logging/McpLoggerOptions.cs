namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ロガーのオプションを表します。
/// </summary>
public sealed class McpLoggerOptions
{
    /// <summary>
    /// 記録する最小のログ レベルを取得または設定します。
    /// </summary>
    public McpLogLevel MinimumLevel { get; init; } = McpLogLevel.Information;

    /// <summary>
    /// ログ ファイルを保存するディレクトリのパスを取得または設定します。
    /// </summary>
    public string? LogDirectory { get; init; }

    /// <summary>
    /// コンソールへのログ出力を有効にするかどうかを示します。
    /// </summary>
    public bool EnableConsole { get; init; } = true;
}
