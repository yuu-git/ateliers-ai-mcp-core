using Ateliers.Logging;
using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

public class InMemoryMcpLoggerTests
{
    [Fact]
    [Trait("説明", @"最小レベル以上のログエントリを記録すること")]
    public void Log_ShouldAddEntryWhenLevelIsAboveMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = LogLevel.Information,
            LogText = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal("Test message", logger.Entries[0].LogText);
        Assert.Equal(LogLevel.Information, logger.Entries[0].Level);
    }

    [Fact]
    [Trait("説明", @"最小レベル未満のログエントリを記録しないこと")]
    public void Log_ShouldNotAddEntryWhenLevelIsBelowMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = LogLevel.Debug,
            LogText = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        Assert.Empty(logger.Entries);
    }

    [Fact]
    [Trait("説明", @"Info メソッドで情報レベルのログを記録すること")]
    public void Info_ShouldLogInformationLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        logger.Info("Info message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, logger.Entries[0].Level);
        Assert.Equal("Info message", logger.Entries[0].LogText);
    }

    [Fact]
    [Trait("説明", @"Warn メソッドで警告レベルのログを記録すること")]
    public void Warn_ShouldLogWarningLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new InMemoryMcpLogger(options);

        // Act
        logger.Warn("Warning message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, logger.Entries[0].Level);
        Assert.Equal("Warning message", logger.Entries[0].LogText);
    }

    [Fact]
    [Trait("説明", @"Error メソッドでエラーレベルのログと例外を記録すること")]
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
        Assert.Equal(LogLevel.Error, logger.Entries[0].Level);
        Assert.Equal("Error message", logger.Entries[0].LogText);
        Assert.Equal(exception, logger.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"Critical メソッドで重大レベルのログと例外を記録すること")]
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
        Assert.Equal(LogLevel.Critical, logger.Entries[0].Level);
        Assert.Equal("Critical message", logger.Entries[0].LogText);
        Assert.Equal(exception, logger.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"存在しない相関IDで検索した場合、空のセッションを返すこと")]
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
        Assert.Equal("Message 1", logger.Entries[0].LogText);
        Assert.Equal("Message 2", logger.Entries[1].LogText);
        Assert.Equal("Message 3", logger.Entries[2].LogText);
    }

    [Fact]
    [Trait("説明", @"Entries プロパティは読み取り専用であること")]
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
