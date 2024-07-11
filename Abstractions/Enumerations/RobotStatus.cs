namespace TME1.Abstractions.Enumerations;
/// <summary>
/// Represents robot status from the task/operability standpoint
/// </summary>
/// <remarks>Used to ensure consistency between layers</remarks>
public enum RobotStatus
{
  /// <summary>
  /// Default, unknown, none
  /// </summary>
  None,
  /// <summary>
  /// for ex. Robot Busy, processes some user task
  /// </summary>
  Normal,
  /// <summary>
  /// for ex. Robot Unavailable, currently cannot accept user tasks, charging
  /// </summary>
  Warning,
  /// <summary>
  /// For ex. Robot Malfunctioning, needs assistance or maintenance
  /// </summary>
  Error,
  /// <summary>
  /// for ex. Robot Available, awaiting orders
  /// </summary>
  Idle,
}
