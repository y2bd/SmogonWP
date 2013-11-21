using System;
using System.Linq;
using System.Threading.Tasks;
using Schmogon;

namespace SchmogonTest
{
  class Program
  {
    static void Main(string[] args)
    {
      Task.WaitAll(test());
    }

    static async Task test()
    {
      var s = new SchmogonClient();

      Console.Write("Please enter a query: ");
      var query = Console.ReadLine();

      var move = (await s.GetAllMovesAsync()).First(m => query != null && m.Name.Contains(query));

      var data = await s.GetMoveDataAsync(move);
    }
  }
}
