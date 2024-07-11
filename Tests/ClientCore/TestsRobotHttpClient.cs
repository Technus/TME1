using AutoFixture;
using TME1.ClientApp;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.Tests.ClientCore;
public class TestsRobotHttpClient : TestsBase<RobotHttpClient<RobotDto>>
{
  protected override RobotHttpClient<RobotDto> CreateSUT(IFixture fixture)
  {
    fixture.Inject(Substitute.For<IHttpClientFactory>());

    return new RobotHttpClient<RobotDto>(
      fixture.Create<IHttpClientFactory>());
  }
}
