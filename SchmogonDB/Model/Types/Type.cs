using System.Collections.Generic;

namespace SchmogonDB.Model.Types
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

  public class TypeOffenseEffect
  {
    internal static readonly IEnumerable<TypeOffenseEffect> TypeEffects = new List<TypeOffenseEffect>
    {
#region typedefs
      new TypeOffenseEffect
      {
              Type = Type.Fairy,
              SuperEffectiveAgainst = new List<Type>{Type.Fighting,Type.Dragon,Type.Dark},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fire,Type.Poison,Type.Steel,},
              NoEffectAgainst = new List<Type>()
      },
      new TypeOffenseEffect
      {
              Type = Type.Ghost,
              SuperEffectiveAgainst = new List<Type>{Type.Ghost,Type.Psychic},
              NotVeryEffectiveAgainst = new List<Type>{Type.Dark},
              NoEffectAgainst = new List<Type>{Type.Normal},
      },
      new TypeOffenseEffect
      {
              Type = Type.Steel,
              SuperEffectiveAgainst = new List<Type>{Type.Rock,Type.Ice,Type.Fairy},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Fire,Type.Water,Type.Electric},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Dark,
              SuperEffectiveAgainst = new List<Type>{Type.Ghost,Type.Psychic},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fighting,Type.Fairy,Type.Dark},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Electric,
              SuperEffectiveAgainst = new List<Type>{Type.Flying,Type.Water},
              NotVeryEffectiveAgainst = new List<Type>{Type.Grass,Type.Electric,Type.Dragon},
              NoEffectAgainst = new List<Type>{Type.Ground},
      },
      new TypeOffenseEffect
      {
              Type = Type.Ice,
              SuperEffectiveAgainst = new List<Type>{Type.Flying,Type.Ground,Type.Grass,Type.Dragon},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Fire,Type.Water,Type.Ice},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Normal,
              SuperEffectiveAgainst = new List<Type>(),
              NotVeryEffectiveAgainst = new List<Type>{Type.Rock,Type.Steel},
              NoEffectAgainst = new List<Type>{Type.Ghost},
      },
      new TypeOffenseEffect
      {
              Type = Type.Fire,
              SuperEffectiveAgainst = new List<Type>{Type.Bug,Type.Steel,Type.Grass,Type.Ice},
              NotVeryEffectiveAgainst = new List<Type>{Type.Rock,Type.Fire,Type.Water,Type.Dragon},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Psychic,
              SuperEffectiveAgainst = new List<Type>{Type.Fighting,Type.Poison},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel,Type.Psychic},
              NoEffectAgainst = new List<Type>{Type.Dark},
      },
      new TypeOffenseEffect
      {
              Type = Type.Flying,
              SuperEffectiveAgainst = new List<Type>{Type.Fighting,Type.Bug,Type.Grass},
              NotVeryEffectiveAgainst = new List<Type>{Type.Rock,Type.Steel,Type.Electric},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Poison,
              SuperEffectiveAgainst = new List<Type>{Type.Grass, Type.Fairy},
              NotVeryEffectiveAgainst = new List<Type>{Type.Poison,Type.Ground,Type.Rock,Type.Ghost},
              NoEffectAgainst = new List<Type>{Type.Steel},
      },
      new TypeOffenseEffect
      {
              Type = Type.Dragon,
              SuperEffectiveAgainst = new List<Type>{Type.Dragon},
              NotVeryEffectiveAgainst = new List<Type>{Type.Steel},
              NoEffectAgainst = new List<Type>{Type.Fairy},
      },
      new TypeOffenseEffect
      {
              Type = Type.Water,
              SuperEffectiveAgainst = new List<Type>{Type.Ground,Type.Rock,Type.Fire},
              NotVeryEffectiveAgainst = new List<Type>{Type.Water,Type.Grass,Type.Dragon},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Fighting,
              SuperEffectiveAgainst = new List<Type>{Type.Normal,Type.Rock,Type.Steel,Type.Ice,Type.Dark},
              NotVeryEffectiveAgainst = new List<Type>{Type.Flying,Type.Poison,Type.Bug,Type.Psychic,Type.Fairy},
              NoEffectAgainst = new List<Type>{Type.Ghost},
      },
      new TypeOffenseEffect
      {
              Type = Type.Rock,
              SuperEffectiveAgainst = new List<Type>{Type.Flying,Type.Bug,Type.Fire,Type.Ice},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fighting,Type.Ground,Type.Steel},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
      {
              Type = Type.Grass,
              SuperEffectiveAgainst = new List<Type>{Type.Ground,Type.Rock,Type.Water},
              NotVeryEffectiveAgainst = new List<Type>{Type.Flying,Type.Poison,Type.Bug,Type.Steel,Type.Fire,Type.Grass,Type.Dragon},
              NoEffectAgainst = new List<Type>()
      },
      new TypeOffenseEffect
      {
              Type = Type.Bug,
              SuperEffectiveAgainst = new List<Type>{Type.Grass,Type.Psychic,Type.Dark},
              NotVeryEffectiveAgainst = new List<Type>{Type.Fighting,Type.Flying,Type.Poison,Type.Ghost,Type.Steel,Type.Fire,Type.Fairy},
              NoEffectAgainst = new List<Type>(),
      },
      new TypeOffenseEffect
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

    private TypeOffenseEffect()
    {
    }
  }

  public class TypeDefenseEffect
  {
    public Type Type { get; private set; }

    /// <summary>
    /// This contains a list of Types such that attacking this type with any of those types results in a "not very effective",
    /// which means 50% damage
    /// </summary>
    public IEnumerable<Type> StrongDefenseAgainst { get; private set; }

    /// <summary>
    /// This contains a list of Types such that attacking this type with any of those types results in a "super effective",
    /// which means 200% damage
    /// </summary>
    public IEnumerable<Type> WeakDefenseAgainst { get; private set; }

    /// <summary>
    /// This contains a list of Types such that attacking this type with any of those types results in "no effect",
    /// which means 0% damage
    /// </summary>
    public IEnumerable<Type> FullDefenseAgainst { get; private set; }

    internal TypeDefenseEffect(Type type, IEnumerable<Type> strongDefenseAgainst, IEnumerable<Type> weakDefenseAgainst, IEnumerable<Type> fullDefenseAgainst)
    {
      Type = type;
      StrongDefenseAgainst = strongDefenseAgainst;
      WeakDefenseAgainst = weakDefenseAgainst;
      FullDefenseAgainst = fullDefenseAgainst;
    }
  }

  public class DualTypeDefenseEffect : TypeDefenseEffect
  {
    public Type SecondType { get; private set; }

    /// <summary>
    /// This contains a list of Types such that attacking this type pairing with any of those types results in a double "super effective",
    /// which means 400% damage
    /// </summary>
    public IEnumerable<Type> VeryWeakDefenseAgainst { get; private set; }

    /// <summary>
    /// This contains a list of Types such that attacking this type pairing with any of those types results in a double "not very effective",
    /// which means 25% damage
    /// </summary>
    public IEnumerable<Type> VeryStrongDefenseAgainst { get; private set; }

    internal DualTypeDefenseEffect(Type type, Type secondType, IEnumerable<Type> strongDefenseAgainst, IEnumerable<Type> weakDefenseAgainst, IEnumerable<Type> fullDefenseAgainst, IEnumerable<Type> veryStrongDefenseAgainst, IEnumerable<Type> veryWeakDefenseAgainst)
      : base(type, strongDefenseAgainst, weakDefenseAgainst, fullDefenseAgainst)
    {
      SecondType = secondType;
      VeryStrongDefenseAgainst = veryStrongDefenseAgainst;
      VeryWeakDefenseAgainst = veryWeakDefenseAgainst;
    }
  }
}