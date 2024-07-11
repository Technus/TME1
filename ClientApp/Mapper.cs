using Riok.Mapperly.Abstractions;
using TME1.ClientApp.ViewModels;
using TME1.ClientCore.Models;

namespace TME1.ClientApp;
/// <summary>
/// Mapper implementation using source generator
/// </summary>
[Mapper]
public partial class Mapper : IMapper
{
  ///<inheritdoc/>
  public partial RobotTileViewModel MapToModel(RobotModel model);
  ///<inheritdoc/>
  public partial RobotModel MapToViewModel(RobotTileViewModel viewModel);
}
