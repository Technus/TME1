using API.Controllers;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TME1.ServerCore.DataTransferObjects;
using TME1.ServerCore.Repositories;
using TME1.ServerCore.Services;
using TME1.TestCommon;

namespace TME1.ServerTests.ServerAPI.Controllers;

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
  public async Task GetAllAsync_ShouldReturnAllElements(int storedCount)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var dataSamples = new List<Fin<RobotDto>>();
    for (var i = 0; i < storedCount; i++)
      dataSamples.Add(Fakers.RobotDto.Generate("populated"));

    var mockRepository = fixture.Create<IRobotRepository<int, RobotDto>>();
    mockRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(dataSamples.ToAsyncEnumerable());
    var recievedCount = 0;
    //Act
    await foreach (var item in sut.GetAllAsync())
    {
      recievedCount++;
      item.Should().NotBeNull();
    }
    //Assert
    recievedCount.Should().Be(storedCount, "because it should return all elements");
    //TODO: assert correct data was provided
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task GetAsync_ShouldReturnSelectedElement_OrReturnNotFound(bool shouldContain, int id)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var dataSamples = Fakers.RobotDto.Generate("populated");
    dataSamples.Id = id;

    var mockRepository = fixture.Create<IRobotRepository<int, RobotDto>>();
    if (shouldContain)
      mockRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(dataSamples);
    else
      mockRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
    //Act
    var result = await sut.GetAsync(id);
    //Assert
    if (shouldContain)
    {
      result.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.ShouldCompare(dataSamples, "because it should be the same content");
    }
    else
    {
      result.Result.Should().BeOfType<NotFoundResult>();
    }
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task StateUpdateAsync_ByDto_ShouldUpdateSelectedElement_OrReturnNotFound(bool shouldContain, int idToUpdate)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var dataSample = Fakers.RobotDto.Generate("populated");
    dataSample.Id = idToUpdate;
    var updateSample = Fakers.RobotStateUpdateDTO.Generate("populated");
    updateSample.Id = idToUpdate;

    var mockRepository = fixture.Create<IRobotRepository<int, RobotDto>>();
    if (shouldContain)
      mockRepository.StateUpdateAsync(updateSample, Arg.Any<CancellationToken>()).Returns(dataSample);//Not very true in real life but works in testing
    else
      mockRepository.StateUpdateAsync(updateSample, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
    //Act
    var result = await sut.StateUpdateAsync(updateSample);
    //Assert
    if (shouldContain)
    {
      result.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.ShouldCompare(dataSample, "because it should be the same content");
    }
    else
    {
      result.Result.Should().BeOfType<NotFoundResult>();
    }
  }

  [Test]
  [TestCase(true, 3)]
  [TestCase(false, 4)]
  public async Task StateUpdateAsync_ById_ShouldUpdateSelectedElement_OrReturnNotFound(bool shouldContain, int idToUpdate)
  {
    //Arrange
    var fixture = CreateFixture();
    var sut = CreateSUT(fixture);

    var dataSample = Fakers.RobotDto.Generate("populated");
    dataSample.Id = idToUpdate;
    var updateSample = Fakers.RobotStateUpdateDTO.Generate("populated");
    updateSample.Id = idToUpdate;

    var mockStateUpdateService = fixture.Create<IRobotStateService<int>>();
    var mockRepository = fixture.Create<IRobotRepository<int, RobotDto>>();

    if (shouldContain)
    {
      mockStateUpdateService.UpdateAsync(dataSample, Arg.Any<CancellationToken>()).Returns(updateSample);
      mockRepository.GetAsync(idToUpdate, Arg.Any<CancellationToken>()).Returns(dataSample);
      mockRepository.StateUpdateAsync(updateSample, Arg.Any<CancellationToken>()).Returns(dataSample);//Not very true in real life but works in testing
    }
    else
    {
      mockRepository.GetAsync(idToUpdate, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
      mockRepository.StateUpdateAsync(updateSample, Arg.Any<CancellationToken>()).Returns(Error.New("Missing"));
    }
    //Act
    var result = await sut.StateUpdateAsync(idToUpdate);
    //Assert
    if (shouldContain)
    {
      result.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.ShouldCompare(dataSample, "because it should be the same content");
    }
    else
    {
      result.Result.Should().BeOfType<NotFoundResult>();
    }
  }
}
