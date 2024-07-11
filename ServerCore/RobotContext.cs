
using Microsoft.EntityFrameworkCore;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.ServerCore;
/// <summary>
/// The main <see cref="DbContext"/>
/// </summary>
/// <param name="options"></param>
public class RobotContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<RobotDto> Robots { get; set; }
}
