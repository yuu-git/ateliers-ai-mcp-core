namespace Ateliers.Ai.Mcp.Logging.DependencyInjection;

/// <summary>
/// MCP ロギング ビルダーを表します。
/// </summary>
public sealed class McpLoggingBuilder
{
    internal IList<IMcpLogger> Loggers { get; } = new List<IMcpLogger>();
    internal McpLoggerOptions Options { get; } = new();

    /// <summary>
    /// 最小ログレベルを設定します。
    /// </summary>
    /// <param name="level"> ログレベル。</param>
    /// <returns> このビルダーのインスタンス。</returns>
    public McpLoggingBuilder SetMinimumLevel(McpLogLevel level)
    {
        Options.MinimumLevel = level;
        return this;
    }

    /// <summary>
    /// コンソール ロガーを追加します。
    /// </summary>
    /// <returns> このビルダーのインスタンス。</returns>
    public McpLoggingBuilder AddConsole()
    {
        Loggers.Add(new ConsoleMcpLogger(Options));
        return this;
    }

    /// <summary>
    /// ファイル ロガーを追加します。
    /// </summary>
    /// <param name="logDirectory"> ログ ディレクトリのパス。指定しない場合、デフォルトのログ ディレクトリが使用されます。</param>
    /// <returns> このビルダーのインスタンス。</returns>
    public McpLoggingBuilder AddFile(string? logDirectory = null)
    {
        var options = new McpLoggerOptions
        {
            MinimumLevel = Options.MinimumLevel,
            LogDirectory = logDirectory,
            EnableConsole = Options.EnableConsole
        };
        Loggers.Add(new FileMcpLogger(options));
        return this;
    }

    /// <summary>
    /// インメモリ ロガーを追加します。
    /// </summary>
    /// <param name="logger"> 追加されたインメモリ ロガー。</param>
    /// <returns> このビルダーのインスタンス。</returns>
    public McpLoggingBuilder AddInMemory(out InMemoryMcpLogger logger)
    {
        logger = new InMemoryMcpLogger(Options);
        Loggers.Add(logger);
        return this;
    }
}
