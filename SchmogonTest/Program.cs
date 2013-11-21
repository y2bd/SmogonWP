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

      var item = (await s.GetAllItemsAsync()).FirstOrDefault(i => i.Name == query);

      var data = (await s.GetItemDataAsync(item));

      var cereal = await s.SerializeItemListAsync();

      var uncereal = await s.DeserializeItemListAsync(cereal);
    }
  }
}
