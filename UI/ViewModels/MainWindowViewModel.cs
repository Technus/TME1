using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace TME1.UI.ViewModels;

/// <summary>
/// Design time data
/// </summary>
public class MainWindowViewModelDesign : MainWindowViewModel
{
  public MainWindowViewModelDesign()
  {
    for (int i = 0; i < 32; i++)
      AllRobotTiles.Add(new RobotTileViewModel
      {
        Id = i,
        ChargeLevel = i / 100f,
      });
  }
}

/// <summary>
/// View model for Main Window
/// </summary>
/// <remarks>Not a perfect solution to directly use Window for that but in this simple demo app case it will work well</remarks>
public partial class MainWindowViewModel : BaseViewModel
{
  private const int _noSelection = -1;
  private int _selectedRobotIndex = _noSelection;
  private RobotTileViewModel? _selectedRobot;

  private ObservableCollection<RobotTileViewModel> _robotTiles;
  private ListCollectionView _filteredRobotTiles;

  public MainWindowViewModel()
  {
    AllRobotTiles = [];
  }

  public ObservableCollection<RobotTileViewModel> AllRobotTiles
  {
    get => _robotTiles;
    set
    {
      if(ReferenceEquals(_robotTiles, value)) 
        return;
      _robotTiles = value;
      OnPropertyChanged();
      FilteredRobotTiles = new ListCollectionView(AllRobotTiles)
      {
        Filter = item => item is RobotTileViewModel robot
            && (_selectedRobot is null || ReferenceEquals(_selectedRobot,robot)),
      };
    }
  }

  public ListCollectionView FilteredRobotTiles
  {
    get => _filteredRobotTiles;
    set
    {
      if(ReferenceEquals(_filteredRobotTiles, value)) 
        return;
      _filteredRobotTiles = value;
      OnPropertyChanged();
    }
  }

  public RobotTileViewModel? SelectedRobot
  {
    get => _selectedRobot;
    set
    {
      if (ReferenceEquals(value, _selectedRobot))
        return;
      _selectedRobot = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(CanUpdateRobot));
      FilteredRobotTiles.Refresh();
    }
  }

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

  [RelayCommand]
  private async Task LoadRobotsAsync()
  {
    for (int i = 0; i < 4; i++)
      AllRobotTiles.Add(new RobotTileViewModel
      {
        Id = i,
        ChargeLevel = i / 100f,
      });
    OnPropertyChanged(nameof(CanNavigate));
  }

  [RelayCommand]
  private async Task UpdateRobotAsync()
  {
    AllRobotTiles.Clear();
  }

  public bool CanUpdateRobot => _selectedRobot is not null;

  [RelayCommand]
  private async Task NavigateLeftAsync()
  {
    if (SelectedRobotIndex is _noSelection)
      SelectedRobotIndex = AllRobotTiles.Count - 1;
    else
      SelectedRobotIndex -= 1;
  }

  [RelayCommand]
  private async Task NavigateRightAsync()
  {
    if (SelectedRobotIndex == AllRobotTiles.Count - 1)
      SelectedRobotIndex = _noSelection;
    else
      SelectedRobotIndex += 1;
  }

  public bool CanNavigate => AllRobotTiles.Count > 0;
}
