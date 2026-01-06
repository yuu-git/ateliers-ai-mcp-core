using Ateliers.Ai.Mcp.Context;
using Xunit;

namespace Ateliers.Ai.Mcp.Core.UnitTests.Context;

/// <summary>
/// McpExecutionContext のテスト
/// </summary>
public class McpExecutionContextTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange & Act
        var context = new McpExecutionContext("test-correlation-id", "test-tool");

        // Assert
        Assert.Equal("test-correlation-id", context.CorrelationId);
        Assert.Equal("test-tool", context.ToolName);
    }

    [Fact]
    public void Constructor_WithNullToolName_ShouldWork()
    {
        // Arrange & Act
        var context = new McpExecutionContext("test-correlation-id", null);

        // Assert
        Assert.Equal("test-correlation-id", context.CorrelationId);
        Assert.Null(context.ToolName);
    }

    [Fact]
    public void Current_ShouldBeNull_WhenNoScopeCreated()
    {
        // Act
        var current = McpExecutionContext.Current;

        // Assert
        Assert.Null(current);
    }

    [Fact]
    public void BeginTool_ShouldSetCurrent()
    {
        // Arrange
        var context = new McpExecutionContext("test-correlation-id", "parent-tool");

        // Act
        using var scope = context.BeginTool("child-tool");
        var current = McpExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.Equal("child-tool", current.ToolName);
        Assert.NotEqual(context.CorrelationId, current.CorrelationId);
    }

    [Fact]
    public void BeginTool_Dispose_ShouldRestorePreviousContext()
    {
        // Arrange
        var context = new McpExecutionContext("test-correlation-id", "parent-tool");

        // Act
        string? scopeCorrelationId;
        string? scopeToolName;
        
        using (var scope = context.BeginTool("child-tool"))
        {
            scopeCorrelationId = McpExecutionContext.Current?.CorrelationId;
            scopeToolName = McpExecutionContext.Current?.ToolName;
        }

        var currentAfterDispose = McpExecutionContext.Current;

        // Assert
        Assert.NotNull(scopeCorrelationId);
        Assert.Equal("child-tool", scopeToolName);
        Assert.Null(currentAfterDispose);
    }

    [Fact]
    public void BeginTool_Nested_ShouldCreateHierarchy()
    {
        // Arrange
        var context = new McpExecutionContext("root-id", "root-tool");

        // Act & Assert
        using (var scope1 = context.BeginTool("tool1"))
        {
            var current1 = McpExecutionContext.Current;
            Assert.NotNull(current1);
            Assert.Equal("tool1", current1.ToolName);

            using (var scope2 = context.BeginTool("tool2"))
            {
                var current2 = McpExecutionContext.Current;
                Assert.NotNull(current2);
                Assert.Equal("tool2", current2.ToolName);
                Assert.NotEqual(current1.CorrelationId, current2.CorrelationId);
            }

            var currentAfterScope2 = McpExecutionContext.Current;
            Assert.NotNull(currentAfterScope2);
            Assert.Equal(current1.CorrelationId, currentAfterScope2.CorrelationId);
            Assert.Equal("tool1", currentAfterScope2.ToolName);
        }
    }

    [Fact]
    public async Task BeginTool_Async_ShouldMaintainContext()
    {
        // Arrange
        var context = new McpExecutionContext("test-id", "parent-tool");

        // Act
        string? correlationId1 = null;
        string? correlationId2 = null;
        string? toolName1 = null;
        string? toolName2 = null;

        using (var scope = context.BeginTool("async-tool"))
        {
            correlationId1 = McpExecutionContext.Current?.CorrelationId;
            toolName1 = McpExecutionContext.Current?.ToolName;
            await Task.Delay(10);
            correlationId2 = McpExecutionContext.Current?.CorrelationId;
            toolName2 = McpExecutionContext.Current?.ToolName;
        }

        // Assert
        Assert.NotNull(correlationId1);
        Assert.Equal(correlationId1, correlationId2);
        Assert.Equal("async-tool", toolName1);
        Assert.Equal(toolName1, toolName2);
    }

    [Fact]
    public void BeginScope_ShouldAlsoWork()
    {
        // Arrange
        var context = new McpExecutionContext("test-id", "test-tool");

        // Act
        using var scope = context.BeginScope("ScopeProperties");
        var current = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.Equal("ScopeProperties", current.Properties);
    }

    [Fact]
    public void StaticCurrent_ShouldReturnMcpContext()
    {
        // Arrange
        var context = new McpExecutionContext("test-id", "test-tool");

        // Act
        using var scope = context.BeginTool("nested-tool");
        var current = McpExecutionContext.Current;
        var baseCurrent = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.NotNull(baseCurrent);
        Assert.IsType<McpExecutionContext>(current);
        Assert.Same(current, baseCurrent);
    }
}
