using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

public class CompositeMcpLoggerTests
{
    [Fact]
    [Trait("説明", @"複数のロガーにログが転送されること")]
    public void Log_ShouldForwardToAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act
        compositeLogger.Log(entry);

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal("Test message", logger1.Entries[0].Message);
        Assert.Equal("Test message", logger2.Entries[0].Message);
    }

    [Fact]
    [Trait("説明", @"Info メソッドで全てのロガーに情報レベルのログが記録されること")]
    public void Info_ShouldLogToAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });

        // Act
        compositeLogger.Info("Info message");

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal(McpLogLevel.Information, logger1.Entries[0].Level);
        Assert.Equal(McpLogLevel.Information, logger2.Entries[0].Level);
        Assert.Equal("Info message", logger1.Entries[0].Message);
        Assert.Equal("Info message", logger2.Entries[0].Message);
    }

    [Fact]
    [Trait("説明", @"Warn メソッドで全てのロガーに警告レベルのログが記録されること")]
    public void Warn_ShouldLogToAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });

        // Act
        compositeLogger.Warn("Warning message");

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal(McpLogLevel.Warning, logger1.Entries[0].Level);
        Assert.Equal(McpLogLevel.Warning, logger2.Entries[0].Level);
    }

    [Fact]
    [Trait("説明", @"Error メソッドで全てのロガーにエラーレベルのログが記録されること")]
    public void Error_ShouldLogToAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });
        var exception = new InvalidOperationException("Test exception");

        // Act
        compositeLogger.Error("Error message", exception);

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal(McpLogLevel.Error, logger1.Entries[0].Level);
        Assert.Equal(exception, logger1.Entries[0].Exception);
        Assert.Equal(exception, logger2.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"Critical メソッドで全てのロガーに重大レベルのログが記録されること")]
    public void Critical_ShouldLogToAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });
        var exception = new Exception("Critical exception");

        // Act
        compositeLogger.Critical("Critical message", exception);

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal(McpLogLevel.Critical, logger1.Entries[0].Level);
        Assert.Equal(exception, logger1.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"複数のログ呼び出しで全てのロガーにエントリが蓄積されること")]
    public void MultipleLogCalls_ShouldAccumulateEntriesInAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });

        // Act
        compositeLogger.Info("Message 1");
        compositeLogger.Warn("Message 2");
        compositeLogger.Error("Message 3");

        // Assert
        Assert.Equal(3, logger1.Entries.Count);
        Assert.Equal(3, logger2.Entries.Count);
        Assert.Equal("Message 1", logger1.Entries[0].Message);
        Assert.Equal("Message 2", logger1.Entries[1].Message);
        Assert.Equal("Message 3", logger1.Entries[2].Message);
    }

    [Fact]
    [Trait("説明", @"ロガーコレクションがnullの場合、例外がスローされること")]
    public void Constructor_ShouldThrowWhenLoggersIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CompositeMcpLogger(null!));
    }

    [Fact]
    [Trait("説明", @"空のロガーコレクションでも例外がスローされないこと")]
    public void Constructor_ShouldNotThrowWhenLoggersIsEmpty()
    {
        // Arrange & Act
        var compositeLogger = new CompositeMcpLogger(Array.Empty<IMcpLogger>());

        // Assert
        Assert.NotNull(compositeLogger);
    }

    [Fact]
    [Trait("説明", @"空のロガーコレクションでログが呼び出されても例外がスローされないこと")]
    public void Log_ShouldNotThrowWhenNoLoggers()
    {
        // Arrange
        var compositeLogger = new CompositeMcpLogger(Array.Empty<IMcpLogger>());
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act & Assert (例外がスローされないことを確認)
        compositeLogger.Log(entry);
    }

    [Fact]
    [Trait("説明", @"一つのロガーが例外をスローしても他のロガーにログが記録されること")]
    public void Log_ShouldContinueWhenOneLoggerThrows()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var validLogger = new InMemoryMcpLogger(options);
        var throwingLogger = new ThrowingMcpLogger();
        var compositeLogger = new CompositeMcpLogger(new IMcpLogger[] { throwingLogger, validLogger });
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act
        compositeLogger.Log(entry);

        // Assert
        Assert.Single(validLogger.Entries);
        Assert.Equal("Test message", validLogger.Entries[0].Message);
    }

    [Fact]
    [Trait("説明", @"異なる種類のロガーを組み合わせることができること")]
    public void Log_ShouldWorkWithDifferentLoggerTypes()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var memoryLogger = new InMemoryMcpLogger(options);
        var consoleLogger = new ConsoleMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new IMcpLogger[] { memoryLogger, consoleLogger });

        // Act
        compositeLogger.Info("Mixed loggers test");

        // Assert
        Assert.Single(memoryLogger.Entries);
        Assert.Equal("Mixed loggers test", memoryLogger.Entries[0].Message);
    }

    // テスト用のロガー: 常に例外をスロー
    private class ThrowingMcpLogger : IMcpLogger
    {
        public void Log(McpLogEntry entry) => throw new InvalidOperationException("Test exception");
        public void Trace(string message) => throw new InvalidOperationException("Test exception");
        public void Debug(string message) => throw new InvalidOperationException("Test exception");
        public void Info(string message) => throw new InvalidOperationException("Test exception");
        public void Warn(string message) => throw new InvalidOperationException("Test exception");
        public void Error(string message, Exception? exception = null) => throw new InvalidOperationException("Test exception");
        public void Critical(string message, Exception? exception = null) => throw new InvalidOperationException("Test exception");
    }
}
