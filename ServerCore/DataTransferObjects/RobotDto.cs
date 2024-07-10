using System.ComponentModel.DataAnnotations;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Enumerations;

namespace TME1.ServerCore.DataTransferObjects;
public class RobotDto(string name, string model) : IRobot<int>
{
  [Key]
  public int Id { get; set; }

  public string Name { get; set; } = name;

  public string Model { get; set; } = model;

  public float ChargeLevel { get; set; } = -1;

  public string? Location { get; set; }

  public RobotPosition Position { get; set; }

  public string? PositionMessage { get; set; }

  public RobotStatus Status { get; set; }

  public string? StatusMessage { get; set; }
}
