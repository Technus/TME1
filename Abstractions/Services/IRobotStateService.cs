using LanguageExt;
using TME1.Abstractions.DataTransferObjects;

namespace TME1.Abstractions.Services;
/// <summary>
/// Used to provide new state for robot
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IRobotStateService<TKey>
{
    /// <summary>
    /// Fetches new robot state from source
    /// </summary>
    /// <param name="robot"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>the update to process</returns>
    Task<Fin<IRobotStateUpdate<TKey>>> UpdateAsync(IRobot<TKey> robot, CancellationToken cancellationToken = default);
}
