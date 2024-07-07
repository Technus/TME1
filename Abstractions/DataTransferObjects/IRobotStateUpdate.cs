using TME1.Abstractions.Enumerations;

namespace TME1.Abstractions.DataTransferObjects;
/// <summary>
/// Message with robot status update
/// </summary>
public interface IRobotStateUpdate<out TKey> : IIDentifiable<TKey>
{
  /// <summary>
  /// Status to set
  /// </summary>
  /// <remarks>strictly always updates the status</remarks>
  RobotStatus Status { get; }
  /// <summary>
  /// Status message to set
  /// </summary>
  /// <remarks>null will keep the same message</remarks>
  string? StatusMessage { get; }
  /// <summary>
  /// Battery level to set, 
  /// </summary>
  /// <remarks>See <see cref="IRobot{TKey}.ChargeLevel"/></remarks>
  float ChargeLevel { get; }
}
