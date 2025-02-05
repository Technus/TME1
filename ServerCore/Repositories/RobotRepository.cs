using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TME1.Abstractions.DataTransferObjects;
using TME1.ServerCore;
using TME1.ServerCore.DataTransferObjects;
using TME1.ServerCore.Repositories;

namespace TME1.Core.Repositories;
/// <summary>
/// EF Core Robot data access
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
/// <remarks>Could be replaced by CQRS to further refine it into smaller pieces, but this is relatively simple CRUD app</remarks>
public class RobotRepository(
  RobotContext context,
  ILogger<RobotRepository> logger) : IRobotRepository<int, RobotDto>
{
  private readonly RobotContext _context = context;
  private readonly ILogger<RobotRepository> _logger = logger;

  /// <inheritdoc/>
  public async Task<Fin<bool>> DeleteAllAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var deletedCount = await _context.Robots.AsNoTracking().ExecuteDeleteAsync(cancellationToken);
      return deletedCount > 0;
    }
    catch (Exception e)
    {
      var error = Error.New(e);
      _logger.LogDeleteError(error);
      return error;
    }
  }

  /// <inheritdoc/>
  public async Task<Fin<bool>> DeleteAsync(int key, CancellationToken cancellationToken = default)
  {
    try
    {
      var deletedCount = await _context.Robots.AsNoTracking().Where(x => x.Id == key).ExecuteDeleteAsync(cancellationToken);
      return deletedCount > 0;
    }
    catch (Exception e)
    {
      var error = Error.New(e);
      _logger.LogDeleteError(error);
      return error;
    }
  }

  /// <inheritdoc/>
  public async IAsyncEnumerable<Fin<RobotDto>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    await foreach (var robot in _context.Robots.AsNoTracking().AsAsyncEnumerable())
      yield return robot;
  }

  /// <inheritdoc/>
  public async Task<Fin<RobotDto>> GetAsync(int key, CancellationToken cancellationToken = default)
  {
    try
    {
      return await _context.Robots.AsNoTracking().Where(x => x.Id == key).FirstAsync(cancellationToken);
    }
    catch (Exception e)
    {
      var error = Error.New(e);
      _logger.LogGetError(error);
      return error;
    }
  }

  /// <inheritdoc/>
  public async Task<Fin<RobotDto>> StateUpdateAsync(IRobotStateUpdate<int> stateUpdate, CancellationToken cancellationToken = default)
  {
    try
    {
      var whereIdMatches = _context.Robots.AsNoTracking().Where(x => x.Id == stateUpdate.Id);

      var countUpdated = await whereIdMatches.ExecuteUpdateAsync(setters => setters
        .SetProperty(x => x.Status, stateUpdate.Status)
        .SetProperty(x => x.StatusMessage, x => stateUpdate.StatusMessage ?? x.StatusMessage)
        .SetProperty(x => x.ChargeLevel, x => stateUpdate.ChargeLevel)
        , cancellationToken);

      if (countUpdated is 1)
        return await whereIdMatches.SingleAsync(cancellationToken);

      var error = Error.New(stateUpdate.Id, "Failed to update");
      _logger.LogSetError(error);
      return error;
    }
    catch (Exception e)
    {
      var error = Error.New(e);
      _logger.LogGetError(error);
      return error;
    }
  }

  /// <inheritdoc/>
  public async Task<Fin<RobotDto>> AddOrUpdateAsync(RobotDto robot, CancellationToken cancellationToken = default)
  {
    try
    {
      if(robot.Id == default)
      {
        var newEntry = await _context.Robots.AddAsync(robot, cancellationToken);
        if (newEntry is null)
        {
          var error = Error.New(robot.Id, "Failed to insert");
          _logger.LogSetError(error);
          return error;
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        newEntry.State = EntityState.Detached;

        if (saveResult.IsFail)
        {
          var error = saveResult.Case as Error;
          _logger.LogSetError(error!);
          return error;
        }

        return newEntry.Entity;
      }
      else
      {
        var whereIdMatches = _context.Robots.AsNoTracking().Where(x => x.Id == robot.Id);

        var countUpdated = await whereIdMatches.ExecuteUpdateAsync(setters => setters
          .SetProperty(x => x.Name, robot.Name)
          .SetProperty(x => x.Model, robot.Model)
          .SetProperty(x => x.ChargeLevel, robot.ChargeLevel)
          .SetProperty(x => x.Location, robot.Location)
          .SetProperty(x => x.Position, robot.Position)
          .SetProperty(x => x.PositionMessage, robot.PositionMessage)
          .SetProperty(x => x.Status, robot.Status)
          .SetProperty(x => x.StatusMessage, robot.StatusMessage)
          , cancellationToken);

        if (countUpdated is 1)
          return await whereIdMatches.SingleAsync(cancellationToken);

        var error = Error.New(robot.Id, "Failed to update");
        _logger.LogSetError(error);
        return error;
      }
    }
    catch (Exception e)
    {
      var error = Error.New(e);
      _logger.LogGetError(error);
      return error;
    }
  }

  private async Task<Fin<bool>> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var savedCount = await _context.SaveChangesAsync(cancellationToken);
      return savedCount > 0;
    }
    catch (Exception ex)
    {
      var error = Error.New(ex);
      _logger.LogSaveError(error);
      return error;
    }
  }
}

public static partial class RobotRepositoryLogExtensions
{
  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Failed to delete: `{error}`")]
  public static partial void LogDeleteError(this ILogger logger, Error error);

  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Failed to get: `{error}`")]
  public static partial void LogGetError(this ILogger logger, Error error);

  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Failed to set: `{error}`")]
  public static partial void LogSetError(this ILogger logger, Error error);

  [LoggerMessage(
    Level = LogLevel.Error,
    Message = "Failed to save: `{error}`")]
  public static partial void LogSaveError(this ILogger logger, Error error);
}

