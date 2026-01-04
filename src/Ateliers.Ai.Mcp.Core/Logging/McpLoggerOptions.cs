namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ロガーのオプションを表します。
/// </summary>
public sealed class McpLoggerOptions
{
    /// <summary>
    /// 記録する最小のログ レベルを取得または設定します。
    /// </summary>
    public McpLogLevel MinimumLevel { get; internal set; } = McpLogLevel.Information;

    /// <summary>
    /// ログ ファイルを保存するディレクトリのパスを取得または設定します。
    /// </summary>
    public string? LogDirectory { get; init; }

    /// <summary>
    /// コンソールへのログ出力を有効にするかどうかを示します。
    /// </summary>
    public bool EnableConsole { get; init; } = true;

    /// <summary>
    /// MCP ロガーのオプションの新しいインスタンスを初期化します。
    /// </summary>
    public McpLoggerOptions()
    {
    }

    /// <summary>
    /// MCP ロガーのオプションの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="minimumLevel">記録する最小のログレベル。</param>
    /// <param name="logDirectory">ログファイルを保存するディレクトリのパス。</param>
    /// <param name="enableConsole">コンソールへのログ出力を有効にするかどうか。</param>
    public McpLoggerOptions(McpLogLevel minimumLevel, string? logDirectory, bool enableConsole)
    {
        MinimumLevel = minimumLevel;
        LogDirectory = logDirectory;
        EnableConsole = enableConsole;
    }
}
