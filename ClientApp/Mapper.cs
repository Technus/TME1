using ExpressMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TME1.ClientApp;
/// <summary>
/// Mapper Initialization
/// </summary>
public static class Mapper
{
  /// <summary>
  /// Extension method to insert <see cref="IMappingServiceProvider"/> into services of <paramref name="hostBuilder"/>
  /// </summary>
  /// <param name="hostBuilder">host to add <see cref="IMappingServiceProvider"/> to</param>
  /// <returns>same as <paramref name="hostBuilder"/></returns>
  public static IHostBuilder UseMapper(this IHostBuilder hostBuilder)
    => hostBuilder.ConfigureServices(services
      => services.AddSingleton<IMappingServiceProvider>(GetConfiguredMapper()));

  /// <summary>
  /// Creates a configured Model<->ViewModel mapper
  /// </summary>
  /// <returns></returns>
  internal static MappingServiceProvider GetConfiguredMapper()
  {
    var mapper = new MappingServiceProvider();
    //TODO: Add needed mappings from Model to ViewModel and back 
    return mapper;
  }
}
