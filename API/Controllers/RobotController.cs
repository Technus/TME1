using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using TME1.Abstractions.Repositories;
using TME1.Core.DataTransferObjects;

namespace API.Controllers;

[ApiController]
[Route("robot/[controller]")]
public class RobotController : RobotControllerBase<int, RobotDTO>
{
  public RobotController(ILogger<RobotController> logger, IRobotRepository<int, RobotDTO> repository) : base(logger, repository)
  {
  }

  protected override void LogGetAllAsyncError(Fin<RobotDTO> result) => Logger.LogGetAllAsyncError(result);
  protected override void LogStateUpdateError(Fin<RobotDTO> result) => Logger.LogStateUpdateError(result);
}

public static partial class RobotControllerLogExtensions
{
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not load robot/s: `{result}`")]
  public static partial void LogGetAllAsyncError(this ILogger logger, Fin<RobotDTO> result);

  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not update state: `{result}`")]
  public static partial void LogStateUpdateError(this ILogger logger, Fin<RobotDTO> result);
}
