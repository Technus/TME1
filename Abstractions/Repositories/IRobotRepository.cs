using LanguageExt;
using TME1.Abstractions.DataTransferObjects;

namespace TME1.Abstractions.Repositories;
/// <summary>
/// For abstracting robot DB data context manipulation
/// </summary>
/// <typeparam name="TRobot">the robot data type</typeparam>
/// <remarks>
/// To allow DB replacement<br/>
/// Could be replaced by CQRS to further refine it into smaller pieces, but this is relatively simple CRUD app
/// </remarks>
public interface IRobotRepository<TKey, TRobot> where TRobot : IRobot<TKey>
{
  /// <summary>
  /// Gets the data of all robots
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>The robots asynchronousely</returns>
  IAsyncEnumerable<Fin<TRobot>> GetAllAsync(CancellationToken cancellationToken = default);
  /// <summary>
  /// Removes all robots
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>if any operation was performed</returns>
  Task<Fin<bool>> DeleteAllAsync(CancellationToken cancellationToken = default);
  /// <summary>
  /// Gets the data of all robots
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>The robot</returns>
  Task<Fin<TRobot>> GetAsync(TKey key, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes the robot
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>if any operation was performed</returns>
  Task<Fin<bool>> DeleteAsync(TKey key, CancellationToken cancellationToken = default);
  /// <summary>
  /// Inserts or Updates the robot as a whole
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>The robot as inserted/updated</returns>
  Task<Fin<TRobot>> AddOrUpdateAsync(TRobot robot, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates the robot status
  /// </summary>
  /// <param name="stateUpdate">new state update to process</param>
  /// <param name="cancellationToken"></param>
  /// <returns>modified robot</returns>
  /// <remarks>return may happen before or after DB was written</remarks>
  Task<Fin<TRobot>> StateUpdateAsync(IRobotStateUpdate<TKey> stateUpdate, CancellationToken cancellationToken = default);
}
