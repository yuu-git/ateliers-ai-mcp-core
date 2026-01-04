namespace Ateliers.Ai.Mcp.Context.UnitTests.Context;

public class McpExecutionContextScopeTests
{
    [Fact]
    [Trait("説明", @"McpExecutionContextScope が正しくコンテキストを設定および復元すること")]
    public void McpExecutionContextScope_ShouldSetAndRestoreContext()
    {
        // Arrange
        var initialContext = McpExecutionContext.Current;
        // Act
        using (var scope = new McpExecutionContextScope("TestTool"))
        {
            var scopedContext = McpExecutionContext.Current;
            // Assert within scope
            Assert.NotNull(scopedContext);
            Assert.Equal("TestTool", scopedContext.ToolName);
            Assert.NotEqual(initialContext?.CorrelationId, scopedContext.CorrelationId);
        }
        // Assert after scope
        var restoredContext = McpExecutionContext.Current;
        Assert.Equal(initialContext, restoredContext);
    }
}
