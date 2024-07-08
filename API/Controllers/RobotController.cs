using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using TME1.Abstractions;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Repositories;
using TME1.Core.DataTransferObjects;

namespace API.Controllers;


[ApiController]
[Route("[controller]")]
public class RobotController(
  ILogger<RobotController> logger, 
  IRobotRepository<int, RobotDTO> repository) : ControllerBase
{
  private readonly ILogger<RobotController> _logger = logger;
  private readonly IRobotRepository<int, RobotDTO> _repository = repository;

  /// <summary>
  /// Endpoint to get all entries from database
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  [Route("GetAll")]
  public async IAsyncEnumerable<RobotDTO> GetAllAsync()
  {
    if (!ModelState.IsValid)
      yield break;

    await foreach (var result in _repository.GetAllAsync())
    {
      if (result.TryGetValue(out var dto))
      {
        yield return dto;
        continue;
      }

      _logger.LogGetAllAsyncError(result.Case as Error);
      yield break;
    }
  }

  /// <summary>
  /// Endpoint to get an entry from database
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  [Route("Get/{key}")]
  public async Task<ActionResult<RobotDTO>> GetAsync([FromRoute] int key)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var result = await _repository.GetAsync(key);
    if (result.TryGetValue(out var dto))
      return Ok(dto);

    _logger.LogStateUpdateError(result.Case as Error);
    return NotFound();
  }

  /// <summary>
  /// Endpoint to update an entry in database
  /// </summary>
  /// <param name="robotStateUpdate"></param>
  /// <returns></returns>
  [HttpPost]
  [Route("StateUpdate")]
  public async Task<ActionResult<RobotDTO>> StateUpdateAsync([FromBody] RobotStateUpdateDTO robotStateUpdate)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var result = await _repository.StateUpdateAsync(robotStateUpdate);
    if(result.TryGetValue(out var dto))
      return Ok(dto);

    _logger.LogStateUpdateError(result.Case as Error);
    return NotFound();
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