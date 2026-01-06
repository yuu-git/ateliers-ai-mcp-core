using Ateliers.Ai.Mcp.Logging;
using Ateliers.Ai.Mcp.Logging.DependencyInjection;
using Ateliers.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ateliers.Ai.Mcp.Core.UnitTests.DependencyInjection;

/// <summary>
/// McpLoggingServiceCollectionExtensions のテスト
/// </summary>
public class McpLoggingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMcpLogging_ShouldRegisterLogger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMcpLogging();
        var provider = services.BuildServiceProvider();
        var logger = provider.GetService<IMcpLogger>();

        // Assert
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddMcpLogging_WithConfiguration_ShouldApplyConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging
                .SetMinimumLevel(LogLevel.Debug)
                .AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IMcpLogger>();

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(memoryLogger);
        logger.Debug("Test message");
        Assert.Single(memoryLogger.Entries);
        Assert.Equal("MCP", memoryLogger.Entries[0].Category);
    }

    [Fact]
    public void AddMcpLogging_WithoutConfiguration_ShouldUseDefaultConsoleLogger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMcpLogging();
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IMcpLogger>();

        // Assert
        Assert.NotNull(logger);
        var exception = Record.Exception(() => logger.Info("Test message"));
        Assert.Null(exception);
    }

    [Fact]
    public void AddMcpLogging_WithMultipleLoggers_ShouldCreateCompositeLogger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging
                .AddConsole()
                .AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IMcpLogger>();

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(memoryLogger);
        logger.Info("Test message");
        Assert.Single(memoryLogger.Entries);
    }

    [Fact]
    public void AddMcpLogging_DefaultCategory_ShouldBeMCP()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        InMemoryMcpLogger memoryLogger = null!;
        services.AddMcpLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IMcpLogger>();

        // Assert
        logger.Info("Test message");
        Assert.Equal("MCP", memoryLogger.Entries[0].Category);
    }
}
