using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Schmogon;
using Schmogon.Data;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using Type = Schmogon.Data.Types.Type;

namespace SchmogonTest
{
  class Program
  {
    private const string MoveFile = "moves.json";
    private const string PokeFile = "pokemon.json";
    private const string AbilFile = "abilities.json";
    private const string ItemFile = "items.json";

    private const string MoveDataFile = "movedata.json";
    private const string PokeDataFile = "pokedata.json";
    private const string AbilDataFile = "abildata.json";
    private const string ItemDataFile = "itemdata.json";

    private const string Root = @"C:\Users\Jason\Documents\schmogon_dump\";

    static void Main(string[] args)
    {
      Task.WaitAll(test());
    }

    static async Task test()
    {
      //await serializeData();

      await deserializePokemon();
    }

    private static async Task deserializePokemon()
    {
      var clock = new Stopwatch();

      clock.Reset();
      clock.Start();

      var s = new SchmogonClient();

      var handler = new HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
      };

      using (var client = new HttpClient(handler, true))
      {
        var result = await client.GetStringAsync("http://162.243.147.104/schmogon/pokedata.json");
        
        var pokes = (await s.DeserializeDataListAsync<PokemonData>(result)).ToList();

        result = await client.GetStringAsync("http://162.243.147.104/schmogon/movedata.json");

        var moves = (await s.DeserializeDataListAsync<MoveData>(result));

        result = await client.GetStringAsync("http://162.243.147.104/schmogon/abildata.json");

        var abils = (await s.DeserializeDataListAsync<AbilityData>(result));

        result = await client.GetStringAsync("http://162.243.147.104/schmogon/itemdata.json");

        var items = (await s.DeserializeDataListAsync<ItemData>(result));
      }
    }

    private static async Task serializeData()
    {
      var s = new SchmogonClient();

      var abils = (await s.GetAllAbilitiesAsync()).ToList();
      Console.WriteLine("Loaded abilities");

      var pokes = (await s.GetAllPokemonAsync()).ToList();
      Console.WriteLine("Loaded pokemon");

      var items = (await s.GetAllItemsAsync()).ToList();
      Console.WriteLine("Loaded items");

      var typedMoves = Enum.GetValues(typeof (Type)).Cast<Type>()
        .Where(t => t != Type.Fairy)
        .Select(async t => new {Type = t, Moves = await s.GetMovesOfTypeAsync(t)})
        .Select(a => a.Result)
        .SelectMany(a => a.Moves.Select(m => new TypedMove(m.Name, m.Description, m.PageLocation, a.Type)))
        .ToList();
      Console.WriteLine("Loaded moves");

      var searchJobs = (new List<IEnumerable<ISearchItem>> {typedMoves, abils, pokes, items})
        .Zip(new[] {MoveFile, AbilFile, PokeFile, ItemFile}, (enumerable, s1) => new {Name = s1, Data = enumerable});

      Console.WriteLine("Zipped search jobs");

      foreach (var job in searchJobs)
      {
        using (var sw = new StreamWriter(Root + job.Name, false, Encoding.UTF8))
        {
          var cereal = await s.SerializeDataListAsync(job.Data);
          sw.Write(cereal);
        }

        Console.WriteLine("Finished {0}", job.Name);
      }

      Console.WriteLine("Finished search jobs");

      var itemData = items.Select(async i => await s.GetItemDataAsync(i))
        .Select(t => t.Result).ToList();
      Console.WriteLine("Loaded all item data");

      var abilData = abils.Select(async a => await s.GetAbilityDataAsync(a))
        .Select(t => t.Result).ToList();
      Console.WriteLine("Loaded all ability data");

      var pokeData = pokes.Select(async p =>
      {
        PokemonData pd = null;

        try
        {
          pd = await s.GetPokemonDataAsync(p);
        }
        catch (Exception e)
        {
          Debugger.Break();
        }

        return pd;
      })
        .Select(t => t.Result).ToList();
      Console.WriteLine("Loaded all ability data");

      var moveData = typedMoves.Select(async m => await s.GetMoveDataAsync(m))
        .Select(t => t.Result).ToList();
      Console.WriteLine("Loaded all ability data");

      var dataJobs = (new List<IEnumerable<IDataItem>> {itemData, abilData, pokeData, moveData})
        .Zip(new[] {ItemDataFile, AbilDataFile, PokeDataFile, MoveDataFile},
          (enumerable, s1) => new {Name = s1, Data = enumerable});

      Console.WriteLine("Zipped data jobs");

      foreach (var job in dataJobs)
      {
        using (var sw = new StreamWriter(Root + job.Name, false, Encoding.UTF8))
        {
          var cereal = await s.SerializeDataListAsync(job.Data);
          sw.Write(cereal);
        }
      }

      Console.WriteLine("Finished data jobs");

      Console.Write("Press any key to continue.");
      Console.ReadKey();
    }
  }
}
