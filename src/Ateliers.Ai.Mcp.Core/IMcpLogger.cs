using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// MCP ロガーを表します。
/// </summary>
public interface IMcpLogger
{
    /// <summary>
    /// ログエントリを記録します。
    /// </summary>
    /// <param name="entry"> ログエントリ </param>
    void Log(McpLogEntry entry);

    /// <summary>
    /// トレース レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message"> メッセージ </param>
    void Trace(string message);

    /// <summary>
    /// デバッグ レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message" > メッセージ </param>
    void Debug(string message);

    /// <summary>
    /// 情報レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message"> メッセージ </param>
    void Info(string message);

    /// <summary>
    /// 警告レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message"> メッセージ </param>
    void Warn(string message);

    /// <summary>
    /// エラーレベルのメッセージを記録します。
    /// </summary>
    /// <param name="message"> メッセージ </param>
    /// <param name="exception"> 関連する例外 (省略可能) </param>
    void Error(string message, Exception? exception = null);

    /// <summary>
    /// 重大レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message"> メッセージ </param>
    /// <param name="exception"> 関連する例外 (省略可能) </param>
    void Critical(string message, Exception? exception = null);
}