using System.ComponentModel.DataAnnotations;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Enumerations;

namespace TME1.ServerCore.DataTransferObjects;
public class RobotStateUpdateDto(int id) : IRobotStateUpdate<int>
{
  [Key]
  public int Id { get; set; } = id;

  public RobotStatus Status { get; set; }

  public string? StatusMessage { get; set; }

  public float ChargeLevel { get; set; }
}
