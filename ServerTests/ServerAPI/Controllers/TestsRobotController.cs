﻿using API.Controllers;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TME1.Abstractions.Repositories;
using TME1.Abstractions.Services;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.Tests.ServerAPI.Controllers;

/// <summary>
/// Actual tests on concrete implementation, does not really test the endpoints but the class logic.
/// </summary>
public class RobotControllerTests : TestsBase<RobotController>
{
  protected override RobotController CreateSUT(IFixture fixture)
  {
    fixture.Inject(Substitute.For<ILogger<RobotController>>());
    fixture.Inject(Substitute.For<IRobotRepository<int, RobotDto>>());
    fixture.Inject(Substitute.For<IRobotStateService<int>>());

    return new RobotController(
      fixture.Create<ILogger<RobotController>>(),
      fixture.Create<IRobotRepository<int, RobotDto>>(),
      fixture.Create<IRobotStateService<int>>());
  }

  [Test]
  [TestCase(0)]
  [TestCase(1)]
  [TestCase(10)]
  public async Task GetAllAsync_ShouldReturnAllElements(int count)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var listOfSamples = new List<Fin<RobotDto>>();
    for (var i = 0; i < count; i++)
      listOfSamples.Add(Fakers.RobotDto.Generate("populated"));

    var source = fixture.Create<IRobotRepository<int, RobotDto>>();
    source.GetAllAsync(Arg.Any<CancellationToken>()).Returns(listOfSamples.ToAsyncEnumerable());
    var recievedCount = 0;
    //Act
    await foreach (var item in sut.GetAllAsync())
    {
      recievedCount++;
      item.Should().NotBeNull();
    }
    //Assert
    recievedCount.Should().Be(count, "because it should return all elements");
    //TODO: assert correct data was provided
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task GetAsync_ShouldReturnSelectedElement_OrReturnNotFound(bool contains, int id)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var sample = Fakers.RobotDto.Generate("populated");
    sample.Id = id;

    var source = fixture.Create<IRobotRepository<int, RobotDto>>();
    if (contains)
      source.GetAsync(id, Arg.Any<CancellationToken>()).Returns(sample);
    else
      source.GetAsync(id, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
    //Act
    var result = await sut.GetAsync(id);
    //Assert
    if (contains)
    {
      result.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.ShouldCompare(sample, "because it should be the same content");
    }
    else
    {
      result.Result.Should().BeOfType<NotFoundResult>();
    }
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task StateUpdateAsync_ByDto_ShouldUpdateSelectedElement_OrReturnNotFound(bool contains, int id)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var sample = Fakers.RobotDto.Generate("populated");
    sample.Id = id;
    var update = Fakers.RobotStateUpdateDTO.Generate("populated");
    update.Id = id;

    var source = fixture.Create<IRobotRepository<int, RobotDto>>();
    if (contains)
      source.StateUpdateAsync(update, Arg.Any<CancellationToken>()).Returns(sample);//Not very true in real life but works in testing
    else
      source.StateUpdateAsync(update, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
    //Act
    var result = await sut.StateUpdateAsync(update);
    //Assert
    if (contains)
    {
      result.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.ShouldCompare(sample, "because it should be the same content");
    }
    else
    {
      result.Result.Should().BeOfType<NotFoundResult>();
    }
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task StateUpdateAsync_ById_ShouldUpdateSelectedElement_OrReturnNotFound(bool contains, int idToUpdate)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var sample = Fakers.RobotDto.Generate("populated");
    sample.Id = idToUpdate;
    var update = Fakers.RobotStateUpdateDTO.Generate("populated");
    update.Id = idToUpdate;

    var stateUpdateService = fixture.Create<IRobotStateService<int>>();
    var source = fixture.Create<IRobotRepository<int, RobotDto>>();

    if (contains)
    {
      stateUpdateService.UpdateAsync(sample, Arg.Any<CancellationToken>()).Returns(update);
      source.GetAsync(idToUpdate, Arg.Any<CancellationToken>()).Returns(sample);
      source.StateUpdateAsync(update, Arg.Any<CancellationToken>()).Returns(sample);//Not very true in real life but works in testing
    }
    else
    {
      source.GetAsync(idToUpdate, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
      source.StateUpdateAsync(update, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
    }
    //Act
    var result = await sut.StateUpdateAsync(idToUpdate);
    //Assert
    if (contains)
    {
      result.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.ShouldCompare(sample, "because it should be the same content");
    }
    else
    {
      result.Result.Should().BeOfType<NotFoundResult>();
    }
  }
}