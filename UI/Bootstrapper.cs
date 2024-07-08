using Microsoft.Extensions.Hosting;
using Lamar.Microsoft.DependencyInjection;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace TME1.UI;
/// <summary>
/// Application root of composition
/// </summary>
internal class Bootstrapper : IDisposable
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
  internal Bootstrapper(params string[]? args) => AppHost = CreateHostBuilder(args).Build();

  /// <summary>
  /// Compose the application
  /// </summary>
  /// <param name="args"></param>
  /// <returns></returns>
  private static IHostBuilder CreateHostBuilder(string[]? args) => Host.CreateDefaultBuilder(args)
    .UseLamar()
    .UseSerilog()
    .ConfigureServices(services=>services
      .AddMediatR(configuration => configuration
        .RegisterServicesFromAssemblyContaining<Bootstrapper>()));

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
