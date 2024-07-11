using Microsoft.EntityFrameworkCore;
using TME1.ServerCore;

namespace TME1.ServerTests.ServerCore;
/// <summary>
/// Database Context for Tests
/// </summary>
public class MockRobotContext : RobotContext
{
  private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EFTestSample;Trusted_Connection=True;ConnectRetryCount=0";

  /// <summary>
  /// Ensures that each test runs on empty database when there is no paralellism
  /// </summary>
  /// <remarks>alternalively the connection string would have to point to different databases every time and that would require proper cleanup</remarks>
  public MockRobotContext() : base(new DbContextOptionsBuilder().UseSqlServer(ConnectionString, options
    => options.MigrationsAssembly(typeof(RobotContext).Assembly.GetName().Name)).Options)
  {
    Database.EnsureDeleted();
    Database.EnsureCreated();
  }
}
