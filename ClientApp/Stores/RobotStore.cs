using LanguageExt.Common;
using System.Collections.ObjectModel;
using TME1.ClientApp.Mapping;
using TME1.ClientApp.ViewModels;
using TME1.ClientCore.Models;

namespace TME1.ClientApp.Stores;
public class RobotStore(IRobotHttpClient httpClient, IMapper mapper)
{
  private readonly IRobotHttpClient _httpClient = httpClient;
  private readonly IMapper _mapper = mapper;
  private readonly Dictionary<int, RobotTileViewModel> _robotLookup = [];

  public ObservableCollection<RobotTileViewModel> RobotTiles { get; private set; } = [];

  public async Task GetAllCommandAsync(CancellationToken cancellationToken = default)
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

  public async Task StateUpdateCommandAsync(int id, CancellationToken cancellationToken = default)
  {
	var result = await _httpClient.StateUpdateAsync(id, cancellationToken);
	switch(result.Case)
    {
      case RobotModel model: _robotLookup[model.Id] = _mapper.MapToViewModel(model); break;
      default: return;
    }
	RobotTiles = [.._robotLookup.Values];
  }
}
