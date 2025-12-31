using Ateliers.Ai.Mcp.Logging;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

public class FileMcpLoggerTests : IDisposable
{
    private readonly string _testLogDirectory;

    public FileMcpLoggerTests()
    {
        _testLogDirectory = Path.Combine(Path.GetTempPath(), $"mcp-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testLogDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testLogDirectory))
        {
            Directory.Delete(_testLogDirectory, recursive: true);
        }
    }

    [Fact]
    [Trait("説明", @"ログファイルが作成されること")]
    public void Log_ShouldCreateLogFile()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        Assert.Single(logFiles);
    }

    [Fact]
    [Trait("説明", @"最小レベル以上のログエントリが記録されること")]
    public void Log_ShouldAddEntryWhenLevelIsAboveMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Information,
            Message = "Test message"
        };

        // Act
        logger.Log(entry);

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var content = File.ReadAllText(logFiles[0]);
        Assert.Contains("Test message", content);
        Assert.Contains("[Information]", content);
    }

    [Fact]
    [Trait("説明", @"最小レベル未満のログエントリが記録されないこと")]
    public void Log_ShouldNotAddEntryWhenLevelIsBelowMinimum()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);
        var entry = new McpLogEntry
        {
            Level = McpLogLevel.Debug,
            Message = "Debug message"
        };

        // Act
        logger.Log(entry);

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        if (logFiles.Length > 0)
        {
            var content = File.ReadAllText(logFiles[0]);
            Assert.DoesNotContain("Debug message", content);
        }
    }

    [Fact]
    [Trait("説明", @"Info メソッドで情報レベルのログが記録されること")]
    public void Info_ShouldLogInformationLevel()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);

        // Act
        logger.Info("Info message");

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var content = File.ReadAllText(logFiles[0]);
        Assert.Contains("Info message", content);
        Assert.Contains("[Information]", content);
    }

    [Fact]
    [Trait("説明", @"Warn メソッドで警告レベルのログが記録されること")]
    public void Warn_ShouldLogWarningLevel()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);

        // Act
        logger.Warn("Warning message");

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var content = File.ReadAllText(logFiles[0]);
        Assert.Contains("Warning message", content);
        Assert.Contains("[Warning]", content);
    }

    [Fact]
    [Trait("説明", @"Error メソッドでエラーレベルのログと例外が記録されること")]
    public void Error_ShouldLogErrorLevel()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.Error("Error message", exception);

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var content = File.ReadAllText(logFiles[0]);
        Assert.Contains("Error message", content);
        Assert.Contains("[Error]", content);
        Assert.Contains("Test exception", content);
    }

    [Fact]
    [Trait("説明", @"Critical メソッドで重大レベルのログと例外が記録されること")]
    public void Critical_ShouldLogCriticalLevel()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);
        var exception = new Exception("Critical exception");

        // Act
        logger.Critical("Critical message", exception);

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var content = File.ReadAllText(logFiles[0]);
        Assert.Contains("Critical message", content);
        Assert.Contains("[Critical]", content);
        Assert.Contains("Critical exception", content);
    }

    [Fact]
    [Trait("説明", @"複数のログ呼び出しでエントリが蓄積されること")]
    public void MultipleLogCalls_ShouldAccumulateEntries()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);

        // Act
        logger.Info("Message 1");
        logger.Warn("Message 2");
        logger.Error("Message 3");

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var content = File.ReadAllText(logFiles[0]);
        Assert.Contains("Message 1", content);
        Assert.Contains("Message 2", content);
        Assert.Contains("Message 3", content);
    }

    [Fact]
    [Trait("説明", @"ログファイル名が日付フォーマットであること")]
    public void LogFileName_ShouldContainDate()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);

        // Act
        logger.Info("Test message");

        // Assert
        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log");
        var fileName = Path.GetFileName(logFiles[0]);
        var expectedPattern = $"mcp-{DateTime.Now:yyyy-MM-dd}.log";
        Assert.Equal(expectedPattern, fileName);
    }

    [Fact]
    [Trait("説明", @"存在しない相関IDで検索した場合、空のセッションが返されること")]
    public void ReadByCorrelationId_ShouldReturnEmptySessionWhenCorrelationIdNotFound()
    {
        // Arrange
        var options = new McpLoggerOptions { LogDirectory = _testLogDirectory };
        var logger = new FileMcpLogger(options);

        // Act
        var session = logger.ReadByCorrelationId("non-existent-id");

        // Assert
        Assert.Equal("non-existent-id", session.CorrelationId);
        Assert.Empty(session.Entries);
    }

    [Fact]
    [Trait("説明", @"存在しないログディレクトリで検索した場合、空のセッションが返されること")]
    public void ReadByCorrelationId_ShouldReturnEmptySessionWhenDirectoryNotExists()
    {
        // Arrange
        var nonExistentDir = Path.Combine(Path.GetTempPath(), $"non-existent-{Guid.NewGuid()}");
        var options = new McpLoggerOptions { LogDirectory = nonExistentDir };
        var logger = new FileMcpLogger(options);

        // Act
        var session = logger.ReadByCorrelationId("test-id");

        // Assert
        Assert.Equal("test-id", session.CorrelationId);
        Assert.Empty(session.Entries);
    }

    [Fact]
    [Trait("説明", @"ログディレクトリが指定されていない場合、デフォルトディレクトリが使用されること")]
    public void Constructor_ShouldUseDefaultDirectoryWhenNotSpecified()
    {
        // Arrange & Act
        var options = new McpLoggerOptions();
        var logger = new FileMcpLogger(options);
        logger.Info("Test message");

        // Assert
        var defaultLogDir = Path.Combine(AppContext.BaseDirectory, "logs", "app");
        Assert.True(Directory.Exists(defaultLogDir));

        // Cleanup
        if (Directory.Exists(defaultLogDir))
        {
            Directory.Delete(defaultLogDir, recursive: true);
        }
    }
}
