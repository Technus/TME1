
using Microsoft.EntityFrameworkCore;
using TME1.Core.DataTransferObjects;

namespace TME1.Core;
public class RobotContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<RobotDTO> Robots { get; set; }
}
