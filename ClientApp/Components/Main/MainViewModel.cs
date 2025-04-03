using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TME1.ClientApp.Components.Robot;
using TME1.ClientApp.Stores;

namespace TME1.ClientApp.Components.Main;

/// <summary>
/// Design time data
/// </summary>
public class MainViewModelDesign : MainViewModel
{
  public MainViewModelDesign()
  {
    for (int i = 0; i < 32; i++)
      AllRobotTiles.Add(new RobotTileViewModel
      {
        Name = i.ToString(),
        Id = i,
        ChargeLevel = i / 100f,
      });
  }
}

/// <summary>
/// View model for Main Window
/// </summary>
/// <remarks>Not a perfect solution to directly use Window for that but in this simple demo app case it will work well</remarks>
public partial class MainViewModel : BaseViewModel
{
  private const int _noSelection = -1;
  private readonly RobotStore _robotStore;
  private int _selectedRobotIndex = _noSelection;
  private RobotTileViewModel? _selectedRobot;

  private ObservableCollection<RobotTileViewModel>? _robotTiles;
  private ListCollectionView? _filteredRobotTiles;

  public MainViewModel() : this(default!) { }

  public MainViewModel(RobotStore robotStore)
  {
    AllRobotTiles = [];
    _robotStore = robotStore;
  }

  /// <summary>
  /// All loaded robots for selection backing
  /// </summary>
  public ObservableCollection<RobotTileViewModel> AllRobotTiles
  {
    get => _robotTiles!;
    set
    {
      if(ReferenceEquals(_robotTiles, value)) 
        return;

      _robotTiles = value;
      OnPropertyChanged();
      ///Also update depdening state
      FilteredRobotTiles = new ListCollectionView(AllRobotTiles)
      {
        Filter = item => item is RobotTileViewModel robot
            && (_selectedRobot is null || ReferenceEquals(_selectedRobot,robot)),
      };
    }
  }

  /// <summary>
  /// Used to present only the selected robot when a selection is made, otherwise see <see cref="AllRobotTiles"/>
  /// </summary>
  public ListCollectionView FilteredRobotTiles
  {
    get => _filteredRobotTiles!;
    set
    {
      if(ReferenceEquals(_filteredRobotTiles, value)) 
        return;

      _filteredRobotTiles = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Currently selected robot from <see cref="AllRobotTiles"/>
  /// </summary>
  public RobotTileViewModel? SelectedRobot
  {
    get => _selectedRobot;
    set
    {
      if (ReferenceEquals(value, _selectedRobot))
        return;

      _selectedRobot = value;
      OnPropertyChanged();
      ///Also update depdening state
      OnPropertyChanged(nameof(CanUpdateRobot));
      FilteredRobotTiles.Refresh();
    }
  }

  /// <summary>
  /// Currently selected robot index from <see cref="AllRobotTiles"/>
  /// </summary>
  public int SelectedRobotIndex
  {
    get => _selectedRobotIndex;
    set
    {
      if (_selectedRobotIndex == value)
        return;
      _selectedRobotIndex = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Load robots command
  /// </summary>
  /// <returns></returns>
  [RelayCommand]
  private async Task LoadRobotsAsync()
  {
    await _robotStore.GetAllAsyncCommand();
    AllRobotTiles = _robotStore.RobotTiles;
    SelectedRobot = default;

    OnPropertyChanged(nameof(CanNavigate));
  }

  /// <summary>
  /// Update robot command
  /// </summary>
  /// <returns></returns>
  [RelayCommand]
  private async Task UpdateRobotAsync()
  {
    if(SelectedRobot is not RobotTileViewModel robot)
      return;

    await _robotStore.StateUpdateAsyncCommand(robot!.Id);
    AllRobotTiles = _robotStore.RobotTiles;
    SelectedRobot = _robotStore.LastUpdatedRobotTile;
  }

  /// <summary>
  /// IsEnabled for Update Robot command
  /// </summary>
  public bool CanUpdateRobot => SelectedRobot is not null;

  /// <summary>
  /// Navigate `&lt;` command 
  /// </summary>
  /// <returns></returns>
  [RelayCommand]
  private void NavigateLeft()
  {
    if (SelectedRobotIndex is _noSelection)
      SelectedRobotIndex = AllRobotTiles.Count - 1;
    else
      SelectedRobotIndex -= 1;
  }

  /// <summary>
  /// Navigate `&gt;` command
  /// </summary>
  /// <returns></returns>
  [RelayCommand]
  private void NavigateRight()
  {
    if (SelectedRobotIndex == AllRobotTiles.Count - 1)
      SelectedRobotIndex = _noSelection;
    else
      SelectedRobotIndex += 1;
  }

  /// <summary>
  /// If navigation/robotSelection should be enabled
  /// </summary>
  /// <remarks>Both `&lt;` and `&gt;` as well as Robot Selection IsEnabled status</remarks>
  public bool CanNavigate => AllRobotTiles.Count > 0;
}
