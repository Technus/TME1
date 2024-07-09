using System.Windows;
using System.Windows.Controls;
using TME1.Abstractions.Enumerations;

namespace TME1.UI.Views;
/// <summary>
/// Interaction logic for RobotTile.xaml
/// </summary>
public partial class RobotTile : UserControl
{
  public RobotTile()
  {
    InitializeComponent();
  }



  public RobotStatus RobotStatus
  {
    get => (RobotStatus)GetValue(RobotStatusProperty);
    set => SetValue(RobotStatusProperty, value);
  }

  public static readonly DependencyProperty RobotStatusProperty =
      DependencyProperty.Register("RobotStatus", typeof(RobotStatus), typeof(RobotTile), new PropertyMetadata(RobotStatus.None));
}
