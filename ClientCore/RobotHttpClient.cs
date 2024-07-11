using LanguageExt;
using System.Runtime.CompilerServices;
using TME1.Abstractions.DataTransferObjects;

namespace TME1.ClientApp;

/// <summary>
/// Storage class for <see cref="IRobot{TKey}"/> context
/// </summary>
public class RobotHttpClient<TRobot>(IHttpClientFactory httpClientFactory) where TRobot : IRobot<int>
{
  private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

  public IAsyncEnumerable<Fin<TRobot>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    => throw new NotImplementedException();

  public Task<Fin<TRobot>> GetAsync(int id)
    => throw new NotImplementedException();

  private Task<Fin<TRobot>> StateUpdateAsync(int robotId)
    => throw new NotImplementedException();
}
