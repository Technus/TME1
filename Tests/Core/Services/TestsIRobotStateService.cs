using AutoFixture;
using TME1.Abstractions.Services;
using TME1.Core.Services;
using TME1.Tests.Core;
using TME1.Core.DataTransferObjects;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.Repositories;
using LanguageExt.Common;
using LanguageExt;

namespace TME1.Tests.Core.Services;

/// <summary>
/// Actual tests on concrete implementation
/// </summary>
[TestFixture]
[NonParallelizable]
public class TestsRobotStateService : TestsIRobotStateService<RobotStateService, int>
{
  protected override RobotStateService CreateSUT(IFixture fixture)
  {
    fixture.Inject(Substitute.For<IRobotRepository<int, RobotDTO>>());
    fixture.Inject(Substitute.For<ILogger<RobotStateService>>());

    return new RobotStateService(
      fixture.Create<IRobotRepository<int, RobotDTO>>(),
      fixture.Create<ILogger<RobotStateService>>());
  }

  protected override void FillWithData(IFixture fixture)
  {
    var repository = fixture.Create<IRobotRepository<int, RobotDTO>>();
    repository.GetAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(callInfo =>
    {
      var id = (int)callInfo[0];
      var token = (CancellationToken)callInfo[1];

      if (token.IsCancellationRequested)
        return Task.FromResult<Fin<RobotDTO>>(Error.New("Cancelled"));

      if (id == default)
        return Task.FromResult<Fin<RobotDTO>>(Error.New("Default id"));

      if (id % 2 is <= 0)
        return Task.FromResult<Fin<RobotDTO>>(Error.New("Not found"));

      var dto = Fakers.RobotDto.Generate(Fakers.Populated);
      dto.Id = id;
      return Task.FromResult<Fin<RobotDTO>>(dto);
    });
  }

  [Test]
  [TestCase(1)]
  [TestCase(3)]
  [TestCase(5)]
  public new Task UpdateAsync_ShouldProvideSuccestResult_WhenRobotExists(int key)
    => base.UpdateAsync_ShouldProvideSuccestResult_WhenRobotExists(key);

  [Test]
  [TestCase(-2)]
  [TestCase(-1)]
  [TestCase(2)]
  [TestCase(4)]
  [TestCase(6)]
  public new Task UpdateAsync_ShouldProvideFaultedResult_WhenRobotDoesNotExist(int key)
    => base.UpdateAsync_ShouldProvideFaultedResult_WhenRobotDoesNotExist(key);
}

/// <summary>
/// Helper class to reuse code for multiple implementations
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class TestsIRobotStateService<TSUT, TKey> : TestsBase<TSUT> where TSUT : IRobotStateService<TKey>
{
  protected abstract void FillWithData(IFixture fixture);

  [Test]
  public Task UpdateAsync_ShouldProvideFaultedResult_WhenDefaultKeyIsUsed()
    => UpdateAsync_ShouldProvideFaultedResult_WhenRobotDoesNotExist(default!);

  protected async Task UpdateAsync_ShouldProvideSuccestResult_WhenRobotExists(TKey key)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    FillWithData(fixture);
    //Act
    var action = () => sut.UpdateAsync(key);
    //Assert
    var result = (await action.Should().NotThrowAsync()).Subject;
    result.IsFail.Should().BeFalse("because we expect to match an existing robot");

    result.TryGetValue(out var dto).Should().BeTrue();
    dto.Id.Should().Be(key, "because it should provide update for selected robot");
    dto.Status.Should().BeDefined("because status is not a flag");
  }

  protected async Task UpdateAsync_ShouldProvideFaultedResult_WhenRobotDoesNotExist(TKey key)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    FillWithData(fixture);
    //Act
    var action = () => sut.UpdateAsync(key);
    //Assert
    var result = (await action.Should().NotThrowAsync()).Subject;
    result.IsFail.Should().BeTrue("because we expect to not match any existing robot");
  }
}
