using System.Globalization;
using System.Text;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// ファイルログを管理する MCP ロガー
/// </summary>
public sealed class FileMcpLogger : IMcpLogger, IMcpLogReader
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

    /// <inheritdoc/>
    public McpLogSession ReadByCorrelationId(string correlationId)
    {
        if (!Directory.Exists(_logDir))
        {
            return Empty(correlationId);
        }

        var entries = new List<McpLogEntry>();

        foreach (var file in Directory.EnumerateFiles(_logDir, "*.log"))
        {
            foreach (var line in File.ReadLines(file))
            {
                if (!line.Contains(correlationId, StringComparison.OrdinalIgnoreCase))
                    continue;

                var entry = TryParse(line);
                if (entry != null)
                {
                    entries.Add(entry);
                }
            }
        }

        return new McpLogSession
        {
            CorrelationId = correlationId,
            Entries = entries
                .OrderBy(e => e.Timestamp)
                .ToList()
        };
    }

    private static McpLogSession Empty(string correlationId)
        => new()
        {
            CorrelationId = correlationId,
            Entries = Array.Empty<McpLogEntry>()
        };

    /// <summary>
    /// 超ゆるいパーサ。
    /// フォーマットが変わっても壊れないことを最優先。
    /// </summary>
    private static McpLogEntry? TryParse(string line)
    {
        // 例想定:
        // 2025-01-01T12:34:56.789Z [INFO] message...
        // フォーマットが違っても message として拾う

        try
        {
            var timestampEnd = line.IndexOf(' ');
            if (timestampEnd <= 0)
            {
                return Fallback(line);
            }

            var timestampText = line[..timestampEnd];
            if (!DateTimeOffset.TryParse(
                    timestampText,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var timestamp))
            {
                return Fallback(line);
            }

            return new McpLogEntry
            {
                Timestamp = timestamp,
                Level = ExtractLevel(line),
                Message = line
            };
        }
        catch
        {
            return null;
        }
    }

    private static McpLogEntry Fallback(string line)
        => new()
        {
            Timestamp = DateTimeOffset.MinValue,
            Level = McpLogLevel.UNKNOWN,
            Message = line
        };

    private static McpLogLevel ExtractLevel(string line)
    {
        if (line.Contains("[TRACE]")) return McpLogLevel.Trace;
        if (line.Contains("[DEBUG]")) return McpLogLevel.Debug;

        if (line.Contains("[INFO]")) return McpLogLevel.Information;
        if (line.Contains("[WARN]")) return McpLogLevel.Warning;
        if (line.Contains("[ERROR]")) return McpLogLevel.Error;
        if (line.Contains("[FATAL]")) return McpLogLevel.Critical;

        return McpLogLevel.UNKNOWN;
    }
}
