using LanguageExt;
using TME1.ClientCore.Models;

namespace TME1.ClientApp;
/// <summary>
/// Http client wrapper interface for Robot context
/// </summary>
public interface IRobotHttpClient
{
  /// <summary>
  /// Get all robots
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  IAsyncEnumerable<Fin<RobotModel>> GetAllAsync(CancellationToken cancellationToken = default);
  /// <summary>
  /// Get single robot based on id
  /// </summary>
  /// <param name="id"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<Fin<RobotModel>> GetAsync(int id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Get updated robot based on id
  /// </summary>
  /// <param name="id"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<Fin<RobotModel>> StateUpdateAsync(int id, CancellationToken cancellationToken = default);
}