using Ateliers.Ai.Mcp.Logging;
using Ateliers.Logging;
using Xunit;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

/// <summary>
/// McpLogEntry のテスト
/// </summary>
public class McpLogEntryTests
{
    [Fact]
    public void McpLogEntry_ShouldInheritFromLogEntry()
    {
        // Arrange & Act
        var entry = new McpLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = LogLevel.Information,
            LogText = "Test message",
            Message = "Test message",
            CorrelationId = "test-id",
            Category = "MCP",
            ToolName = "test.tool"
        };

        // Assert
        Assert.IsAssignableFrom<LogEntry>(entry);
        Assert.Equal("Test message", entry.Message);
        Assert.Equal("test-id", entry.CorrelationId);
        Assert.Equal("MCP", entry.Category);
        Assert.Equal("test.tool", entry.ToolName);
    }

    [Fact]
    public void McpLogEntry_WithNullToolName_ShouldWork()
    {
        // Arrange & Act
        var entry = new McpLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = LogLevel.Information,
            LogText = "Test message",
            Message = "Test message",
            CorrelationId = "test-id",
            Category = "MCP",
            ToolName = null
        };

        // Assert
        Assert.Null(entry.ToolName);
    }

    [Fact]
    public void McpLogEntry_ShouldSupportAllLogLevels()
    {
        // Arrange
        var levels = new[]
        {
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Information,
            LogLevel.Warning,
            LogLevel.Error,
            LogLevel.Critical
        };

        // Act & Assert
        foreach (var level in levels)
        {
            var entry = new McpLogEntry
            {
                Timestamp = DateTimeOffset.UtcNow,
                Level = level,
                LogText = $"Test {level}",
                ToolName = "test.tool"
            };

            Assert.Equal(level, entry.Level);
        }
    }

    [Fact]
    public void McpLogEntry_WithException_ShouldStoreException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        var entry = new McpLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = LogLevel.Error,
            LogText = "Error occurred",
            Message = "Error occurred",
            Exception = exception,
            ToolName = "test.tool"
        };

        // Assert
        Assert.Equal(exception, entry.Exception);
    }

    [Fact]
    public void McpLogEntry_WithProperties_ShouldStoreProperties()
    {
        // Arrange
        var properties = new Dictionary<string, object>
        {
            ["key1"] = "value1",
            ["key2"] = 123
        };

        // Act
        var entry = new McpLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = LogLevel.Information,
            LogText = "Test message",
            Properties = properties,
            ToolName = "test.tool"
        };

        // Assert
        Assert.NotNull(entry.Properties);
        Assert.Equal(2, entry.Properties.Count);
        Assert.Equal("value1", entry.Properties["key1"]);
        Assert.Equal(123, entry.Properties["key2"]);
    }
}
