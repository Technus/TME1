using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.DataTransferObjects;
using TME1.Core.Repositories;
using TME1.ServerCore.DataTransferObjects;
using TME1.ServerCore.Repositories;
using TME1.ServerTests.ServerCore;
using TME1.TestCommon;

namespace TME1.ServerTests.ServerCore.Repositories;

/// <summary>
/// Actual tests on concrete implementation
/// </summary>
[TestFixture]
[NonParallelizable]
public class TestsRobotRepository : TestsIRobotRepository<RobotRepository, int, RobotDto>
{
  protected override RobotRepository CreateSUT(IFixture fixture)
  {
    fixture.Inject(new MockRobotContext());
    fixture.Inject(Substitute.For<ILogger<RobotRepository>>());

    return new RobotRepository(
      fixture.Create<MockRobotContext>(),
      fixture.Create<ILogger<RobotRepository>>());
  }

  protected override RobotDto CreateRobotWithId(IFixture fixture, int idToSet)
  {
    var robot = Fakers.RobotDto.Generate(Fakers.Populated);
    robot.Id = idToSet;
    return robot;
  }

  protected override async Task<RobotDto> FromContextGetRobotWithId(IFixture fixture, int idToGet)
  {
    var context = fixture.Create<MockRobotContext>();
    return await context.Robots.AsNoTracking().Where(robot => robot.Id == idToGet).FirstAsync();
  }

  protected override IRobotStateUpdate<int> CreateRobotStateUpdateWithId(IFixture fixture, int id, string ruleSet = Fakers.Populated)
  {
    var robot = Fakers.RobotStateUpdateDTO.Generate(ruleSet);
    robot.Id = id;
    return robot;
  }

  protected override async Task FillContextWithData(IFixture fixture, int entryCount)
  {
    var context = fixture.Create<MockRobotContext>();
    await context.Robots.AddRangeAsync(Fakers.RobotDto.Generate(entryCount, Fakers.Populated));
    await context.SaveChangesAsync();
  }

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(2)]
  [TestCase(3)]
  public new Task GetAllAsync_ShouldReturnWholeCollection(int expectedCount)
    => base.GetAllAsync_ShouldReturnWholeCollection(expectedCount);

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(2)]
  [TestCase(3)]
  public new Task DeleteAllAsync_ShouldReturnTrueWhenDeletedAnything(int prefillCount)
    => base.DeleteAllAsync_ShouldReturnTrueWhenDeletedAnything(prefillCount);

  [Test]
  [TestCase(11, 10)]
  [TestCase(20, 10)]
  public new Task UpsertAsync_ShouldFail_WhenKeyIsMissing(int key, int prefillCount)
    => base.UpsertAsync_ShouldFail_WhenKeyIsMissing(key, prefillCount);

  [Test]
  [TestCase(1, 10)]
  [TestCase(7, 10)]
  public new Task UpsertAsync_ShouldUpdate_WhenKeyIsPresent(int key, int prefillCount)
    => base.UpsertAsync_ShouldUpdate_WhenKeyIsPresent(key, prefillCount);

  [Test]
  [TestCase(11, 10)]
  [TestCase(20, 10)]
  public new Task StateUpdateAsync_ShouldFail_WhenKeyIsMissing(int key, int prefillCount)
    => base.StateUpdateAsync_ShouldFail_WhenKeyIsMissing(key, prefillCount);

  [Test]
  [TestCase(1, 10)]
  [TestCase(7, 10)]
  public new Task StateUpdateAsync_ShouldUpdate_WhenKeyIsPresent(int key, int prefillCount)
    => base.StateUpdateAsync_ShouldUpdate_WhenKeyIsPresent(key, prefillCount);
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
  protected abstract Task FillContextWithData(IFixture fixture, int entryCount);

  protected abstract TRobot CreateRobotWithId(IFixture fixture, TKey idToSet);
  protected abstract Task<TRobot> FromContextGetRobotWithId(IFixture fixture, TKey idToGet);
  protected abstract IRobotStateUpdate<TKey> CreateRobotStateUpdateWithId(IFixture fixture, TKey idToSet, string ruleSet = Fakers.Populated);

  protected async Task GetAllAsync_ShouldReturnWholeCollection(int expectedCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, expectedCount);
    var measuredCount = 0;
    //Act
    await foreach (var item in sut.GetAllAsync())
    {
      //Assert
      item.IsFail.Should().BeFalse("because we expect valid data");
      measuredCount++;
    }
    //Assert
    measuredCount.Should().Be(expectedCount, "because whole collection should be returned");
    //TODO: Assert that Valid data was provided
  }

  protected async Task DeleteAllAsync_ShouldReturnTrueWhenDeletedAnything(int prefillCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, prefillCount);
    //Act
    var result = await sut.DeleteAllAsync();
    //Assert
    result.TryGetValue(out var deletedAny).Should().BeTrue();
    deletedAny.Should().Be(prefillCount > 0, "because should return true if operation was performed on any data");
  }

  [Test]
  [TestCase(10)]
  public async Task UpsertAsync_ShouldInsertNewEntry_WhenKeyIsDefault(int prefillCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, prefillCount);
    var dto = CreateRobotWithId(fixture, default!);
    var mockContext = fixture.Create<MockRobotContext>();
    var comparisonConfig = new ComparisonConfig();
    comparisonConfig.IgnoreProperty<TRobot>(x => x.Id);
    //Act
    var result = await sut.AddOrUpdateAsync(dto);
    //Assert
    result.TryGetValue(out var insertedNew).Should().BeTrue("because insertion should succed");
    (await mockContext.Robots.CountAsync()).Should().Be(prefillCount + 1, "because we inserted new entry");
    insertedNew.ShouldCompare(dto, "because data should be persisted", comparisonConfig);
  }

  protected async Task UpsertAsync_ShouldFail_WhenKeyIsMissing(TKey key, int prefillCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, prefillCount);
    var dto = CreateRobotWithId(fixture, key);
    var mockContext = fixture.Create<MockRobotContext>();
    //Act
    var result = await sut.AddOrUpdateAsync(dto);
    //Assert
    result.IsFail.Should().BeTrue("because row with that key is missing");
    (await mockContext.Robots.CountAsync()).Should().Be(prefillCount, "because nothing happened");
  }

  protected async Task UpsertAsync_ShouldUpdate_WhenKeyIsPresent(TKey key, int prefillCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, prefillCount);
    var dto = CreateRobotWithId(fixture, key);
    var mockContext = fixture.Create<MockRobotContext>();
    //Act
    var result = await sut.AddOrUpdateAsync(dto);
    //Assert
    result.TryGetValue(out var updated).Should().BeTrue("because row with that key exists");
    (await mockContext.Robots.CountAsync()).Should().Be(prefillCount, "because it was just updated");
    updated.ShouldCompare(dto, "because data should be persisted");
  }

  [Test]
  [TestCase(10)]
  public Task StateUpdateAsync_ShouldFail_WhenKeyIsDefault(int count)
    => StateUpdateAsync_ShouldFail_WhenKeyIsMissing(default!, count);

  protected async Task StateUpdateAsync_ShouldFail_WhenKeyIsMissing(TKey key, int prefillCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, prefillCount);
    var updateRequest = CreateRobotStateUpdateWithId(fixture, key);
    var mockContext = fixture.Create<MockRobotContext>();
    //Act
    var result = await sut.StateUpdateAsync(updateRequest);
    //Assert
    result.IsFail.Should().BeTrue("because row with that key is missing");
    (await mockContext.Robots.CountAsync()).Should().Be(prefillCount, "because nothing happened");
  }

  protected async Task StateUpdateAsync_ShouldUpdate_WhenKeyIsPresent(TKey key, int prefillCount)
  {
    //Arrage
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);
    await FillContextWithData(fixture, prefillCount);
    var mockContext = fixture.Create<MockRobotContext>();
    var updateRequest = CreateRobotStateUpdateWithId(fixture, key);
    var oldRobot = await FromContextGetRobotWithId(fixture, key);
    //Act
    var result = await sut.StateUpdateAsync(updateRequest);
    //Assert
    var newRobot = await FromContextGetRobotWithId(fixture, key);

    result.TryGetValue(out var updatedRobot).Should().BeTrue("because row with that key exists");
    (await mockContext.Robots.CountAsync()).Should().Be(prefillCount, "because it was just updated");

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

    updatedRobot.ChargeLevel.Should().Be(float.Clamp(updateRequest.ChargeLevel, float.NegativeInfinity, 1));
    newRobot.ChargeLevel.Should().Be(float.Clamp(updateRequest.ChargeLevel, float.NegativeInfinity, 1));
  }
}