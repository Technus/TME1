namespace TME1.ClientCore;
public class HttpClientWithBaseAddressFactory : IHttpClientFactory
{
  public HttpClient CreateClient(string name)
  {
    var httpClient = new HttpClient
    {
      BaseAddress = new Uri(name)
    };
    return httpClient;
  }
}
