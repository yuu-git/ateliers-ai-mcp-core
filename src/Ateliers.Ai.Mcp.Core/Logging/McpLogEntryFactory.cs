using Ateliers.Ai.Mcp.Logging.Context;

namespace Ateliers.Ai.Mcp.Logging;

internal static class McpLogEntryFactory
{
    /// <summary>
    /// 新しい McpLogEntry を作成します。
    /// </summary>
    /// <param name="level"> ログレベル </param>
    /// <param name="message"> ログメッセージ </param>
    /// <param name="exception"> 例外情報 (オプション) </param>
    /// <returns></returns>
    public static McpLogEntry Create(
        McpLogLevel level,
        string message,
        Exception? exception = null)
    {
        var ctx = McpExecutionContext.Current;

        return new McpLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = level,
            Message = message,
            Exception = exception,
            CorrelationId = ctx?.CorrelationId,
            ToolName = ctx?.ToolName
        };
    }
}
