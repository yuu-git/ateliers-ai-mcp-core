using Ateliers.Ai.Mcp.Logging;
using System.Text;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

public class ConsoleMcpLoggerTests : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalOutput;

    public ConsoleMcpLoggerTests()
    {
        _consoleOutput = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_consoleOutput);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _consoleOutput.Dispose();
    }

    private string GetConsoleOutput()
    {
        return _consoleOutput.ToString();
    }

    [Fact]
    [Trait("説明", @"最小レベル以上のログエントリがコンソールに出力されること")]
    public void Log_ShouldWriteToConsoleWhenLevelIsAboveMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Test message", output);
        Assert.Contains("[Information]", output);
    }

    [Fact]
    [Trait("説明", @"最小レベル未満のログエントリがコンソールに出力されないこと")]
    public void Log_ShouldNotWriteToConsoleWhenLevelIsBelowMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Debug,
            Message = "Debug message"
        };

        // Act
        logger.Log(entry);

        // Assert
        var output = GetConsoleOutput();
        Assert.DoesNotContain("Debug message", output);
    }

    [Fact]
    [Trait("説明", @"Info メソッドで情報レベルのログがコンソールに出力されること")]
    public void Info_ShouldLogInformationLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);

        // Act
        logger.Info("Info message");

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Info message", output);
        Assert.Contains("[Information]", output);
    }

    [Fact]
    [Trait("説明", @"Warn メソッドで警告レベルのログがコンソールに出力されること")]
    public void Warn_ShouldLogWarningLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);

        // Act
        logger.Warn("Warning message");

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Warning message", output);
        Assert.Contains("[Warning]", output);
    }

    [Fact]
    [Trait("説明", @"Error メソッドでエラーレベルのログと例外がコンソールに出力されること")]
    public void Error_ShouldLogErrorLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.Error("Error message", exception);

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Error message", output);
        Assert.Contains("[Error]", output);
        Assert.Contains("Test exception", output);
    }

    [Fact]
    [Trait("説明", @"Critical メソッドで重大レベルのログと例外がコンソールに出力されること")]
    public void Critical_ShouldLogCriticalLevel()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);
        var exception = new Exception("Critical exception");

        // Act
        logger.Critical("Critical message", exception);

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Critical message", output);
        Assert.Contains("[Critical]", output);
        Assert.Contains("Critical exception", output);
    }

    [Fact]
    [Trait("説明", @"複数のログ呼び出しでコンソール出力が蓄積されること")]
    public void MultipleLogCalls_ShouldAccumulateConsoleOutput()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);

        // Act
        logger.Info("Message 1");
        logger.Warn("Message 2");
        logger.Error("Message 3");

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Message 1", output);
        Assert.Contains("Message 2", output);
        Assert.Contains("Message 3", output);
    }

    [Fact]
    [Trait("説明", @"タイムスタンプがHH:mm:ssフォーマットで出力されること")]
    public void Log_ShouldFormatTimestampCorrectly()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);
        var timestamp = new DateTimeOffset(2025, 1, 15, 14, 30, 45, TimeSpan.Zero);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message",
            Timestamp = timestamp
        };

        // Act
        logger.Log(entry);

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[14:30:45]", output);
    }

    [Fact]
    [Trait("説明", @"例外がない場合、例外情報が出力されないこと")]
    public void Log_ShouldNotWriteExceptionWhenNull()
    {
        // Arrange
        var options = new McpLoggerOptions();
        var logger = new ConsoleMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Error,
            Message = "Error without exception",
            Exception = null
        };

        // Act
        logger.Log(entry);

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Error without exception", output);
        var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
    }
}
