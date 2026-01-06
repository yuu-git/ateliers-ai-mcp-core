using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// 複数の MCP ロガーにログを転送する MCP ロガーを表します。
/// </summary>
public sealed class CompositeMcpLogger : CompositeLogger, IMcpLogger
{
    private readonly IReadOnlyList<IMcpLogger> _mcpLoggers;

    /// <summary>
    /// CompositeMcpLogger の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="loggers">転送先の MCP ロガーのコレクション</param>
    /// <exception cref="ArgumentNullException"><paramref name="loggers"/> が null の場合</exception>
    public CompositeMcpLogger(IEnumerable<IMcpLogger> loggers)
        : base(loggers ?? throw new ArgumentNullException(nameof(loggers)), "MCP")
    {
        _mcpLoggers = loggers.ToList();
    }

    /// <summary>
    /// MCP ログエントリを記録します。
    /// </summary>
    /// <param name="entry">MCP ログエントリ</param>
    public void Log(McpLogEntry entry)
    {
        foreach (var logger in _mcpLoggers)
        {
            try
            {
                logger.Log(entry);
            }
            catch
            {
                // ログ失敗でプロセスを止めない
            }
        }
    }
}
