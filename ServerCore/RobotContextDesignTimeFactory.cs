using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TME1.ServerCore;
/// <summary>
/// For Migration scaffolding
/// </summary>
public class RobotContextDesignTimeFactory : IDesignTimeDbContextFactory<RobotContext>
{
  public RobotContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder().UseSqlServer(options
    => options.MigrationsAssembly(typeof(RobotContext).Assembly.GetName().Name)).Options);
}
