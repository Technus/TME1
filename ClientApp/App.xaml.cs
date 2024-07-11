using System.Windows;

namespace TME1.ClientApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application, IDisposable
{
  private Bootstrapper? _bootstrapper;
  private bool disposedValue;

  /// <summary>
  /// Bootstraps with: <see cref="Bootstrapper.StartAsync(Application)"/>
  /// </summary>
  /// <param name="e"></param>
  protected override async void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    var bootstrapper = new Bootstrapper(e.Args);
    _bootstrapper = bootstrapper;
    await bootstrapper.StartAsync(this);
  }

  protected override async void OnExit(ExitEventArgs e)
  {
    base.OnExit(e);

    if (_bootstrapper is Bootstrapper bootstrapper)
      await bootstrapper.StopAsync(this);
  }

  private void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        _bootstrapper?.Dispose();
      }
      disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}

