using Microsoft.EntityFrameworkCore.Design;

namespace TME1.ServerTests.ServerCore;
/// <summary>
/// Design Time (Test Time) Database Context factory
/// </summary>
public class MockRobotContextDesignTimeFactory : IDesignTimeDbContextFactory<MockRobotContext>
{
  public MockRobotContext CreateDbContext(string[] args) => new();
}
