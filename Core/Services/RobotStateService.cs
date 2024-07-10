using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Enumerations;
using TME1.Abstractions.Repositories;
using TME1.Abstractions.Services;
using TME1.Core.DataTransferObjects;

namespace TME1.Core.Services;
public class RobotStateService(
  IRobotRepository<int, RobotDTO> repository,
  ILogger<RobotStateService> logger) : IRobotStateService<int>
{
  private static readonly Error _invalidResult = Error.New("Checking curent robot state result was invalid");
  private readonly IRobotRepository<int, RobotDTO> _repository = repository;
  private readonly ILogger<RobotStateService> _logger = logger;

  private readonly int _definedRobotStatusCount = Enum.GetValues<RobotStatus>().Length;
  private readonly string[] _robotStatusMessageExamples = ["Ok","Drinking Tea","Charging","Teapot"];

  public async Task<Fin<IRobotStateUpdate<int>>> UpdateAsync(int robotId, CancellationToken cancellationToken = default)
  {
    try
    {
      var result = await _repository.GetAsync(robotId, cancellationToken);
      switch(result.Case)
      {
        case RobotDTO robot:
          var update = new RobotStateUpdateDTO(robotId)
          {
            ChargeLevel = robot.ChargeLevel >= 0 ? float.Clamp(robot.ChargeLevel + 0.01f, 0, 1) : robot.ChargeLevel,
            Status = (RobotStatus)Random.Shared.Next(_definedRobotStatusCount),
            StatusMessage = _robotStatusMessageExamples[Random.Shared.Next(_robotStatusMessageExamples.Length)],
          };
          return update;
        case Error error:
          _logger.LogUpdateError(error);
          return error;
        default:
          _logger.LogUpdateError(_invalidResult);
          return _invalidResult;
      }
    }
    catch (Exception ex)
    {
      var error = Error.New(ex);
      _logger.LogUpdateError(error);
      return error;
    }
  }
}

public static partial class RobotStateServiceLogExtensions
{
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Failed to update: `{error}`")]
  public static partial void LogUpdateError(this ILogger logger, Error error);
}

