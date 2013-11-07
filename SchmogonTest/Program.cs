using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Schmogon;
using Type = Schmogon.Data.Types.Type;

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
      
      var moves = await s.GetMovesOfTypeAsync(Type.Psychic);

      foreach (var move in moves)
      {
        Console.WriteLine(move.Name);
      }
      
      /*
      var abilties = await s.SearchAbilitiesAsync(query);

      var d = await s.GetAbilityDataAsync(abilties.First());

      var x = d;*/
    }
  }
}
