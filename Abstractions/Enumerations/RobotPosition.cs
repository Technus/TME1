namespace TME1.Abstractions.Enumerations;
/// <summary>
/// Descri
/// </summary>
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
