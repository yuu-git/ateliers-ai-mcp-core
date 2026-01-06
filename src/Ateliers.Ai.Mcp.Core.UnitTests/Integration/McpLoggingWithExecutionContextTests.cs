using Ateliers.Ai.Mcp;
using Ateliers.Ai.Mcp.Context;
using Ateliers.Ai.Mcp.DependencyInjection;
using Ateliers.Ai.Mcp.Logging;
using Ateliers.Ai.Mcp.Logging.DependencyInjection;
using Ateliers.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Integration;

/// <summary>
/// MCP ロギングと実行コンテキストの統合テスト
/// </summary>
public class McpLoggingWithExecutionContextTests
{
    [Fact]
    public void LoggingWithContext_ShouldIncludeCorrelationIdAndToolName()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();

        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging
                .SetMinimumLevel(LogLevel.Debug)
                .AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<IMcpLogger>();

        using (var toolScope = context.BeginTool("test.tool"))
        {
            logger.Info("Test message");
        }

        // Assert
        Assert.NotNull(memoryLogger);
        Assert.Single(memoryLogger.Entries);
        Assert.NotNull(memoryLogger.Entries[0].CorrelationId);
        Assert.Equal("test.tool", memoryLogger.Entries[0].ToolName);
        Assert.Equal("MCP", memoryLogger.Entries[0].Category);
        Assert.Equal("Test message", memoryLogger.Entries[0].Message);
    }

    [Fact]
    public void NestedTools_ShouldHaveDifferentCorrelationIds()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();

        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<IMcpLogger>();

        using (var tool1 = context.BeginTool("outer.tool"))
        {
            logger.Info("Outer message");

            using (var tool2 = context.BeginTool("inner.tool"))
            {
                logger.Info("Inner message");
            }

            logger.Info("Outer message 2");
        }

        // Assert
        Assert.NotNull(memoryLogger);
        Assert.Equal(3, memoryLogger.Entries.Count);

        var outerCorrelationId = memoryLogger.Entries[0].CorrelationId;
        var innerCorrelationId = memoryLogger.Entries[1].CorrelationId;

        Assert.NotEqual(outerCorrelationId, innerCorrelationId);
        Assert.Equal(outerCorrelationId, memoryLogger.Entries[2].CorrelationId);

        Assert.Equal("outer.tool", memoryLogger.Entries[0].ToolName);
        Assert.Equal("inner.tool", memoryLogger.Entries[1].ToolName);
        Assert.Equal("outer.tool", memoryLogger.Entries[2].ToolName);
    }

    [Fact]
    public async Task AsyncToolOperations_ShouldMaintainCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();

        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<IMcpLogger>();

        using (var toolScope = context.BeginTool("async.tool"))
        {
            logger.Info("Before delay");
            await Task.Delay(10);
            logger.Info("After delay");
        }

        // Assert
        Assert.NotNull(memoryLogger);
        Assert.Equal(2, memoryLogger.Entries.Count);
        Assert.Equal(
            memoryLogger.Entries[0].CorrelationId,
            memoryLogger.Entries[1].CorrelationId);
        Assert.Equal("async.tool", memoryLogger.Entries[0].ToolName);
        Assert.Equal("async.tool", memoryLogger.Entries[1].ToolName);
    }

    [Fact]
    public void McpLoggingPolicy_ShouldBeFollowed()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();

        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<IMcpLogger>();

        using (var toolScope = context.BeginTool("test.tool"))
        {
            logger.Info("MCP.Start");
            logger.Info("Processing...");
            logger.Info("MCP.Success");
        }

        // Assert
        Assert.Equal(3, memoryLogger.Entries.Count);
        Assert.Equal("MCP.Start", memoryLogger.Entries[0].Message);
        Assert.Equal("MCP.Success", memoryLogger.Entries[2].Message);
    }

    [Fact]
    public void LogReader_ShouldReadByCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();

        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<IMcpLogger>();

        string? correlationId;
        using (var toolScope = context.BeginTool("test.tool"))
        {
            logger.Info("Test message");
            correlationId = McpExecutionContext.Current?.CorrelationId;
        }

        var session = memoryLogger.ReadByCorrelationId(correlationId!);

        // Assert
        Assert.NotNull(session);
        Assert.Equal(correlationId, session.CorrelationId);
        Assert.Single(session.Entries);
        Assert.Equal("Test message", session.Entries[0].Message);
    }

    [Fact]
    public void LogReader_ShouldReadByCategory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMcpExecutionContext();

        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcpExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<IMcpLogger>();

        using (var toolScope = context.BeginTool("test.tool"))
        {
            logger.Info("MCP message");
        }

        var session = memoryLogger.ReadByCategory("MCP");

        // Assert
        Assert.NotNull(session);
        Assert.Single(session.Entries);
        Assert.Equal("MCP", session.Entries[0].Category);
    }
}
