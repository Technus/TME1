using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Enumerations;
using TME1.ServerCore.DataTransferObjects;
using TME1.ServerCore.Services;

namespace TME1.Core.Services;

/// <summary>
/// Robot status updating service using generated data
/// </summary>
/// <param name="logger"></param>
public class RobotStateService(ILogger<RobotStateService> logger) : IRobotStateService<int>
{
  private static readonly Error _missingRobot = Error.New("Robot is undefined");
  private readonly ILogger<RobotStateService> _logger = logger;

  private readonly int _definedRobotStatusCount = Enum.GetValues<RobotStatus>().Length;
  private readonly string[] _robotStatusMessageExamples = ["Ok","Drinking Tea","Charging","Teapot"];

  /// <summary>
  /// Dummy update generator, incrments <see cref="IRobot{TKey}.ChargeLevel"/> and fiddles with <see cref="IRobot{TKey}.Status"/> and <see cref="IRobot{TKey}.StatusMessage"/>
  /// </summary>
  /// <param name="robot"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public Task<Fin<IRobotStateUpdate<int>>> UpdateAsync(IRobot<int> robot, CancellationToken cancellationToken = default)
  {
    if(robot is null)
    {
      _logger.LogUpdateError(_missingRobot);
      return Task.FromResult<Fin<IRobotStateUpdate<int>>>(_missingRobot);
    }

    var update = new RobotStateUpdateDto(robot.Id)
    {
      ChargeLevel = robot.ChargeLevel >= 0 ? float.Clamp(robot.ChargeLevel + 0.01f, 0, 1) : robot.ChargeLevel,
      Status = (RobotStatus)Random.Shared.Next(_definedRobotStatusCount),
      StatusMessage = _robotStatusMessageExamples[Random.Shared.Next(_robotStatusMessageExamples.Length)],
    };
    return Task.FromResult<Fin<IRobotStateUpdate<int>>>(update);
  }
}

public static partial class RobotStateServiceLogExtensions
{
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Failed to update: `{error}`")]
  public static partial void LogUpdateError(this ILogger logger, Error error);
}

