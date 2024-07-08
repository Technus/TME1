using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using TME1.Abstractions;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Repositories;

namespace API.Controllers;

public abstract class RobotControllerBase<TKey, TRobot> : ControllerBase where TRobot : IRobot<TKey>
{
  protected ILogger<RobotControllerBase<TKey, TRobot>> Logger { get; }
  private readonly IRobotRepository<TKey, TRobot> _repository;

  protected RobotControllerBase(ILogger<RobotControllerBase<TKey, TRobot>> logger, IRobotRepository<TKey, TRobot> repository)
  {
    Logger = logger;
    _repository = repository;
  }

  [HttpGet(Name = "GetAll")]
  public async IAsyncEnumerable<TRobot> GetAllAsync()
  {
    await foreach (var result in _repository.GetAllAsync())
    {
      if (result.TryGetValue(out var dto))
      {
        yield return dto;
      }
      else
      {
        LogGetAllAsyncError(result);
        yield break;
      }
    }
  }

  [HttpPost(Name = "StateUpdate")]
  public async Task<IActionResult> StateUpdateAsync(IRobotStateUpdate<TKey> robotStateUpdate)
  {
    if (!ModelState.IsValid)
      return BadRequest();

    var result = await _repository.StateUpdateAsync(robotStateUpdate);
    if(result.TryGetValue(out var dto))
      return Ok(dto);

    LogStateUpdateError(result);
    return NotFound();
  }

  protected abstract void LogGetAllAsyncError(Fin<TRobot> result);

  protected abstract void LogStateUpdateError(Fin<TRobot> result);
}