using System.Collections.ObjectModel;
using TME1.ClientApp.Components.Robot;
using TME1.ClientApp.Mapping;
using TME1.ClientCore;
using TME1.ClientCore.Models;

namespace TME1.ClientApp.Stores;
/// <summary>
/// Robot state storage
/// </summary>
/// <param name="httpClient"></param>
/// <param name="mapper"></param>
public class RobotStore(IRobotHttpClient httpClient, IMapper mapper)
{
  private readonly IRobotHttpClient _httpClient = httpClient;
  private readonly IMapper _mapper = mapper;
  /// <summary>
  /// Used for quickly replace one entry and produce new list
  /// </summary>
  private readonly Dictionary<int, RobotTileViewModel> _robotLookup = [];
  
  /// <summary>
  /// A view on all robots
  /// </summary>
  public ObservableCollection<RobotTileViewModel> RobotTiles { get; private set; } = [];
  /// <summary>
  /// last updated robot by <see cref="StateUpdateAsyncCommand(int, CancellationToken)"/>
  /// </summary>
  public RobotTileViewModel? LastUpdatedRobotTile { get; private set; }

  /// <summary>
  /// Queries for all robots
  /// Updates <see cref="RobotTiles"/>
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task GetAllAsyncCommand(CancellationToken cancellationToken = default)
  {
	await foreach (var robot in _httpClient.GetAllAsync(cancellationToken))
	{
	  switch(robot.Case)
	  {
		case RobotModel model: _robotLookup[model.Id] = _mapper.MapToViewModel(model); continue;
        default: return;
      }
    }
    RobotTiles = [.. _robotLookup.Values];
  }

  /// <summary>
  /// Queries for new robot state based on <paramref name="id"/> of a robot 
  /// Updates <see cref="RobotTiles"/> and <see cref="LastUpdatedRobotTile"/>
  /// </summary>
  /// <param name="id"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task StateUpdateAsyncCommand(int id, CancellationToken cancellationToken = default)
  {
	var result = await _httpClient.StateUpdateAsync(id, cancellationToken);
	switch(result.Case)
    {
      case RobotModel model:
        var viewModel = _mapper.MapToViewModel(model);
        _robotLookup[model.Id] = viewModel;
        RobotTiles = [.. _robotLookup.Values];
        LastUpdatedRobotTile = viewModel;
        break;
      default: return;
    }
  }
}
