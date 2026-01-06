using Ateliers.Ai.Mcp.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Ateliers.Ai.Mcp.DependencyInjection;

/// <summary>
/// IServiceCollection 用の MCP 実行コンテキスト拡張メソッドを提供します。
/// </summary>
public static class McpExecutionContextServiceCollectionExtensions
{
    /// <summary>
    /// MCP 実行コンテキストをサービス コレクションに追加します。
    /// </summary>
    /// <param name="services">サービス コレクション</param>
    /// <returns>更新されたサービス コレクション</returns>
    public static IServiceCollection AddMcpExecutionContext(
        this IServiceCollection services)
    {
        services.AddScoped<IMcpExecutionContext>(provider =>
        {
            var current = McpExecutionContext.Current;
            if (current != null)
            {
                return current;
            }

            // コンテキストがない場合は新しく作成
            var correlationId = Guid.NewGuid().ToString();
            return new McpExecutionContext(correlationId, null);
        });

        // 基底インターフェースも登録
        services.AddScoped<IExecutionContext>(provider => 
            provider.GetRequiredService<IMcpExecutionContext>());

        return services;
    }
}
