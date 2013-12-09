using System;

namespace SmogonWP.Utilities
{
  public static class StatCalculator
  {
    public static readonly int MaxHP = CalculateHP(100, 255, 252, 31);
    public static readonly int MaxAttack = CalculateOtherStat(100, 180, 252, 31, 1.1);
    public static readonly int MaxDefense = CalculateOtherStat(100, 230, 252, 31, 1.1);
    public static readonly int MaxSpecialAttack = CalculateOtherStat(100, 180, 252, 31, 1.1);
    public static readonly int MaxSpecialDefense = CalculateOtherStat(100, 230, 252, 31, 1.1);
    public static readonly int MaxSpeed = CalculateOtherStat(100, 180, 252, 31, 1.1);

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
  }
}
