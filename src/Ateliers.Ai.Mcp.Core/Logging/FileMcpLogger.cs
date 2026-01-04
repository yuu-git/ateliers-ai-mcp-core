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
        
        if (!string.IsNullOrEmpty(entry.CorrelationId))
        {
            sb.Append($"[CID:{entry.CorrelationId}] ");
        }

        if (!string.IsNullOrEmpty(entry.ToolName))
        {
            sb.Append($"[Tool:{entry.ToolName}] ");
        }

        sb.Append(entry.LogText);

        if (entry.Exception != null)
        {
            sb.AppendLine();
            sb.Append(entry.Exception);
        }

        File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
    }

    /// <inheritdoc/>
    public void Trace(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Trace, message));

    /// <inheritdoc/>
    public void Debug(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Debug, message));

    /// <inheritdoc/>
    public void Info(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Information, message));

    /// <inheritdoc/>
    public void Warn(string message) => Log(McpLogEntryFactory.Create(McpLogLevel.Warning, message));

    /// <inheritdoc/>
    public void Error(string message, Exception? ex = null) => Log(McpLogEntryFactory.Create(McpLogLevel.Error, message, ex));

    /// <inheritdoc/>
    public void Critical(string message, Exception? ex = null) => Log(McpLogEntryFactory.Create(McpLogLevel.Critical, message, ex));

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

    /// <inheritdoc/>
    public McpLogSession ReadLastSession()
    {
        if (!Directory.Exists(_logDir))
        {
            return new McpLogSession
            {
                CorrelationId = string.Empty,
                Entries = Array.Empty<McpLogEntry>()
            };
        }
        var latestEntry = (McpLogEntry?)null;
        foreach (var file in Directory.EnumerateFiles(_logDir, "*.log"))
        {
            foreach (var line in File.ReadLines(file))
            {
                var entry = TryParse(line);
                if (entry == null)
                    continue;
                if (latestEntry == null || entry.Timestamp > latestEntry.Timestamp)
                {
                    latestEntry = entry;
                }
            }
        }
        if (latestEntry == null || string.IsNullOrEmpty(latestEntry.CorrelationId))
        {
            return new McpLogSession
            {
                CorrelationId = string.Empty,
                Entries = Array.Empty<McpLogEntry>()
            };
        }

        return ReadByCorrelationId(latestEntry.CorrelationId);
    }

    /// <summary>
    /// 超ゆるいパーサ。
    /// フォーマットが変わっても壊れないことを最優先。
    /// </summary>
    private static McpLogEntry? TryParse(string line)
    {
        // 例想定:
        // [2026-01-04T19:47:36.4512537+00:00] [INFO] message...
        // フォーマットが違っても message として拾う

        try
        {
            // タイムスタンプが角括弧で囲まれている形式に対応
            if (!line.StartsWith('['))
            {
                return Fallback(line);
            }

            var timestampEnd = line.IndexOf(']');
            if (timestampEnd <= 0)
            {
                return Fallback(line);
            }

            // 角括弧を除いたタイムスタンプ部分を取得
            var timestampText = line[1..timestampEnd];
            if (!DateTimeOffset.TryParse(
                    timestampText,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var timestamp))
            {
                return Fallback(line);
            }

            var correlationIdIndex = line.IndexOf("[CID:", StringComparison.OrdinalIgnoreCase);
            var correlationId = string.Empty;
            if (correlationIdIndex >= 0)
            {
                var cidEnd = line.IndexOf(']', correlationIdIndex);
                if (cidEnd > correlationIdIndex)
                {
                    correlationId = line[(correlationIdIndex + 5)..cidEnd];
                }
            }

            var toolNameIndex = line.IndexOf("[Tool:", StringComparison.OrdinalIgnoreCase);
            var toolName = string.Empty;
            if (toolNameIndex >= 0)
            {
                var toolEnd = line.IndexOf(']', toolNameIndex);
                if (toolEnd > toolNameIndex)
                {
                    toolName = line[(toolNameIndex + 6)..toolEnd];
                }
            }

            var messageStart = line.LastIndexOf("] ");
            var message = string.Empty;
            if (messageStart >= 0 && messageStart + 2 < line.Length)
            {
                message = line[(messageStart + 2)..];
            }

            return new McpLogEntry
            {
                CorrelationId = correlationId,
                ToolName = toolName,
                Timestamp = timestamp,
                Level = ExtractLevel(line),
                LogText = line,
                Message = message
            };
        }
        catch
        {
            return Fallback(line);
        }
    }

    private static McpLogEntry Fallback(string line)
        => new()
        {
            Timestamp = DateTimeOffset.MinValue,
            Level = McpLogLevel.UNKNOWN,
            LogText = line
        };

    private static McpLogLevel ExtractLevel(string line)
    {
        if (line.Contains("[Trace]")) return McpLogLevel.Trace;
        if (line.Contains("[Debug]")) return McpLogLevel.Debug;

        if (line.Contains("[Information]")) return McpLogLevel.Information;
        if (line.Contains("[Warning]")) return McpLogLevel.Warning;
        if (line.Contains("[Error]")) return McpLogLevel.Error;
        if (line.Contains("[Critical]")) return McpLogLevel.Critical;

        return McpLogLevel.UNKNOWN;
    }
}
