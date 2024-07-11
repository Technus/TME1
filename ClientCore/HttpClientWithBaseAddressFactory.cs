namespace TME1.ClientCore;
/// <summary>
/// Constructs <see cref="HttpClient"/> instances.<br/>
/// Sets the <see cref="HttpClient.BaseAddress"/> using <see cref="IHttpClientFactory.CreateClient(string)"/> name parameter
/// </summary>
public class HttpClientWithBaseAddressFactory : IHttpClientFactory
{
  /// <summary>
  /// Creates client with <see cref="HttpClient.BaseAddress"/> set to <paramref name="name"/>
  /// </summary>
  /// <param name="name"></param>
  /// <returns>client with <see cref="HttpClient.BaseAddress"/> set to <paramref name="name"/></returns>
  public HttpClient CreateClient(string name)
  {
    var httpClient = new HttpClient
    {
      BaseAddress = new Uri(name)
    };
    return httpClient;
  }
}
