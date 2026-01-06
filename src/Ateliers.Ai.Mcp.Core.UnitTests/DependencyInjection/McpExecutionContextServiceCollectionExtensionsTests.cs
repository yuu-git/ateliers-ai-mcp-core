using Ateliers.Ai.Mcp;
using Ateliers.Ai.Mcp.Context;
using Ateliers.Ai.Mcp.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ateliers.Ai.Mcp.Core.UnitTests.DependencyInjection;

/// <summary>
/// McpExecutionContextServiceCollectionExtensions のテスト
/// </summary>
public class McpExecutionContextServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMcpExecutionContext_ShouldRegisterContext()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMcpExecutionContext();
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetService<IMcpExecutionContext>();

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void AddMcpExecutionContext_ShouldRegisterBothInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMcpExecutionContext();
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var mcpContext = scope.ServiceProvider.GetService<IMcpExecutionContext>();
        var baseContext = scope.ServiceProvider.GetService<IExecutionContext>();

        // Assert
        Assert.NotNull(mcpContext);
        Assert.NotNull(baseContext);
        Assert.Same(mcpContext, baseContext);
    }

    [Fact]
    public void AddMcpExecutionContext_ShouldCreateNewContextWhenNoCurrentContext()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.CorrelationId);
    }

    [Fact]
    public void AddMcpExecutionContext_ShouldBeScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();
        var provider = services.BuildServiceProvider();

        // Act
        IMcpExecutionContext? context1;
        IMcpExecutionContext? context2;

        using (var scope1 = provider.CreateScope())
        {
            context1 = scope1.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        }

        using (var scope2 = provider.CreateScope())
        {
            context2 = scope2.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        }

        // Assert
        Assert.NotEqual(context1.CorrelationId, context2.CorrelationId);
    }

    [Fact]
    public void AddMcpExecutionContext_WithBeginTool_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();

        string? toolName;
        using (var toolScope = context.BeginTool("test-tool"))
        {
            toolName = McpExecutionContext.Current?.ToolName;
        }

        // Assert
        Assert.Equal("test-tool", toolName);
    }
}
