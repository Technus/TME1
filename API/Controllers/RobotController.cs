using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TME1.Abstractions.Repositories;
using TME1.Core.DataTransferObjects;

namespace API.Controllers;


[ApiController]
[Route("robots")]
public class RobotController(
  ILogger<RobotController> logger, 
  IRobotRepository<int, RobotDTO> repository) : ControllerBase
{
  private static readonly Error _resultWasInvalid = Error.New("Result was invalid");
  private readonly ILogger<RobotController> _logger = logger;
  private readonly IRobotRepository<int, RobotDTO> _repository = repository;

  /// <summary>
  /// Endpoint to get all Robots
  /// </summary>
  /// <param name="cancellationToken">cancelation for enumeration</param>
  /// <returns></returns>
  [HttpGet]
  public async IAsyncEnumerable<RobotDTO> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (!ModelState.IsValid)
      yield break;

    await foreach (var result in _repository.GetAllAsync(cancellationToken))
    {
      switch(result.Case)
      {
        case RobotDTO dto: yield return dto; continue;
        case Error error: _logger.LogGetAllAsyncError(error); yield break;
        default: _logger.LogStateUpdateError(_resultWasInvalid); yield break;
      }
    }
  }

  /// <summary>
  /// Endpoint to get a Robot by its id
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  [Route("{id}")]
  public async Task<ActionResult<RobotDTO>> GetAsync([FromRoute] int id)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var result = await _repository.GetAsync(id);
    switch(result.Case)
    {
      case RobotDTO dto: return Ok(dto);
      case Error error: _logger.LogStateUpdateError(error); return NotFound();
      default: _logger.LogStateUpdateError(_resultWasInvalid); return BadRequest();
    }
  }

  /// <summary>
  /// Endpoint to update a Robot State
  /// </summary>
  /// <param name="robotStateUpdate"></param>
  /// <returns></returns>
  [HttpPatch]
  [Route("state")]
  public async Task<ActionResult<RobotDTO>> StateUpdateAsync([FromBody] RobotStateUpdateDTO robotStateUpdate)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var result = await _repository.StateUpdateAsync(robotStateUpdate);
    switch(result.Case)
    {
      case RobotDTO dto: return Ok(dto);
      case Error error: _logger.LogStateUpdateError(error); return NotFound();
      default: _logger.LogStateUpdateError(_resultWasInvalid); return BadRequest();
    }
  }
}

public static partial class RobotControllerLogExtensions
{
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not load robot/s: `{error}`")]
  public static partial void LogGetAllAsyncError(this ILogger logger, Error error);

  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not update state: `{error}`")]
  public static partial void LogStateUpdateError(this ILogger logger, Error error);
}