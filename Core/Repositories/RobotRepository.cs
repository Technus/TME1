using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Repositories;
using TME1.Core.DataTransferObjects;

namespace TME1.Core.Repositories;
public class RobotRepository(DbContext context, ILogger<RobotRepository> logger) : IRobotRepository<int, RobotDTO>
{
  private readonly DbContext _context = context;
  private readonly ILogger<RobotRepository> _logger = logger;

  public Task<Fin<bool>> DeleteAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
  public Task<Fin<bool>> DeleteAsync(int key, CancellationToken cancellationToken = default) => throw new NotImplementedException();
  
  public async IAsyncEnumerable<Fin<RobotDTO>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    yield break;
  }

  public Task<Fin<RobotDTO>> GetAsync(int key, CancellationToken cancellationToken = default) => throw new NotImplementedException();
  public Task<Fin<RobotDTO>> StateUpdateAsync(IRobotStateUpdate<int> stateUpdate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
  public Task<Fin<RobotDTO>> AddOrUpdateAsync(RobotDTO robot, CancellationToken cancellationToken = default) => throw new NotImplementedException();
  public Task<Fin<bool>> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
