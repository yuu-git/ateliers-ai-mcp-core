using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// MCP ロガーのオプションを表します。
/// </summary>
public sealed class McpLoggerOptions : LoggerOptions
{
    /// <summary>
    /// MCP ロガーのオプションの新しいインスタンスを初期化します。
    /// </summary>
    public McpLoggerOptions()
    {
        Category = "MCP";
    }

    /// <summary>
    /// MCP ロガーのオプションの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="minimumLevel">記録する最小のログレベル</param>
    /// <param name="logDirectory">ログファイルを保存するディレクトリのパス</param>
    /// <param name="enableConsole">コンソールへのログ出力を有効にするかどうか</param>
    public McpLoggerOptions(LogLevel minimumLevel, string? logDirectory, bool enableConsole)
        : base(minimumLevel, logDirectory, enableConsole, "MCP")
    {
    }
}
