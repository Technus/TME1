using LanguageExt;
using LanguageExt.Common;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TME1.Abstractions.DataTransferObjects;

namespace TME1.ClientApp;

/// <summary>
/// Http client wrapper for Robot context
/// </summary>
/// <param name="httpClientFactory">factory for http clients</param>
/// <param name="connectionString">conection string to use while creating http client</param>
public class RobotHttpClient<TRobot>(IHttpClientFactory httpClientFactory, string connectionString) where TRobot : IRobot<int>
{
  private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
  private readonly string _connectionString = connectionString;

  public async IAsyncEnumerable<Fin<TRobot>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-ndjson"));
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var result = await httpClient.GetAsync("/api/robots", cancellationToken);
    if (result.IsSuccessStatusCode)
    {
      var content = result.Content;
      var mediaType = content.Headers.ContentType?.MediaType;
      switch (mediaType)
      {
        case "application/json":
          var jsonContent = await content.ReadAsAsync<IEnumerable<TRobot>>();
          foreach (var item in jsonContent)
          {
            if(item is null)
            {
              yield return Error.New("Item was null");
              yield break;
            }
            yield return item;
          }
          yield break;

        case "application/x-ndjson":
          var ndjsonContent = ReadFromNdjsonAsync<TRobot>(content);
          await foreach (var item in ndjsonContent)
          {
            if (item is null)
            {
              yield return Error.New("Item was null");
              yield break;
            }
            yield return item;
          }
          yield break;

        default: yield return Error.New("Not supported"); yield break;
      }
    }
  }

  public Task<Fin<TRobot>> GetAsync(int id)
    => throw new NotImplementedException();

  private Task<Fin<TRobot>> StateUpdateAsync(int robotId)
    => throw new NotImplementedException();

  public static async IAsyncEnumerable<TValue?> ReadFromNdjsonAsync<TValue>(HttpContent content)
  {
    using var contentStream = await content.ReadAsStreamAsync().ConfigureAwait(false);
    using var contentStreamReader = new StreamReader(contentStream);

    while (!contentStreamReader.EndOfStream)
    {
      var line = await contentStreamReader.ReadLineAsync().ConfigureAwait(false);

      if (line is null)
        continue;

      yield return JsonSerializer.Deserialize<TValue>(line);
    }
  }
}
