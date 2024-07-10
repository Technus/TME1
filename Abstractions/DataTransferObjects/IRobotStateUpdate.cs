using TME1.Abstractions.Enumerations;

namespace TME1.Abstractions.DataTransferObjects;
/// <summary>
/// Abstraction over message with robot status update
/// </summary>
/// <remarks>Used to ensure consistency between layers</remarks>
public interface IRobotStateUpdate<out TKey> : IIDentifiable<TKey>
{
  /// <summary>
  /// <see cref="IRobot{TKey}.Status"/> to set
  /// </summary>
  /// <remarks>always updates</remarks>
  RobotStatus Status { get; }
  /// <summary>
  /// <see cref="IRobot{TKey}.StatusMessage"/> to set
  /// </summary>
  /// <remarks>null will keep the same value</remarks>
  string? StatusMessage { get; }
  /// <summary>
  /// <see cref="IRobot{TKey}.ChargeLevel"/> to set
  /// </summary>
  /// <remarks>always updates</remarks>
  float ChargeLevel { get; }
}
