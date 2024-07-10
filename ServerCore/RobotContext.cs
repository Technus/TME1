
using Microsoft.EntityFrameworkCore;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.ServerCore;
public class RobotContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<RobotDto> Robots { get; set; }
}
