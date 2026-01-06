using System.Globalization;
using System.Text;
using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Logging;

/// <summary>
/// ファイルログを管理する MCP ロガー
/// </summary>
public sealed class FileMcpLogger : FileLogger, IMcpLogger, IMcpLogReader
{
    private readonly string _logDir;
    private readonly McpLoggerOptions _options;

    /// <summary>
    /// ファイル MCP ロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">ロガーのオプション</param>
    public FileMcpLogger(McpLoggerOptions options)
        : base(options, "mcp")
    {
        _options = options;
        _logDir = options.LogDirectory ?? Path.Combine(AppContext.BaseDirectory, "logs", "app");
    }

    /// <summary>
    /// MCP ログエントリを記録します。
    /// </summary>
    /// <param name="entry">MCP ログエントリ</param>
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
        
        if (!string.IsNullOrEmpty(entry.Category))
        {
            sb.Append($"[{entry.Category}] ");
        }

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
    public override void Log(LogEntry entry)
    {
        if (entry is McpLogEntry mcpEntry)
        {
            Log(mcpEntry);
        }
        else
        {
            // 現在の MCP コンテキストから ToolName を取得
            var currentContext = Ai.Mcp.Context.McpExecutionContext.Current;
            
            Log(new McpLogEntry
            {
                Timestamp = entry.Timestamp,
                Level = entry.Level,
                LogText = entry.LogText,
                Message = entry.Message,
                Exception = entry.Exception,
                CorrelationId = entry.CorrelationId,
                Category = entry.Category,
                ToolName = currentContext?.ToolName,
                Properties = entry.Properties
            });
        }
    }

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

    /// <summary>
    /// 指定された相関IDに基づいてログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadByCorrelationId(string correlationId)
        => ReadByCorrelationId(correlationId);

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
    /// 最後のログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadLastSession()
        => ReadLastSession();

    /// <inheritdoc/>
    public McpLogSession ReadByCategory(string category)
    {
        if (!Directory.Exists(_logDir))
        {
            return new McpLogSession
            {
                CorrelationId = string.Empty,
                Entries = Array.Empty<McpLogEntry>()
            };
        }

        var entries = new List<McpLogEntry>();

        foreach (var file in Directory.EnumerateFiles(_logDir, "*.log"))
        {
            foreach (var line in File.ReadLines(file))
            {
                if (!line.Contains($"[{category}]", StringComparison.OrdinalIgnoreCase))
                    continue;

                var entry = TryParse(line);
                if (entry != null && entry.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true)
                {
                    entries.Add(entry);
                }
            }
        }

        return new McpLogSession
        {
            CorrelationId = string.Empty,
            Entries = entries
                .OrderBy(e => e.Timestamp)
                .ToList()
        };
    }

    /// <summary>
    /// 指定されたカテゴリに基づいてログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadByCategory(string category)
        => ReadByCategory(category);

    /// <inheritdoc/>
    public McpLogSession ReadByCorrelationIdAndCategory(string correlationId, string category)
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

                if (!line.Contains($"[{category}]", StringComparison.OrdinalIgnoreCase))
                    continue;

                var entry = TryParse(line);
                if (entry != null && entry.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true)
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

    /// <summary>
    /// 指定された相関IDとカテゴリに基づいてログセッションを読み取ります（基底インターフェース実装）。
    /// </summary>
    LogSession ILogReader.ReadByCorrelationIdAndCategory(string correlationId, string category)
        => ReadByCorrelationIdAndCategory(correlationId, category);

    private static McpLogEntry? TryParse(string line)
    {
        try
        {
            if (!line.StartsWith('['))
            {
                return Fallback(line);
            }

            var timestampEnd = line.IndexOf(']');
            if (timestampEnd <= 0)
            {
                return Fallback(line);
            }

            var timestampText = line[1..timestampEnd];
            if (!DateTimeOffset.TryParse(
                    timestampText,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var timestamp))
            {
                return Fallback(line);
            }

            var level = ExtractLevel(line);

            var categoryIndex = line.IndexOf('[', timestampEnd + 1);
            var category = string.Empty;
            if (categoryIndex >= 0)
            {
                var categoryEnd = line.IndexOf(']', categoryIndex);
                if (categoryEnd > categoryIndex)
                {
                    var categoryText = line[(categoryIndex + 1)..categoryEnd];
                    if (!categoryText.StartsWith("CID:") && !categoryText.StartsWith("Tool:"))
                    {
                        category = categoryText;
                    }
                }
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
                Category = category,
                Timestamp = timestamp,
                Level = level,
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
            Level = LogLevel.Unknown,
            LogText = line
        };

    private static LogLevel ExtractLevel(string line)
    {
        if (line.Contains("[Trace]")) return LogLevel.Trace;
        if (line.Contains("[Debug]")) return LogLevel.Debug;
        if (line.Contains("[Information]")) return LogLevel.Information;
        if (line.Contains("[Warning]")) return LogLevel.Warning;
        if (line.Contains("[Error]")) return LogLevel.Error;
        if (line.Contains("[Critical]")) return LogLevel.Critical;

        return LogLevel.Unknown;
    }
}
