using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using TME1.Abstractions;
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
  private readonly IRobotRepository<int, RobotDTO> _repository = repository;
  private readonly ILogger<RobotStateService> _logger = logger;

  private readonly int _definedRobotStatusCount = Enum.GetValues<RobotStatus>().Length;
  private readonly string[] _robotStatusMessageExamples = ["Ok","Drinking Tea","Charging","Teapot"];

  public async Task<Fin<IRobotStateUpdate<int>>> UpdateAsync(int robot, CancellationToken cancellationToken = default)
  {
    try
    {
      var result = await _repository.GetAsync(robot, cancellationToken);
      if (result.TryGetValue(out var value))
      {
        var update = new RobotStateUpdateDTO(robot)
        {
          ChargeLevel = value.ChargeLevel >= 0 ? float.Clamp(value.ChargeLevel + 0.01f,0,1) : value.ChargeLevel,
          Status = (RobotStatus)Random.Shared.Next(_definedRobotStatusCount),
          StatusMessage = _robotStatusMessageExamples[Random.Shared.Next(_robotStatusMessageExamples.Length)],
        };
        return update;
      }
      return result.Case as Error;
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

