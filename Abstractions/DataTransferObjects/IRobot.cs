using TME1.Abstractions.Enumerations;

namespace TME1.Abstractions.DataTransferObjects;
/// <summary>
/// Abstraction over robot data
/// </summary>
/// <remarks>Used to ensure consistency between layers</remarks>
public interface IRobot<out TKey> : IIDentifiable<TKey>
{
  /// <summary>
  /// User friendly name
  /// </summary>
  string Name { get; }
  /// <summary>
  /// Robot Model/type
  /// </summary>
  string Model { get; }
  /// <summary>
  /// Robot charge level in range from 0 to 1
  /// </summary>
  /// <remarks>Use negative number for unknown values</remarks>
  float ChargeLevel { get; }
  /// <summary>
  /// Current site it is located at
  /// </summary>
  string? Location { get; }
  /// <summary>
  /// Curent position
  /// </summary>
  RobotPosition Position { get; }
  /// <summary>
  /// Additional details for current position
  /// </summary>
  string? PositionMessage { get; }
  /// <summary>
  /// Current status
  /// </summary>
  RobotStatus Status { get; }
  /// <summary>
  /// Additional details for current status
  /// </summary>
  string? StatusMessage { get; }
}
