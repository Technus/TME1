using Microsoft.EntityFrameworkCore;
using TME1.ServerCore;

namespace TME1.Tests.ServerCore;
/// <summary>
/// Database Context for Tests
/// </summary>
public class MockRobotContext : RobotContext
{
  private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EFTestSample;Trusted_Connection=True;ConnectRetryCount=0";

  public MockRobotContext() : base(new DbContextOptionsBuilder().UseSqlServer(ConnectionString, options
    => options.MigrationsAssembly(typeof(RobotContext).Assembly.GetName().Name)).Options)
  {
    Database.EnsureDeleted();
    Database.EnsureCreated();
  }
}
