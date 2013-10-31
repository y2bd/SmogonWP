using System;
using System.Collections.Generic;

namespace Schmogon.Utilities
{
  public static class Extensions
  {
    ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
    ///<param name="items">The enumerable to search.</param>
    ///<param name="predicate">The expression to test the items against.</param>
    ///<returns>The index of the first matching item, or -1 if no items match.</returns>
    public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
      if (items == null) throw new ArgumentNullException("items");
      if (predicate == null) throw new ArgumentNullException("predicate");

      int retVal = 0;
      foreach (var item in items)
      {
        if (predicate(item)) return retVal;
        retVal++;
      }
      return -1;
    }

    public static bool IsBetween(this int num, int left, int right, bool inclusive = false)
    {
      if (inclusive)
      {
        return num >= left && num <= right;
      }

      return num > left && num < right;
    }
  }
}
