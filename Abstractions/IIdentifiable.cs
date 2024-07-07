namespace TME1.Abstractions;
/// <summary>
/// Common logic for database objects
/// </summary>
/// <typeparam name="TId">Identifier type</typeparam>
public interface IIDentifiable<out TId>
{
  /// <summary>
  /// Unique identifier in a collection or table
  /// </summary>
  TId Id { get; }
}
