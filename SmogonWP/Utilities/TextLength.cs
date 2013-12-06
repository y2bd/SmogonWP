using System.Diagnostics;
using System.Linq;

namespace SmogonWP.Utilities
{
  public static class TextLength
  {
    private const string FatChars = "mogw";
    private const string WideChars = "abcdehknpqsuvxyz ";
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
  }
}
