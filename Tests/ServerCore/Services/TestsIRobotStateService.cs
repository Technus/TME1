using AutoFixture;
using TME1.Abstractions.Services;
using TME1.Core.Services;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.DataTransferObjects;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.Tests.ServerCore.Services;

/// <summary>
/// Actual tests on concrete implementation
/// </summary>
[TestFixture]
[NonParallelizable]
public class TestsRobotStateService : TestsIRobotStateService<RobotStateService, int>
{
  protected override RobotStateService CreateSUT(IFixture fixture)
  {
    fixture.Inject(Substitute.For<ILogger<RobotStateService>>());

    return new RobotStateService(
      fixture.Create<ILogger<RobotStateService>>());
  }

  protected override RobotDto CreateRobot(IFixture fixture)
    => Fakers.RobotDto.Generate(Fakers.Populated);
}

/// <summary>
/// Helper class to reuse code for multiple implementations
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class TestsIRobotStateService<TSUT, TKey> : TestsBase<TSUT> where TSUT : IRobotStateService<TKey>
{
  protected abstract IRobot<TKey> CreateRobot(IFixture fixture);

  [Test]
  public async Task UpdateAsync_ShouldProvideSuccestResult_WhenRobotExists()
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    var robot = CreateRobot(fixture);
    //Act
    var action = () => sut.UpdateAsync(robot);
    //Assert
    var result = (await action.Should().NotThrowAsync()).Subject;
    result.IsFail.Should().BeFalse("because we expect to match an existing robot");

    result.TryGetValue(out var dto).Should().BeTrue();
    dto.Id.Should().Be(robot.Id, "because it should provide update for selected robot");
    dto.Status.Should().BeDefined("because status is not a flag");
  }

  [Test]
  public async Task UpdateAsync_ShouldProvideFaultedResult_WhenRobotDoesNotExist()
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    //Act
    var action = () => sut.UpdateAsync(default!);
    //Assert
    var result = (await action.Should().NotThrowAsync()).Subject;
    result.IsFail.Should().BeTrue("because we expect to not match any existing robot");
  }
}
