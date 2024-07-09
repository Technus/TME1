using System.Collections.ObjectModel;

namespace TME1.UI.ViewModels;

/// <summary>
/// Design time data
/// </summary>
public class MainWindowViewModelDesign : MainWindowViewModel
{
  public MainWindowViewModelDesign()
  {
    for (int i = 0; i < 32; i++)
      VisibleRobotTiles.Add(new RobotTileViewModel());
  }
}

/// <summary>
/// View model for Main Window
/// </summary>
/// <remarks>Not a perfect solution to directly use Window for that but in this simple demo app case it will work well</remarks>
public class MainWindowViewModel : BaseViewModel
{
  public MainWindowViewModel()
  {
    for (int i = 0; i < 32; i++)
      VisibleRobotTiles.Add(new RobotTileViewModel());
  }

  public ObservableCollection<RobotTileViewModel> VisibleRobotTiles { get; } = [];
}
