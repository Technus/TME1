using AutoFixture;
using LanguageExt.Pipes;
using RichardSzalay.MockHttp;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TME1.Abstractions.DataTransferObjects;
using TME1.ClientApp;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.Tests.ClientCore;
public class TestsRobotHttpClient : TestsBase<RobotHttpClient<RobotDto>>
{
  private const string _connectionString = "http://localhost";

  protected override RobotHttpClient<RobotDto> CreateSUT(IFixture fixture)
  {
    fixture.Inject(Substitute.For<IHttpClientFactory>());
    fixture.Inject(new MockHttpMessageHandler());

    return new RobotHttpClient<RobotDto>(
      fixture.Create<IHttpClientFactory>(),
      _connectionString);
  }

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(10)]
  public async Task GetAllAsync_ShouldReturnAllData_InJson(int count)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    using var httpMock = fixture.Create<MockHttpMessageHandler>();
    httpMock.When($"{_connectionString}/api/robots")
      .Respond("application/json", JsonSerializer.Serialize(Fakers.RobotDto.Generate(count,Fakers.Populated)));

    var httpClient = httpMock.ToHttpClient();
    httpClient.BaseAddress = new Uri(_connectionString);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var clientFactory = fixture.Create<IHttpClientFactory>();
    clientFactory.CreateClient(_connectionString).Returns(httpClient);

    var downloadedCount = 0;
    //Act
    await foreach (var item in sut.GetAllAsync())
    {
      if(item.TryGetValue(out var robot))
        downloadedCount++;
    }
    //Assert
    downloadedCount.Should().Be(count, "because it should return all entries");
    //TODO: check data vailidity
  }

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(10)]
  public async Task GetAllAsync_ShouldReturnAllData_InNdjson(int count)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    using var stream = new MemoryStream();
    var robots = Fakers.RobotDto.Generate(count, Fakers.Populated);
    foreach (var robot in robots)
    {
      await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(robot)));
      stream.WriteByte((byte)'\n');
    }
    await stream.FlushAsync();
    stream.Seek(0, SeekOrigin.Begin);

    using var httpMock = fixture.Create<MockHttpMessageHandler>();
    httpMock.When($"{_connectionString}/api/robots")
      .Respond("application/x-ndjson", stream);

    var httpClient = httpMock.ToHttpClient();
    httpClient.BaseAddress = new Uri(_connectionString);

    var clientFactory = fixture.Create<IHttpClientFactory>();
    clientFactory.CreateClient(_connectionString).Returns(httpClient);

    var downloadedCount = 0;
    //Act
    await foreach (var item in sut.GetAllAsync())
    {
      if (item.TryGetValue(out var robot))
        downloadedCount++;
    }
    //Assert
    downloadedCount.Should().Be(count, "because it should return all entries");
    //TODO: check data vailidity
  }
}
