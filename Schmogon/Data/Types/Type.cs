using System.Collections.Generic;

namespace Schmogon.Data.Types
{
  public enum Type
  {
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
  }

  public class TypeEffect
  {
    internal static readonly IEnumerable<TypeEffect> TypeEffects = new List<TypeEffect>
    {
#region typedefs
      new TypeEffect
      {
              Type = Type.Ghost,
              SuperEffectiveAgainst = new List<Type>{Type.Ghost,Type.Psychic},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Dark},
              NoEffectAgainst = new List<Type>{Type.Normal},
      },
      new TypeEffect
      {
              Type = Type.Steel,
              SuperEffectiveAgainst = new List<Type>{Type.Rock,Type.Ice},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Fire,Type.Water,Type.Electric},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Dark,
              SuperEffectiveAgainst = new List<Type>{Type.Ghost,Type.Psychic},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fighting,Type.Steel,Type.Dark},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Electric,
              SuperEffectiveAgainst = new List<Type>{Type.Flying,Type.Water},
              NotVeryEffectiveAgainst = new List<Type>{Type.Grass,Type.Electric,Type.Dragon},
              NoEffectAgainst = new List<Type>{Type.Ground},
      },
      new TypeEffect
      {
              Type = Type.Ice,
              SuperEffectiveAgainst = new List<Type>{Type.Flying,Type.Ground,Type.Grass,Type.Dragon},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Fire,Type.Water,Type.Ice},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Normal,
              SuperEffectiveAgainst = new List<Type>(),
              NotVeryEffectiveAgainst = new List<Type>{Type.Rock,Type.Steel},
              NoEffectAgainst = new List<Type>{Type.Ghost},
      },
      new TypeEffect
      {
              Type = Type.Fire,
              SuperEffectiveAgainst = new List<Type>{Type.Bug,Type.Steel,Type.Grass,Type.Ice},
              NotVeryEffectiveAgainst = new List<Type>{Type.Rock,Type.Fire,Type.Water,Type.Dragon},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Psychic,
              SuperEffectiveAgainst = new List<Type>{Type.Fighting,Type.Poison},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Psychic},
              NoEffectAgainst = new List<Type>{Type.Dark},
      },
      new TypeEffect
      {
              Type = Type.Flying,
              SuperEffectiveAgainst = new List<Type>{Type.Fighting,Type.Bug,Type.Grass},
              NotVeryEffectiveAgainst = new List<Type>{Type.Rock,Type.Steel,Type.Electric},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Poison,
              SuperEffectiveAgainst = new List<Type>{Type.Grass},
              NotVeryEffectiveAgainst = new List<Type>{Type.Poison,Type.Ground,Type.Rock,Type.Ghost},
              NoEffectAgainst = new List<Type>{Type.Steel},
      },
      new TypeEffect
      {
              Type = Type.Dragon,
              SuperEffectiveAgainst = new List<Type>{Type.Dragon},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Water,
              SuperEffectiveAgainst = new List<Type>{Type.Ground,Type.Rock,Type.Fire},
              NotVeryEffectiveAgainst = new List<Type>{Type.Water,Type.Grass,Type.Dragon},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Fighting,
              SuperEffectiveAgainst = new List<Type>{Type.Normal,Type.Rock,Type.Steel,Type.Ice,Type.Dark},
              NotVeryEffectiveAgainst = new List<Type>{Type.Flying,Type.Poison,Type.Bug,Type.Psychic},
              NoEffectAgainst = new List<Type>{Type.Ghost},
      },
      new TypeEffect
      {
              Type = Type.Rock,
              SuperEffectiveAgainst = new List<Type>{Type.Flying,Type.Bug,Type.Fire,Type.Ice},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fighting,Type.Ground,Type.Steel},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Grass,
              SuperEffectiveAgainst = new List<Type>{Type.Ground,Type.Rock,Type.Water},
              NotVeryEffectiveAgainst = new List<Type>{Type.Flying,Type.Poison,Type.Bug,Type.Steel,Type.Fire,Type.Grass,Type.Dragon},
      },
      new TypeEffect
      {
              Type = Type.Bug,
              SuperEffectiveAgainst = new List<Type>{Type.Grass,Type.Psychic,Type.Dark},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fighting,Type.Flying,Type.Poison,Type.Ghost,Type.Steel,Type.Fire},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeEffect
      {
              Type = Type.Ground,
              SuperEffectiveAgainst = new List<Type>{Type.Poison,Type.Rock,Type.Steel,Type.Fire,Type.Electric},
              NotVeryEffectiveAgainst = new List<Type>{Type.Bug,Type.Grass},
              NoEffectAgainst = new List<Type>{Type.Flying},
      },
#endregion typedefs
    };

    public Type Type { get; internal set; }
    public IEnumerable<Type> SuperEffectiveAgainst { get; internal set; }
    public IEnumerable<Type> NotVeryEffectiveAgainst { get; internal set; }
    public IEnumerable<Type> NoEffectAgainst { get; internal set; }

    private TypeEffect()
    {
    }
  }
}