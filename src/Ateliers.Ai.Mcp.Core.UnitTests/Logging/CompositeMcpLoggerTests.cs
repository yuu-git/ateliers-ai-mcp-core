using Ateliers.Ai.Mcp.Logging;
using Ateliers.Logging;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

public class CompositeMcpLoggerTests
{
    [Fact]
    [Trait("説明", @"複数のロガーにログを転送すること")]
    public void Log_ShouldForwardToAllLoggers()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger1 = new InMemoryMcpLogger(options);
        var logger2 = new InMemoryMcpLogger(options);
        var compositeLogger = new CompositeMcpLogger(new[] { logger1, logger2 });
        var entry = new McpLogEntry
        {
            Level = LogLevel.Information,
            LogText = "Test message"
        };

        // Act
        compositeLogger.Log(entry);

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal("Test message", logger1.Entries[0].LogText);
        Assert.Equal("Test message", logger2.Entries[0].LogText);
    }

    [Fact]
    [Trait("説明", @"Info メソッドで全てのロガーに情報レベルのログを記録すること")]
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
        Assert.Equal(LogLevel.Information, logger1.Entries[0].Level);
        Assert.Equal(LogLevel.Information, logger2.Entries[0].Level);
        Assert.Equal("Info message", logger1.Entries[0].LogText);
        Assert.Equal("Info message", logger2.Entries[0].LogText);
    }

    [Fact]
    [Trait("説明", @"Warn メソッドで全てのロガーに警告レベルのログを記録すること")]
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
        Assert.Equal(LogLevel.Warning, logger1.Entries[0].Level);
        Assert.Equal(LogLevel.Warning, logger2.Entries[0].Level);
    }

    [Fact]
    [Trait("説明", @"Error メソッドで全てのロガーにエラーレベルのログを記録すること")]
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
        Assert.Equal(LogLevel.Error, logger1.Entries[0].Level);
        Assert.Equal(exception, logger1.Entries[0].Exception);
        Assert.Equal(exception, logger2.Entries[0].Exception);
    }

    [Fact]
    [Trait("説明", @"Critical メソッドで全てのロガーに重大レベルのログを記録すること")]
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
        Assert.Equal(LogLevel.Critical, logger1.Entries[0].Level);
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
        Assert.Equal("Message 1", logger1.Entries[0].LogText);
        Assert.Equal("Message 2", logger1.Entries[1].LogText);
        Assert.Equal("Message 3", logger1.Entries[2].LogText);
    }

    [Fact]
    [Trait("説明", @"ロガーコレクションがnullの場合、例外をスローすること")]
    public void Constructor_ShouldThrowWhenLoggersIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CompositeMcpLogger(null!));
    }

    [Fact]
    [Trait("説明", @"空のロガーコレクションでも例外をスローしないこと")]
    public void Constructor_ShouldNotThrowWhenLoggersIsEmpty()
    {
        // Arrange & Act
        var compositeLogger = new CompositeMcpLogger(Array.Empty<IMcpLogger>());

        // Assert
        Assert.NotNull(compositeLogger);
    }

    [Fact]
    [Trait("説明", @"空のロガーコレクションでログ呼び出しても例外をスローしないこと")]
    public void Log_ShouldNotThrowWhenNoLoggers()
    {
        // Arrange
        var compositeLogger = new CompositeMcpLogger(Array.Empty<IMcpLogger>());
        var entry = new McpLogEntry
        {
            Level = LogLevel.Information,
            LogText = "Test message"
        };

        // Act & Assert (例外をスローしないことを確認)
        compositeLogger.Log(entry);
    }

    [Fact]
    [Trait("説明", @"一つのロガーが例外をスローしても他のロガーにログを記録すること")]
    public void Log_ShouldContinueWhenOneLoggerThrows()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var validLogger = new InMemoryMcpLogger(options);
        var throwingLogger = new ThrowingMcpLogger();
        var compositeLogger = new CompositeMcpLogger(new IMcpLogger[] { throwingLogger, validLogger });
        var entry = new McpLogEntry
        {
            Level = LogLevel.Information,
            LogText = "Test message"
        };

        // Act
        compositeLogger.Log(entry);

        // Assert
        Assert.Single(validLogger.Entries);
        Assert.Equal("Test message", validLogger.Entries[0].LogText);
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
        Assert.Equal("Mixed loggers test", memoryLogger.Entries[0].LogText);
    }

    // テスト用のロガー: 常に例外をスロー
    private class ThrowingMcpLogger : IMcpLogger
    {
        public void Log(McpLogEntry entry) => throw new InvalidOperationException("Test exception");
        public void Log(LogEntry entry) => throw new InvalidOperationException("Test exception");
        public void Trace(string message) => throw new InvalidOperationException("Test exception");
        public void Debug(string message) => throw new InvalidOperationException("Test exception");
        public void Info(string message) => throw new InvalidOperationException("Test exception");
        public void Warn(string message) => throw new InvalidOperationException("Test exception");
        public void Error(string message, Exception? exception = null) => throw new InvalidOperationException("Test exception");
        public void Critical(string message, Exception? exception = null) => throw new InvalidOperationException("Test exception");
    }
}
