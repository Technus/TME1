using System.Windows;

namespace TME1.ClientApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  private Bootstrapper? _bootstrapper;

  /// <summary>
  /// Bootstraps with: <see cref="Bootstrapper.Start(Application)"/>
  /// </summary>
  /// <param name="e"></param>
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);
    _bootstrapper = new Bootstrapper(e.Args);
    _bootstrapper.Start(this);
  }

  protected override void OnExit(ExitEventArgs e)
  {
    base.OnExit(e);
    _bootstrapper?.Dispose();
  }
}

