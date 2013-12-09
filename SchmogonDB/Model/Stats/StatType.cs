using System.Collections.Generic;

namespace SchmogonDB.Model.Stats
{
  public enum StatType
  {
    Attack,
    Defense,
    SpecialAttack,
    SpecialDefense,
    Speed,
    HP
  }

  public static class StatUtils
  {
    private static readonly IDictionary<StatType, string> StatNames = new Dictionary<StatType, string>
    {
      {StatType.Attack, "Attack"},
      {StatType.Defense, "Defense"},
      {StatType.HP, "HP"},
      {StatType.SpecialAttack, "Special Attack"},
      {StatType.SpecialDefense, "Special Defense"},
      {StatType.Speed, "Speed"}
    };

    private static readonly IDictionary<StatType, string> ShortNames = new Dictionary<StatType, string>
    {
      {StatType.Attack, "Atk"},
      {StatType.Defense, "Def"},
      {StatType.HP, "HP"},
      {StatType.SpecialAttack, "SpA"},
      {StatType.SpecialDefense, "SpD"},
      {StatType.Speed, "Spe"}
    };

    public static string GetName(StatType stat)
    {
      return StatNames[stat];
    }

    public static string GetShortName(StatType stat)
    {
      return ShortNames[stat];
    }
  }
}
