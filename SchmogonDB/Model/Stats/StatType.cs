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

    public static string GetStatName(StatType stat)
    {
      return StatNames[stat];
    }
  }
}
