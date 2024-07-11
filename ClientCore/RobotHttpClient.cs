using LanguageExt;
using LanguageExt.Common;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TME1.ClientCore.Models;

namespace TME1.ClientCore;

/// <summary>
/// Http client wrapper for Robot context
/// </summary>
/// <param name="httpClientFactory">factory for http clients</param>
/// <param name="connectionString">conection string to use while creating http client</param>
public class RobotHttpClient(IHttpClientFactory httpClientFactory, string connectionString) : IRobotHttpClient
{
  private const string _ndJsonMediaType = "application/x-ndjson";
  private const string _jsonMediaType = "application/json";
  private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
  {
    PropertyNameCaseInsensitive = true, ///since json names are not CamelCase
  };

  private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
  private readonly string _connectionString = connectionString;

  /// <inheritdoc/>
  public async IAsyncEnumerable<Fin<RobotModel>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ndJsonMediaType));
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_jsonMediaType));

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
      case _jsonMediaType:
        var jsonContent = await content.ReadAsAsync<IEnumerable<RobotModel>>(cancellationToken);
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

      case _ndJsonMediaType:
        var ndjsonContent = ReadFromNdjsonAsync<RobotModel>(content, cancellationToken);
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

  /// <inheritdoc/>
  public async Task<Fin<RobotModel>> GetAsync(int id, CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_jsonMediaType));

    var result = await httpClient.GetAsync($"/api/robots/{id}", cancellationToken);
    if (!result.IsSuccessStatusCode)
      return Error.New("Failed to read");

    var item = await result.Content.ReadAsAsync<RobotModel>();
    if (item is null)
      return Error.New("Item was null");

    return item;
  }

  /// <inheritdoc/>
  public async Task<Fin<RobotModel>> StateUpdateAsync(int id, CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_jsonMediaType));

    var result = await httpClient.GetAsync($"/api/robots/with-new-state/{id}", cancellationToken);
    if (!result.IsSuccessStatusCode)
      return Error.New("Failed to read");

    var item = await result.Content.ReadAsAsync<RobotModel>();
    if (item is null)
      return Error.New("Item was null");

    return item;
  }

  /// <summary>
  /// Reads new line delimited json stream from <paramref name="content"/>
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  /// <param name="content"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  private static async IAsyncEnumerable<TValue> ReadFromNdjsonAsync<TValue>(HttpContent content, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    using var contentStream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    using var contentStreamReader = new StreamReader(contentStream);

    while (!contentStreamReader.EndOfStream)
    {
      var line = await contentStreamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false);

      if (line is null)
        continue;

      var item = JsonSerializer.Deserialize<TValue>(line, _jsonSerializerOptions);

      if (item is null)
        continue;

      yield return item;
    }
  }
}
