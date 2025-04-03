using Microsoft.Extensions.Hosting;
using Lamar.Microsoft.DependencyInjection;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TME1.ClientApp.Mapping;
using System.Net.Http;
using TME1.ClientCore;
using Microsoft.Extensions.Configuration;
using TME1.ClientApp.Stores;
using TME1.ClientApp.Components.Main;

namespace TME1.ClientApp;
/// <summary>
/// Application root of composition
/// </summary>
public sealed class Bootstrapper : IDisposable
{
  private bool _disposedValue;

  /// <summary>
  /// Constructed host
  /// </summary>
  private IHost AppHost { get; }

  /// <summary>
  /// Composes the application and initializes <see cref="AppHost"/>
  /// </summary>
  /// <param name="args"></param>
  public Bootstrapper(params string[]? args) => AppHost = CreateHost(args);

  /// <summary>
  /// Compose the application
  /// </summary>
  /// <param name="args"></param>
  /// <returns></returns>
  private static IHost CreateHost(string[]? args)
  {
    var builder = Host.CreateDefaultBuilder(args);
    builder
      .UseLamar()
      .UseSerilog()
      .ConfigureServices(serviceCollection => serviceCollection
        .AddSingleton<IHttpClientFactory, HttpClientWithBaseAddressFactory>()
        .AddSingleton<IRobotHttpClient>(serviceProvider => new RobotHttpClient(
          serviceProvider.GetRequiredService<IHttpClientFactory>(),
          serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("TME1") ?? "http://localhost:5218"))//TODO: provide configuration in a controlled manner
        .AddSingleton<IMapper, Mapper>()
        .AddSingleton<RobotStore>()
        .AddSingleton<MainWindow>()
        .AddSingleton<MainViewModel>()
      );

    return builder.Build();
  }

  /// <summary>
  /// Starts the apphost and sets the main window and its data context.
  /// </summary>
  /// <param name="application">the holder application of bootstrapper</param>
  /// <remarks>
  /// Instead of using App.xaml to load main window we do it manually.
  /// It is done to get the right objects from IoC and not create them twice.
  /// </remarks>
  public async Task StartAsync(Application application)
  {
    await AppHost.StartAsync();

    application.MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
    application.MainWindow.DataContext = AppHost.Services.GetRequiredService<MainViewModel>();
    application.MainWindow.Show();
  }

  /// <summary>
  /// Stops the app host and closes the main window
  /// </summary>
  /// <param name="application"></param>
  /// <returns></returns>
  public async Task StopAsync(Application application)
  {
    ///For sanity
    application.MainWindow?.Close();

    await AppHost.StopAsync();
  }

  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
        AppHost.Dispose();
      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
