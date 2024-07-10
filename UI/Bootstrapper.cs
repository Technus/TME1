using Microsoft.Extensions.Hosting;
using Lamar.Microsoft.DependencyInjection;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TME1.UI.ViewModels;
using TME1.UI.Views;
using ExpressMapper;

namespace TME1.UI;
/// <summary>
/// Application root of composition
/// </summary>
public class Bootstrapper : IDisposable
{
  private bool _disposedValue;

  /// <summary>
  /// Constructed host
  /// </summary>
  internal IHost AppHost { get; init; }

  /// <summary>
  /// Composes the application and initializes <see cref="AppHost"/>
  /// </summary>
  /// <param name="args"></param>
  public Bootstrapper(params string[]? args) => AppHost = CreateHostBuilder(args).Build();

  /// <summary>
  /// Compose the application
  /// </summary>
  /// <param name="args"></param>
  /// <returns></returns>
  private static IHostBuilder CreateHostBuilder(string[]? args) => Host
    .CreateDefaultBuilder(args)
    .UseLamar()
    .UseSerilog()
    .UseMapper()
    .ConfigureServices(services=> services
      .AddSingleton<IMappingServiceProvider, MappingServiceProvider>()
      .AddSingleton<MainWindow>()
      .AddSingleton<MainWindowViewModel>()
      .AddMediatR(configuration => configuration
        .RegisterServicesFromAssemblyContaining<Bootstrapper>()));

  /// <summary>
  /// Starts the apphost and sets the main window and its data context.
  /// </summary>
  /// <param name="application">the holder application of bootstrapper</param>
  /// <remarks>
  /// Instead of using App.xaml to load main window we do it manually.
  /// It is done to get the right objects from IoC and not create them twice.
  /// </remarks>
  public void Start(Application application)
  {
    AppHost.Start();

    application.MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
    application.MainWindow.DataContext = AppHost.Services.GetRequiredService<MainWindowViewModel>();
    application.MainWindow.Show();
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        AppHost.Dispose();
      }
      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
