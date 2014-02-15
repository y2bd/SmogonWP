using System;

namespace SmogonWP.Utilities
{
  public static class StatCalculator
  {
    public static readonly int MaxHP = CalculateMaxHPAtLevel(100);
    public static readonly int MaxAttack = CalculateMaxAttackAtLevel(100);
    public static readonly int MaxDefense = CalculateMaxDefenseAtLevel(100);
    public static readonly int MaxSpecialAttack = CalculateMaxSpecialAttackAtLevel(100);
    public static readonly int MaxSpecialDefense = CalculateMaxSpecialDefenseAtLevel(100);
    public static readonly int MaxSpeed = CalculateMaxSpeedAtLevel(100);

    public static int CalculateHP(int level, int baseHP, int ev, int iv)
    {
      var top = (iv + baseHP * 2.0 + ev * 0.25 + 100) * level;

      return (int)Math.Floor(top / 100.0 + 10);
    }

    public static int CalculateOtherStat(int level, int baseStat, int ev, int iv, double natureMultiplier)
    {
      var top = (iv + baseStat * 2.0 + ev / 4.0) * level;

      var inner = (int)Math.Floor(top / 100.0 + 5);

      var result = (int)Math.Floor(inner * natureMultiplier);

      return result;
    }

    public static int CalculateMaxHPAtLevel(int level)
    {
      return CalculateHP(level, 255, 252, 31);
    }

    public static int CalculateMaxAttackAtLevel(int level)
    {
      return CalculateOtherStat(level, 180, 252, 31, 1.1);
    }

    public static int CalculateMaxDefenseAtLevel(int level)
    {
      return CalculateOtherStat(level, 230, 252, 31, 1.1);
    }

    public static int CalculateMaxSpecialAttackAtLevel(int level)
    {
      return CalculateOtherStat(level, 180, 252, 31, 1.1);
    }

    public static int CalculateMaxSpecialDefenseAtLevel(int level)
    {
      return CalculateOtherStat(level, 230, 252, 31, 1.1);
    }

    public static int CalculateMaxSpeedAtLevel(int level)
    {
      return CalculateOtherStat(level, 180, 252, 31, 1.1);
    }
  }
}
