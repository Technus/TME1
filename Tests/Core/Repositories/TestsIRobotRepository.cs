using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TME1.Abstractions;
using TME1.Abstractions.DataTransferObjects;
using TME1.Abstractions.Repositories;
using TME1.Core.DataTransferObjects;
using TME1.Core.Repositories;
using TME1.Tests.Core;

namespace TME1.Tests.Repositories;

/// <summary>
/// Actual tests on concrete implementation
/// </summary>
[TestFixture]
[NonParallelizable]
public class TestsRobotRepository : TestsIRobotRepository<RobotRepository, int, RobotDTO>
{
  protected override RobotRepository CreateSUT(IFixture fixture)
  {
    fixture.Inject(new MockRobotContext());
    fixture.Inject(Substitute.For<ILogger<RobotRepository>>());

    return new RobotRepository(
      fixture.Create<MockRobotContext>(),
      fixture.Create<ILogger<RobotRepository>>());
  }

  protected override RobotDTO CreateRobotWithId(IFixture fixture, int id)
  {
    var robot = Fakers.RobotDto.Generate(Fakers.Populated);
    robot.Id = id;
    return robot;
  }

  protected override async Task<RobotDTO> GetRobotWithId(IFixture fixture, int id)
  {
    var context = fixture.Create<MockRobotContext>();
    return await context.Robots.AsNoTracking().Where(robot => robot.Id == id).FirstAsync();
  }

  protected override IRobotStateUpdate<int> CreateRobotStateUpdateWithId(IFixture fixture, int id, string ruleSet = Fakers.Populated)
  {
    var robot = Fakers.RobotStateUpdateDTO.Generate(ruleSet);
    robot.Id = id;
    return robot;
  }

  protected override async Task FillWithData(IFixture fixture, int count)
  {
    var context = fixture.Create<MockRobotContext>();
    await context.Robots.AddRangeAsync(Fakers.RobotDto.Generate(count, Fakers.Populated));
    await context.SaveChangesAsync();
  }

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(2)]
  [TestCase(3)]
  public new Task GetAllAsync_ShouldReturnWholeCollection(int count)
    => base.GetAllAsync_ShouldReturnWholeCollection(count);

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(2)]
  [TestCase(3)]
  public new Task DeleteAllAsync_ShouldReturnTrueWhenDeletedAnything(int count)
    => base.DeleteAllAsync_ShouldReturnTrueWhenDeletedAnything(count);

  [Test]
  [TestCase(11,10)]
  [TestCase(20,10)]
  public new Task UpsertAsync_ShouldFail_WhenKeyIsMissing(int key, int count)
    => base.UpsertAsync_ShouldFail_WhenKeyIsMissing(key, count);

  [Test]
  [TestCase(1,10)]
  [TestCase(7,10)]
  public new Task UpsertAsync_ShouldUpdate_WhenKeyIsPresent(int key, int count)
    => base.UpsertAsync_ShouldUpdate_WhenKeyIsPresent(key, count);

  [Test]
  [TestCase(11, 10)]
  [TestCase(20, 10)]
  public new Task StateUpdateAsync_ShouldFail_WhenKeyIsMissing(int key, int count)
    => base.StateUpdateAsync_ShouldFail_WhenKeyIsMissing(key, count);

  [Test]
  [TestCase(1, 10)]
  [TestCase(7, 10)]
  public new Task StateUpdateAsync_ShouldUpdate_WhenKeyIsPresent(int key, int count)
    => base.StateUpdateAsync_ShouldUpdate_WhenKeyIsPresent(key, count);
}

/// <summary>
/// Helper class to reuse code for multiple implementations
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TRobot"></typeparam>
public abstract class TestsIRobotRepository<TSUT, TKey, TRobot> : TestsBase<TSUT> where TSUT : IRobotRepository<TKey, TRobot>
  where TRobot : IRobot<TKey>
{
  protected abstract Task FillWithData(IFixture fixture, int count);

  protected abstract TRobot CreateRobotWithId(IFixture fixture, TKey id);
  protected abstract Task<TRobot> GetRobotWithId(IFixture fixture, TKey id);
  protected abstract IRobotStateUpdate<TKey> CreateRobotStateUpdateWithId(IFixture fixture, TKey id, string ruleSet = Fakers.Populated);

  protected async Task GetAllAsync_ShouldReturnWholeCollection(int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    var measuredCount = 0;
    //Act
    await foreach (var item in sut.GetAllAsync())
    {
      //Assert
      item.IsFail.Should().BeFalse("because we expect valid data");
      measuredCount++;
    }
    //Assert
    measuredCount.Should().Be(count, "because whole collection should be returned");
    //TODO: Assert that Valid data was provided
  }

  protected async Task DeleteAllAsync_ShouldReturnTrueWhenDeletedAnything(int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    //Act
    var result = await sut.DeleteAllAsync();
    //Assert
    result.TryGetValue(out var deletedAny).Should().BeTrue();
    deletedAny.Should().Be(count>0, "because should return true if operation was performed on any data");
  }

  [Test]
  [TestCase(10)]
  public async Task UpsertAsync_ShouldInsertNewEntry_WhenKeyIsDefault(int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    var dto = CreateRobotWithId(fixture, default!);
    var context = fixture.Create<MockRobotContext>();
    var comparisonConfig = new ComparisonConfig();
    comparisonConfig.IgnoreProperty<TRobot>(x => x.Id);
    //Act
    var result = await sut.AddOrUpdateAsync(dto);
    //Assert
    result.TryGetValue(out var insertedNew).Should().BeTrue("because insertion should succed");
    (await context.Robots.CountAsync()).Should().Be(count + 1, "because we inserted new entry");
    insertedNew.ShouldCompare(dto, "because data should be persisted", comparisonConfig);
  }

  protected async Task UpsertAsync_ShouldFail_WhenKeyIsMissing(TKey key, int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    var dto = CreateRobotWithId(fixture, key);
    var context = fixture.Create<MockRobotContext>();
    //Act
    var result = await sut.AddOrUpdateAsync(dto);
    //Assert
    result.IsFail.Should().BeTrue("because row with that key is missing");
    (await context.Robots.CountAsync()).Should().Be(count, "because nothing happened");
  }

  protected async Task UpsertAsync_ShouldUpdate_WhenKeyIsPresent(TKey key, int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    var dto = CreateRobotWithId(fixture, key);
    var context = fixture.Create<MockRobotContext>();
    //Act
    var result = await sut.AddOrUpdateAsync(dto);
    //Assert
    result.TryGetValue(out var updated).Should().BeTrue("because row with that key exists");
    (await context.Robots.CountAsync()).Should().Be(count, "because it was just updated");
    updated.ShouldCompare(dto, "because data should be persisted");
  }

  [Test]
  [TestCase(10)]
  public Task StateUpdateAsync_ShouldFail_WhenKeyIsDefault(int count)
    => StateUpdateAsync_ShouldFail_WhenKeyIsMissing(default!, count);

  protected async Task StateUpdateAsync_ShouldFail_WhenKeyIsMissing(TKey key, int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    var updateRequest = CreateRobotStateUpdateWithId(fixture, key);
    var context = fixture.Create<MockRobotContext>();
    //Act
    var result = await sut.StateUpdateAsync(updateRequest);
    //Assert
    result.IsFail.Should().BeTrue("because row with that key is missing");
    (await context.Robots.CountAsync()).Should().Be(count, "because nothing happened");
  }

  protected async Task StateUpdateAsync_ShouldUpdate_WhenKeyIsPresent(TKey key, int count)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillWithData(fixture, count);
    var context = fixture.Create<MockRobotContext>();
    var updateRequest = CreateRobotStateUpdateWithId(fixture, key);
    var oldRobot = await GetRobotWithId(fixture, key);
    //Act
    var result = await sut.StateUpdateAsync(updateRequest);
    //Assert
    var newRobot = await GetRobotWithId(fixture, key);

    result.TryGetValue(out var updatedRobot).Should().BeTrue("because row with that key exists");
    (await context.Robots.CountAsync()).Should().Be(count, "because it was just updated");

    updatedRobot.Id.Should().Be(key, "because it should edit correct entry");

    updatedRobot.Status.Should().Be(updateRequest.Status);
    newRobot.Status.Should().Be(updateRequest.Status);

    if (updateRequest.StatusMessage is not null)
    {
      updatedRobot.StatusMessage.Should().Be(updateRequest.StatusMessage);
      newRobot.StatusMessage.Should().Be(updateRequest.StatusMessage);
    }
    else
    {
      updatedRobot.StatusMessage.Should().Be(oldRobot.StatusMessage);
      newRobot.StatusMessage.Should().Be(oldRobot.StatusMessage);
    }

    if (updateRequest.ChargeLevel is not 0)
    {
      updatedRobot.ChargeLevel.Should().Be(float.Clamp(oldRobot.ChargeLevel + updateRequest.ChargeLevel, float.NegativeInfinity, 1));
      newRobot.ChargeLevel.Should().Be(float.Clamp(oldRobot.ChargeLevel + updateRequest.ChargeLevel, float.NegativeInfinity, 1));
    }
    else
    {
      updatedRobot.ChargeLevel.Should().Be(oldRobot.ChargeLevel);
      newRobot.ChargeLevel.Should().Be(oldRobot.ChargeLevel);
    }
  }
}