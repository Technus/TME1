using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Enumerations;

namespace TME1.ClientApp.Components.Robot;

/// <summary>
/// Design time data
/// </summary>
public class RobotTileViewModelDesign : RobotTileViewModel
{
  public RobotTileViewModelDesign()
  {
    Name = "Roboto";
    Model = "Robot 1";
    ChargeLevel = 0.5f;
    Location = "War Saw";
    Position = RobotPosition.Safe;
    PositionMessage = "Cooperating";
    Status = RobotStatus.Normal;
    StatusMessage = "OK";
  }
}

/// <summary>
/// View model for Robot Tiles
/// </summary>
public class RobotTileViewModel : BaseViewModel, IRobot<int>
{
  private string _name = string.Empty;
  private string _model = string.Empty;
  private float _chargeLevel;
  private string? _location;
  private RobotPosition _position;
  private string? _positionMessage;
  private RobotStatus _status;
  private string? _statusMessage;
  private int _id;

  /// <inheritdoc/>
  public string Name
  {
    get => _name;
    set
    {
      if (_name == value)
        return;
      _name = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public string Model
  {
    get => _model;
    set
    {
      if (_model == value)
        return;
      _model = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public float ChargeLevel
  {
    get => _chargeLevel;
    set
    {
      if (_chargeLevel == value)
        return;
      _chargeLevel = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public string? Location
  {
    get => _location;
    set
    {
      if (_location == value)
        return;
      _location = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public RobotPosition Position
  {
    get => _position;
    set
    {
      if (_position == value)
        return;
      _position = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public string? PositionMessage
  {
    get => _positionMessage;
    set
    {
      if (_positionMessage == value)
        return;
      _positionMessage = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public RobotStatus Status
  {
    get => _status;
    set
    {
      if (_status == value)
        return;
      _status = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public string? StatusMessage
  {
    get => _statusMessage;
    set
    {
      if (_statusMessage == value)
        return;
      _statusMessage = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc/>
  public int Id
  {
    get => _id;
    set
    {
      if (_id == value)
        return;
      _id = value;
      OnPropertyChanged();
    }
  }
}
