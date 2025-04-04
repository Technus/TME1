﻿using Bogus;
using TME1.Abstractions.Enumerations;
using TME1.ServerCore.DataTransferObjects;

namespace TME1.ServerTests;
/// <summary>
/// Mock Data generators
/// </summary>
public static class Fakers
{
  public const string Populated = "populated";
  public const string Minimal = "minimal";

  /// <summary>
  /// Creates fake <see cref="ServerCore.DataTransferObjects.RobotDto"/> with possible configurations:<br/>
  /// populated - where most fields are populated<br/>
  /// minimal - where only name and mode is populated<br/>
  /// </summary>
  public static Faker<RobotDto> RobotDto { get; } = new Faker<RobotDto>()
    .RuleSet(Populated, ruleset => ruleset
      .CustomInstantiator(f =>
      {
        var model = f.PickRandom("Chiron", "Robot", "Robotee", "Robotor");
        return new RobotDto($"{model} {f.IndexGlobal}", model);
      })
      .RuleFor(o => o.ChargeLevel, f => f.Random.Bool(0.9f) ? f.Random.Float(0, 1) : -1)
      .RuleFor(o => o.Location, f => f.Address.City())
      .RuleFor(o => o.Position, f => f.PickRandom<RobotPosition>())
      .RuleFor(o => o.PositionMessage, f => f.Random.Bool(0.9f) ? f.Lorem.Sentence(4) : string.Empty)
      .RuleFor(o => o.Status, f => f.PickRandom<RobotStatus>())
      .RuleFor(o => o.StatusMessage, f => f.Random.Bool(0.9f) ? f.Lorem.Sentence(4) : string.Empty)
    )
    .RuleSet(Minimal, ruleset => ruleset
      .CustomInstantiator(f =>
      {
        var model = f.PickRandom("Chiron", "Robot", "Robotee", "Robotor");
        return new RobotDto($"{model} {f.IndexGlobal}", model);
      })
      .RuleFor(o => o.ChargeLevel, -1)
      .RuleFor(o => o.Location, string.Empty)
      .RuleFor(o => o.Position, RobotPosition.None)
      .RuleFor(o => o.PositionMessage, string.Empty)
      .RuleFor(o => o.Status, RobotStatus.None)
      .RuleFor(o => o.StatusMessage, string.Empty)
    );

  /// <summary>
  /// Creates fake <see cref="RobotStateUpdateDTO"/> with possible configurations:<br/>
  /// populated - where most fields are populated<br/>
  /// minimal - where only name and mode is populated<br/>
  /// </summary>
  public static Faker<RobotStateUpdateDto> RobotStateUpdateDTO { get; } = new Faker<RobotStateUpdateDto>()
    .RuleSet(Populated, ruleset => ruleset
      .CustomInstantiator(f => new RobotStateUpdateDto(f.IndexGlobal))
      .RuleFor(o => o.Status, f => f.PickRandom<RobotStatus>())
      .RuleFor(o => o.StatusMessage, f => f.Random.Bool(0.9f) ? f.Lorem.Sentence(4) : string.Empty)
      .RuleFor(o => o.ChargeLevel, f => f.Random.Bool(0.9f) ? f.Random.Float(-1, 1) : 0)
    )
    .RuleSet(Minimal, ruleset => ruleset
      .CustomInstantiator(f => new RobotStateUpdateDto(f.IndexGlobal))
      .RuleFor(o => o.Status, RobotStatus.None)
      .RuleFor(o => o.StatusMessage, string.Empty)
      .RuleFor(o => o.ChargeLevel, 0)
    );
}
