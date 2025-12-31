using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

public class InMemoryMcpLoggerTests
{
    [Fact]
    [Trait("説明", @"最小レベル以上のログエントリが記録されること")]
    public void Log_ShouldAddEntryWhenLevelIsAboveMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal("Test message", logger.Entries[0].Message);
        Assert.Equal(McpLogLevel.Information, logger.Entries[0].Level);
    }

    [Fact]
    [Trait("説明", @"最小レベル未満のログエントリが記録されないこと")]
    public void Log_ShouldNotAddEntryWhenLevelIsBelowMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Debug,
            Message = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        Assert.Empty(logger.Entries);
    }

    [Fact]
    [Trait("説明", @"Info メソッドで情報レベルのログが記録されること")]
    public void Info_ShouldLogInformationLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        logger.Info("Info message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(McpLogLevel.Information, logger.Entries[0].Level);
        Assert.Equal("Info message", logger.Entries[0].Message);
    }

    [Fact]
    [Trait("説明", @"Warn メソッドで警告レベルのログが記録されること")]
    public void Warn_ShouldLogWarningLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        logger.Warn("Warning message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(McpLogLevel.Warning, logger.Entries[0].Level);
        Assert.Equal("Warning message", logger.Entries[0].Message);
    }

    [Fact]
    [Trait("説明", @"Error メソッドでエラーレベルのログと例外が記録されること")]
    public void Error_ShouldLogErrorLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.Error("Error message", exception);

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(McpLogLevel.Error, logger.Entries[0].Level);
        Assert.Equal("Error message", logger.Entries[0].Message);
        Assert.Equal(exception, logger.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"Critical メソッドで重大レベルのログと例外が記録されること")]
    public void Critical_ShouldLogCriticalLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);
        var exception = new Exception("Critical exception");

        // Act
        logger.Critical("Critical message", exception);

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(McpLogLevel.Critical, logger.Entries[0].Level);
        Assert.Equal("Critical message", logger.Entries[0].Message);
        Assert.Equal(exception, logger.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"存在しない相関IDで検索した場合、空のセッションが返されること")]
    public void ReadByCorrelationId_ShouldReturnEmptySessionWhenCorrelationIdNotFound()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        var session = logger.ReadByCorrelationId("non-existent-id");

        // Assert
        Assert.Equal("non-existent-id", session.CorrelationId);
        Assert.Empty(session.Entries);
    }

    [Fact]
    [Trait("説明", @"複数のログ呼び出しでエントリが蓄積されること")]
    public void MultipleLogCalls_ShouldAccumulateEntries()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        logger.Info("Message 1");
        logger.Warn("Message 2");
        logger.Error("Message 3");

        // Assert
        Assert.Equal(3, logger.Entries.Count);
        Assert.Equal("Message 1", logger.Entries[0].Message);
        Assert.Equal("Message 2", logger.Entries[1].Message);
        Assert.Equal("Message 3", logger.Entries[2].Message);
    }

    [Fact]
    [Trait("説明", @"Entries プロパティが読み取り専用であること")]
    public void Entries_ShouldBeReadOnly()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        logger.Info("Test message");

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<McpLogEntry>>(logger.Entries);
    }
}
