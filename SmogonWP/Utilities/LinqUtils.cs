using System;
using System.Collections.Generic;

namespace SmogonWP.Utilities
{
  public static class LinqUtils
  {
    public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
    {
      var i = 0;
      foreach (var e in ie) action(e, i++);
    }

    public static void Each<T>(this IEnumerable<T> ie, Action<T> action)
    {
      foreach (var e in ie) action(e);
    }
  }
}
