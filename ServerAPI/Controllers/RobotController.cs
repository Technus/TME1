using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TME1.Abstractions.DataTransferObjects;
using TME1.ServerCore.DataTransferObjects;
using TME1.ServerCore.Repositories;
using TME1.ServerCore.Services;

namespace API.Controllers;
/// <summary>
/// Endpoints for <see cref="TME1.ServerCore.RobotContext"/>
/// </summary>
/// <param name="logger"></param>
/// <param name="repository">Robot repository</param>
/// <param name="stateUpdateService">robot state upadte service</param>
[ApiController]
[Route("api/robots")]
public class RobotController(
  ILogger<RobotController> logger, 
  IRobotRepository<int, RobotDto> repository,
  IRobotStateService<int> stateUpdateService) : ControllerBase
{
  private static readonly Error _errorInvalidResult = Error.New("Robot repository returned invalid result");
  private static readonly Error _errorInvalidUpdateResult = Error.New("Robot update service returned invalid result");
  private readonly ILogger<RobotController> _logger = logger;
  private readonly IRobotRepository<int, RobotDto> _repository = repository;
  private readonly IRobotStateService<int> _stateUpdateService = stateUpdateService;

  /// <summary>
  /// Endpoint to get all Robots
  /// </summary>
  /// <param name="cancellationToken">cancelation for enumeration</param>
  /// <returns>json response or ndjson stream depending on request header</returns>
  [HttpGet]
  public async IAsyncEnumerable<RobotDto> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (!ModelState.IsValid)
      yield break;

    await foreach (var result in _repository.GetAllAsync(cancellationToken))
    {
      switch(result.Case)
      {
        case RobotDto dto: yield return dto; continue;
        case Error error: _logger.LogGetAllAsyncError(error); yield break;
        default: _logger.LogGetAllAsyncError(_errorInvalidResult); yield break;
      }
    }
  }

  /// <summary>
  /// Endpoint to get a Robot by its <paramref name="id"/>
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  [Route("{id}")]
  public async Task<ActionResult<RobotDto>> GetAsync([FromRoute] int id)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var result = await _repository.GetAsync(id);
    switch(result.Case)
    {
      case RobotDto dto: return Ok(dto);
      case Error error: _logger.LogGetAsyncError(error); return NotFound();
      default: _logger.LogGetAsyncError(_errorInvalidResult); return BadRequest();
    }
  }

  private async Task<ActionResult<RobotDto>> StateUpdateAsyncCore(IRobotStateUpdate<int> robotStateUpdate)
  {
    var result = await _repository.StateUpdateAsync(robotStateUpdate);
    switch (result.Case)
    {
      case RobotDto dto: return Ok(dto);
      case Error error: _logger.LogStateUpdateError(error); return NotFound();
      default: _logger.LogStateUpdateError(_errorInvalidResult); return BadRequest();
    }
  }

  /// <summary>
  /// Endpoint to update a Robot State from external data
  /// </summary>
  /// <param name="robotStateUpdate"></param>
  /// <returns>updated state</returns>
  [HttpPatch]
  public async Task<ActionResult<RobotDto>> StateUpdateAsync([FromBody] RobotStateUpdateDto robotStateUpdate)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    return await StateUpdateAsyncCore(robotStateUpdate);
  }

  /// <summary>
  /// Endpoint to update a Robot State with serverside generated data
  /// </summary>
  /// <param name="id"></param>
  /// <returns>updated state</returns>
  [HttpGet]
  [Route("with-new-state/{id}")]
  public async Task<ActionResult<RobotDto>> StateUpdateAsync([FromRoute] int id)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var oldRobot = await _repository.GetAsync(id);
    switch (oldRobot.Case)
    {
      case RobotDto dto:
        var updateForRobot = await _stateUpdateService.UpdateAsync(dto);
        switch (updateForRobot.Case)
        {
          case IRobotStateUpdate<int> updateDto: return await StateUpdateAsyncCore(updateDto);
          case Error error: _logger.LogStateUpdateError(error); return UnprocessableEntity();
          default: _logger.LogStateUpdateError(_errorInvalidUpdateResult); return BadRequest();
        }
      case Error error: _logger.LogStateUpdateError(error); return NotFound();
      default: _logger.LogStateUpdateError(_errorInvalidResult); return BadRequest();
    }
  }
}

/// <summary>
/// Logging helper
/// </summary>
public static partial class RobotControllerLogExtensions
{
  /// <summary>
  /// Error logging for <see cref="RobotController.GetAllAsync(CancellationToken)"/>
  /// </summary>
  /// <param name="logger"></param>
  /// <param name="error"></param>
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not load robot/s: `{error}`")]
  public static partial void LogGetAllAsyncError(this ILogger logger, Error error);

  /// <summary>
  /// Error logging for <see cref="RobotController.GetAsync(int)"/>
  /// </summary>
  /// <param name="logger"></param>
  /// <param name="error"></param>
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not load robot: `{error}`")]
  public static partial void LogGetAsyncError(this ILogger logger, Error error);

  /// <summary>
  /// Error logging for:<br/>
  /// <see cref="RobotController.StateUpdateAsyncCore(IRobotStateUpdate{int})"/><br/>
  /// <see cref="RobotController.StateUpdateAsync(int)"/><br/>
  /// <see cref="RobotController.StateUpdateAsync(RobotStateUpdateDto)"/>
  /// </summary>
  /// <param name="logger"></param>
  /// <param name="error"></param>
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Could not update state: `{error}`")]
  public static partial void LogStateUpdateError(this ILogger logger, Error error);
}