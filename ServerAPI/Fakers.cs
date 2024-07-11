using Bogus;
using TME1.Abstractions.Enumerations;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.ServerAPI;
/// <summary>
/// Mock Data generators
/// </summary>
public static class Fakers
{
  /// <summary>
  /// Creates fake <see cref="ServerCore.DataTransferObjects.RobotDto"/> with possible configurations:<br/>
  /// populated - where most fields are populated<br/>
  /// minimal - where only name and mode is populated<br/>
  /// </summary>
  public static Faker<RobotDto> RobotDto { get; } = new Faker<RobotDto>()
      .CustomInstantiator(f =>
      {
        var model = f.PickRandom("Chiron", "Robot", "Robotee", "Robotor");
        return new RobotDto($"{model} {f.IndexGlobal}", model);
      })
      .RuleFor(o => o.ChargeLevel, f => f.Random.Bool(0.9f) ? f.Random.Float(0, 1) : -1)
      .RuleFor(o => o.Location, f => f.Address.City())
      .RuleFor(o => o.Position, f => f.PickRandom<RobotPosition>())
      .RuleFor(o => o.PositionMessage, f => f.Random.Bool(0.9f) ? f.Lorem.Sentence(2) : string.Empty)
      .RuleFor(o => o.Status, f => f.PickRandom<RobotStatus>())
      .RuleFor(o => o.StatusMessage, f => f.Random.Bool(0.9f) ? f.Lorem.Sentence(2) : string.Empty);
}
