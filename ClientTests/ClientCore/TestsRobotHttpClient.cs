using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TME1.ClientApp;

namespace TME1.Tests.ClientCore;

public class TestsRobotHttpClient : TestsIRobotHttpClient<RobotHttpClient>
{
  protected override RobotHttpClient CreateSUT(IFixture fixture)
  {
    fixture.Inject(Substitute.For<IHttpClientFactory>());
    fixture.Inject(new MockHttpMessageHandler());

    return new RobotHttpClient(
      fixture.Create<IHttpClientFactory>(),
      _connectionString);
  }
}

public abstract class TestsIRobotHttpClient<TSUT> : TestsBase<TSUT> where TSUT : IRobotHttpClient
{
  protected const string _connectionString = "http://localhost";

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

  [Test]
  [TestCase(true,3)]
  [TestCase(false,4)]
  public async Task GetAsync_ShouldReturnData_WhenPresent(bool contains, int id)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var robotSample = Fakers.RobotDto.Generate("populated");

    using var httpMock = fixture.Create<MockHttpMessageHandler>();
    var mockEndpoint = httpMock.When($"{_connectionString}/api/robots/{id}");
    if (contains)
      mockEndpoint.Respond("application/json", JsonSerializer.Serialize(robotSample));
    else
      mockEndpoint.Respond(HttpStatusCode.NotFound);

    var httpClient = httpMock.ToHttpClient();
    httpClient.BaseAddress = new Uri(_connectionString);

    var clientFactory = fixture.Create<IHttpClientFactory>();
    clientFactory.CreateClient(_connectionString).Returns(httpClient);

    //Act
    var result = await sut.GetAsync(id);
    //Assert
    result.TryGetValue(out var robot).Should().Be(contains, "because that is how the api mock was set up");
    if(contains)
      robot.ShouldCompare(robotSample,"because the data should be preserved");
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task StateUpdateAsync_ShouldReturnData_WhenPresent(bool contains, int id)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var robotSample = Fakers.RobotDto.Generate("populated");

    using var httpMock = fixture.Create<MockHttpMessageHandler>();
    var mockEndpoint = httpMock.When($"{_connectionString}/api/robots/with-new-state/{id}");
    if (contains)
      mockEndpoint.Respond("application/json", JsonSerializer.Serialize(robotSample));
    else
      mockEndpoint.Respond(HttpStatusCode.NotFound);

    var httpClient = httpMock.ToHttpClient();
    httpClient.BaseAddress = new Uri(_connectionString);

    var clientFactory = fixture.Create<IHttpClientFactory>();
    clientFactory.CreateClient(_connectionString).Returns(httpClient);

    //Act
    var result = await sut.StateUpdateAsync(id);
    //Assert
    result.TryGetValue(out var robot).Should().Be(contains, "because that is how the api mock was set up");
    if (contains)
      robot.ShouldCompare(robotSample, "because the data should be preserved");
  }
}
