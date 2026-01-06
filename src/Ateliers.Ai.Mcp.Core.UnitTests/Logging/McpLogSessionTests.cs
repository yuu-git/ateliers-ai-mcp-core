using Ateliers.Ai.Mcp.Logging;
using Ateliers.Logging;
using Xunit;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Logging;

/// <summary>
/// McpLogSession のテスト
/// </summary>
public class McpLogSessionTests
{
    [Fact]
    public void McpLogSession_ShouldInheritFromLogSession()
    {
        // Arrange & Act
        var session = new McpLogSession
        {
            CorrelationId = "test-id",
            Entries = new List<McpLogEntry>
            {
                new McpLogEntry
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    Level = LogLevel.Information,
                    LogText = "Test message",
                    ToolName = "test.tool"
                }
            }
        };

        // Assert
        Assert.IsAssignableFrom<LogSession>(session);
        Assert.Equal("test-id", session.CorrelationId);
        Assert.Single(session.Entries);
    }

    [Fact]
    public void McpLogSession_EmptyEntries_ShouldWork()
    {
        // Arrange & Act
        var session = new McpLogSession
        {
            CorrelationId = "test-id",
            Entries = Array.Empty<McpLogEntry>()
        };

        // Assert
        Assert.Empty(session.Entries);
    }

    [Fact]
    public void McpLogSession_MultipleEntries_ShouldPreserveOrder()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var entries = new List<McpLogEntry>
        {
            new McpLogEntry
            {
                Timestamp = now,
                Level = LogLevel.Information,
                LogText = "First",
                ToolName = "test.tool"
            },
            new McpLogEntry
            {
                Timestamp = now.AddSeconds(1),
                Level = LogLevel.Information,
                LogText = "Second",
                ToolName = "test.tool"
            },
            new McpLogEntry
            {
                Timestamp = now.AddSeconds(2),
                Level = LogLevel.Information,
                LogText = "Third",
                ToolName = "test.tool"
            }
        };

        // Act
        var session = new McpLogSession
        {
            CorrelationId = "test-id",
            Entries = entries
        };

        // Assert
        Assert.Equal(3, session.Entries.Count);
        Assert.Equal("First", session.Entries[0].LogText);
        Assert.Equal("Second", session.Entries[1].LogText);
        Assert.Equal("Third", session.Entries[2].LogText);
    }

    [Fact]
    public void McpLogSession_Entries_ShouldBeReadOnly()
    {
        // Arrange
        var entries = new List<McpLogEntry>
        {
            new McpLogEntry
            {
                Timestamp = DateTimeOffset.UtcNow,
                Level = LogLevel.Information,
                LogText = "Test",
                ToolName = "test.tool"
            }
        };

        // Act
        var session = new McpLogSession
        {
            CorrelationId = "test-id",
            Entries = entries
        };

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<McpLogEntry>>(session.Entries);
    }

    [Fact]
    public void McpLogSession_WithDifferentToolNames_ShouldWork()
    {
        // Arrange & Act
        var session = new McpLogSession
        {
            CorrelationId = "test-id",
            Entries = new List<McpLogEntry>
            {
                new McpLogEntry
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    Level = LogLevel.Information,
                    LogText = "Tool1 message",
                    ToolName = "tool1"
                },
                new McpLogEntry
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    Level = LogLevel.Information,
                    LogText = "Tool2 message",
                    ToolName = "tool2"
                }
            }
        };

        // Assert
        Assert.Equal(2, session.Entries.Count);
        Assert.Equal("tool1", session.Entries[0].ToolName);
        Assert.Equal("tool2", session.Entries[1].ToolName);
    }
}
