using LanguageExt;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Repositories;
using TME1.Abstractions.Services;
using TME1.Core.DataTransferObjects;

namespace TME1.Core.Services;
public class RobotStateService(IRobotRepository<int, RobotDTO> repository, ILogger<RobotStateService> logger) : IRobotStateService<int>
{
  private readonly IRobotRepository<int, RobotDTO> _repository = repository;
  private readonly ILogger<RobotStateService> _logger = logger;

  public Task<Fin<IRobotStateUpdate<int>>> UpdateAsync(int robot, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
