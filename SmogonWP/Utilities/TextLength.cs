using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmogonWP.Utilities
{
  public static class TextLength
  {
    private const string WideChars = "abcdeghkmnopqsuvwxyz ";
    private const string NarrowChars = "fijlrt-";

    public static int EstimateTextLength(string text)
    {
      var score = 0;

      foreach (var c in text.ToLower())
      {
        if (NarrowChars.Contains(c)) score += 2;
        else score += 4;
      }

      score += text.Length;

      Debug.WriteLine("{0}: {1}", text, score);

      return score;
    }
  }
}
