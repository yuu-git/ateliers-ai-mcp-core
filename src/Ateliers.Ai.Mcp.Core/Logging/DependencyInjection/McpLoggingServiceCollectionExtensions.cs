using Microsoft.Extensions.DependencyInjection;

namespace Ateliers.Ai.Mcp.Logging.DependencyInjection;

/// <summary>
/// IServiceCollection 用の MCP ロギング拡張メソッドを提供します。
/// </summary>
public static class McpLoggingServiceCollectionExtensions
{
    /// <summary>
    /// MCP ロギングをサービス コレクションに追加します。
    /// </summary>
    /// <param name="services"> サービス コレクション </param>
    /// <param name="configure"> ロギング ビルダーの構成アクション </param>
    /// <returns> 更新されたサービス コレクション </returns>
    public static IServiceCollection AddMcpLogging(
        this IServiceCollection services,
        Action<McpLoggingBuilder>? configure = null)
    {
        var builder = new McpLoggingBuilder();

        configure?.Invoke(builder);

        if (builder.Loggers.Count == 0)
        {
            builder.AddConsole();
        }

        IMcpLogger logger = builder.Loggers.Count == 1
            ? builder.Loggers[0]
            : new CompositeMcpLogger(builder.Loggers);

        services.AddSingleton(logger);

        return services;
    }
}
