using TME1.ClientApp.ViewModels;
using TME1.ClientCore.Models;

namespace TME1.ClientApp.Mapping;
/// <summary>
/// Simple interface to abstract mapping done in the application
/// </summary>
public interface IMapper
{
  /// <summary>
  /// Converts robot view model to it's model
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  RobotModel MapToModel(RobotTileViewModel viewModel);

  /// <summary>
  /// Converts robot model to it's view model
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  RobotTileViewModel MapToViewModel(RobotModel model);
}