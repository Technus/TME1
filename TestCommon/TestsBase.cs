using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NUnit.Framework;

namespace TME1.Tests;
/// <summary>
/// Base test class to ensure some common testing practices
/// </summary>
/// <typeparam name="TSUT"></typeparam>
public abstract class TestsBase<TSUT>
{
  /// <summary>
  /// Ideally should only call constructor
  /// </summary>
  /// <param name="fixture"></param>
  /// <returns></returns>
  protected abstract TSUT CreateSUT(IFixture fixture);

  /// <summary>
  /// Creates fixture for test using <see cref="AutoNSubstituteCustomization"/> 
  /// with <see cref="AutoNSubstituteCustomization.ConfigureMembers"/> set to <see langword="true"/>
  /// </summary>
  /// <returns></returns>
  protected IFixture CreateFixture() => new Fixture()
    .Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });

  [Test]
  public void CreateSUT_ShouldInstantiateCorrectType()
  {
    //Arrange
    var fixture = CreateFixture();
    //Act
    var action = () => CreateSUT(fixture);
    //Assert
    action.Should()
      .NotThrow("because it must be possible to create it for testing")
      .Which.Should()
      .NotBeNull("because to run tests we need an instance")
      .And
      .BeOfType<TSUT>("because testing needs to be done on the defined type and not an inheritor");
  }

  [Test]
  public void CreateSUT_ShouldAlwaysMakeNewInstance()
  {
    //Arrange
    var fixture = CreateFixture();
    //Act
    var sut1 = CreateSUT(fixture);
    var sut2 = CreateSUT(fixture);
    //Assert
    sut1.Should().NotBeNull();
    sut2.Should().NotBeNull();

    sut1.Should().NotBeSameAs(sut2, "because it is required that the tests are operating on their own objects");
  }
}
