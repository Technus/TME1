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
  private const string _ndJson = "application/x-ndjson";
  private const string _json = "application/json";

  private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
  private readonly string _connectionString = connectionString;

  public async IAsyncEnumerable<Fin<TRobot>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ndJson));
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_json));

    var result = await httpClient.GetAsync("/api/robots", cancellationToken);
    if (!result.IsSuccessStatusCode)
    {
      yield return Error.New("Failed to read all");
      yield break;
    }

    var content = result.Content;
    var mediaType = content.Headers.ContentType?.MediaType;
    switch (mediaType)
    {
      case _json:
        var jsonContent = await content.ReadAsAsync<IEnumerable<TRobot>>(cancellationToken);
        foreach (var item in jsonContent)
        {
          if (item is null)
          {
            yield return Error.New("Item was null");
            yield break;
          }
          yield return item;
        }
        yield break;

      case _ndJson:
        var ndjsonContent = ReadFromNdjsonAsync<TRobot>(content, cancellationToken);
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

  public async Task<Fin<TRobot>> GetAsync(int id, CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_json));

    var result = await httpClient.GetAsync($"/api/robots/{id}", cancellationToken);
    if (!result.IsSuccessStatusCode)
      return Error.New("Failed to read");

    var item = await result.Content.ReadAsAsync<TRobot>();
    if(item is null)
      return Error.New("Item was null");

    return item;
  }

  private Task<Fin<TRobot>> StateUpdateAsync(int robotId, CancellationToken cancellationToken = default)
    => throw new NotImplementedException();

  public static async IAsyncEnumerable<TValue?> ReadFromNdjsonAsync<TValue>(HttpContent content, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    using var contentStream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    using var contentStreamReader = new StreamReader(contentStream);

    while (!contentStreamReader.EndOfStream)
    {
      var line = await contentStreamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false);

      if (line is null)
        continue;

      yield return JsonSerializer.Deserialize<TValue>(line);
    }
  }
}
