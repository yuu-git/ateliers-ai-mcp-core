using System.Text;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// ファイルにログを出力する MCP ロガーを表します。
/// </summary>
public sealed class FileMcpLogger : IMcpLogger
{
    private readonly string _logDir;
    private readonly McpLoggerOptions _options;

    /// <summary>
    /// ファイル MCP ロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options"> ロガーのオプション </param>
    public FileMcpLogger(McpLoggerOptions options)
    {
        _options = options;
        _logDir = options.LogDirectory ?? Path.Combine(AppContext.BaseDirectory, "logs", "app");

        Directory.CreateDirectory(_logDir);
    }

    /// <inheritdoc/>
    public void Log(McpLogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        var filePath = Path.Combine(
            _logDir,
            $"mcp-{DateTime.Now:yyyy-MM-dd}.log"
        );

        var sb = new StringBuilder();
        sb.Append($"[{entry.Timestamp:O}] [{entry.Level}] ");
        sb.Append(entry.Message);

        if (entry.Exception != null)
        {
            sb.AppendLine();
            sb.Append(entry.Exception);
        }

        File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
    }

    /// <inheritdoc/>
    public void Trace(string message) => Log(new() { Level = McpLogLevel.Trace, Message = message });

    /// <inheritdoc/>
    public void Debug(string message) => Log(new() { Level = McpLogLevel.Debug, Message = message });

    /// <inheritdoc/>
    public void Info(string message) => Log(new() { Level = McpLogLevel.Information, Message = message });

    /// <inheritdoc/>
    public void Warn(string message) => Log(new() { Level = McpLogLevel.Warning, Message = message });

    /// <inheritdoc/>
    public void Error(string message, Exception? ex = null) => Log(new() { Level = McpLogLevel.Error, Message = message, Exception = ex });

    /// <inheritdoc/>
    public void Critical(string message, Exception? ex = null) => Log(new() { Level = McpLogLevel.Critical, Message = message, Exception = ex });
}
