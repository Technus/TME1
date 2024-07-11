namespace TME1.Abstractions.Enumerations;
/// <summary>
/// Represents robot position from the safety standpoint
/// </summary>
/// <remarks>Used to ensure consistency between layers</remarks>
public enum RobotPosition
{
  /// <summary>
  /// Default, unknown, none
  /// </summary>
  None,
  /// <summary>
  /// Performing safe operations, collaborating with operator
  /// </summary>
  Safe,
  /// <summary>
  /// Performing unsafe operations, operating alone, operators excluded from range  
  /// </summary>
  Unsafe,
}
