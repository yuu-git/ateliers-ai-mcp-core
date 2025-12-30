namespace Ateliers.Ai.Mcp.Logging.Retention;

/// <summary>
/// MCP ログの保持ポリシーを表します。
/// </summary>
public sealed class McpLogRetentionPolicy
{
    /// <summary>
    /// トレース レベルのログの保持期間を取得します。
    /// </summary>
    public TimeSpan TraceRetention { get; init; } = TimeSpan.FromDays(3);

    /// <summary>
    /// デバッグ レベルのログの保持期間を取得します。
    /// </summary>
    public TimeSpan DebugRetention { get; init; } = TimeSpan.FromDays(7);

    /// <summary>
    /// 情報レベルのログの保持期間を取得します。
    /// </summary>
    public TimeSpan InformationRetention { get; init; } = TimeSpan.FromDays(14);

    /// <summary>
    /// 警告レベルのログの保持期間を取得します。
    /// </summary>
    public TimeSpan WarningRetention { get; init; } = TimeSpan.FromDays(30);

    /// <summary>
    /// エラー レベルのログの保持期間を取得します。
    /// </summary>
    public TimeSpan ErrorRetention { get; init; } = TimeSpan.FromDays(90);
}
