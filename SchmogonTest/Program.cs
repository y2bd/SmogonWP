using System.Threading.Tasks;
using Schmogon;

namespace SchmogonTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var s = new SchmogonClient();

      Task.WaitAll(s.SearchMovesAsync("fire"));

      return;
    }
  }
}
