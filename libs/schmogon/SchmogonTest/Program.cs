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

      return;
    }

    static async Task test()
    {
      var s = new SchmogonClient();

      Console.Write("Please enter a query: ");
      var query = Console.ReadLine();

      /*
      var moves = await s.SearchMovesAsync(query);
      var x = await s.GetMoveDataAsync(moves.ElementAt(0));
      */

      var abilties = await s.SearchAbilitiesAsync(query);

      var d = await s.GetAbilityDataAsync(abilties.First());

      var x = d;
    }
  }
}
