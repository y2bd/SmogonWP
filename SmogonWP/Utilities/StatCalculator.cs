using System;

namespace SmogonWP.Utilities
{
  public static class StatCalculator
  {
    public static int CalculateHP(int level, int baseHP, int ev, int iv)
    {
      var top = (iv + baseHP * 2.0 + ev * 0.25 + 100) * level;

      return (int)Math.Floor(top / 100.0 + 10);
    }

    public static int CalculateOtherStat(int level, int baseStat, int ev, int iv, double natureMultiplier)
    {
      var top = (iv + baseStat * 2.0 + ev * 0.25) * level;

      var inner = (int)Math.Floor(top / 100.0 + 5);

      var result = (int)Math.Floor(inner * natureMultiplier);

      return result;
    }
  }
}
