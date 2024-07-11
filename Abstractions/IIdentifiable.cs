namespace TME1.Abstractions;
/// <summary>
/// Common logic for data objects to identify them by key
/// </summary>
/// <typeparam name="TId">Identifier type</typeparam>
public interface IIdentifiable<out TId>
{
  /// <summary>
  /// Unique identifier in a collection or table
  /// </summary>
  TId Id { get; }
}
