using Microsoft.Extensions.Hosting;
using System.Windows;

namespace TME1.UI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  private Bootstrapper? _bootstrapper;

  protected override void OnStartup(StartupEventArgs e)
  {
    _bootstrapper = new Bootstrapper(e.Args);
    _bootstrapper.AppHost.Start();
    base.OnStartup(e);
  }

  protected override void OnExit(ExitEventArgs e)
  {
    base.OnExit(e);
    _bootstrapper?.Dispose();
  }
}

