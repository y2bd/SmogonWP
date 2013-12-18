using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SmogonWP.Utilities
{
  public static class TextUtils
  {
    private const string FatChars = "mgw";
    private const string WideChars = "abcdehknppqsuvxyz ";
    private const string NarrowChars = "fijlrt-";

    public static int EstimateTextLength(string text)
    {
      var score = 0;

      foreach (var c in text.ToLower())
      {
        if (NarrowChars.Contains(c)) score += 2;
        else if (FatChars.Contains(c)) score += 8;
        else score += 4;
      }

      score += text.Length;

      Debug.WriteLine("{0}: {1}", text, score);

      return score;
    }

    public static string ToTitleCase(string word)
    {
      IEnumerable<string> split = word.Split(' ').ToList();
      split = split.Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower());

      return string.Join(" ", split);
    }
  }
}
