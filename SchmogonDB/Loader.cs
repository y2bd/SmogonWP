using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Schmogon;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;

namespace SchmogonDB
{
  internal class Loader
  {
    private const string PokePath = "http://162.243.147.104/schmogon/pokedata.json";
    private const string MovePath = "http://162.243.147.104/schmogon/movedata.json";
    private const string AbilPath = "http://162.243.147.104/schmogon/abildata.json";
    private const string ItemPath = "http://162.243.147.104/schmogon/itemdata.json";

    public async Task<LoaderData> LoadAllData()
    {
      var handler = new HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip
      };

      string pokeres, moveres, abilres, itemres;

      using (var client = new HttpClient(handler, true))
      {
        pokeres = await client.GetStringAsync(PokePath);
        moveres = await client.GetStringAsync(MovePath);
        abilres = await client.GetStringAsync(AbilPath);
        itemres = await client.GetStringAsync(ItemPath);
      }

      var s = new SchmogonClient();

      return new LoaderData
      {
        Pokemon = await s.DeserializeDataListAsync<PokemonData>(pokeres),
        Abilities = await s.DeserializeDataListAsync<AbilityData>(abilres),
        Moves = await s.DeserializeDataListAsync<MoveData>(moveres),
        Items = await s.DeserializeDataListAsync<ItemData>(itemres),
      };
    }

    internal class LoaderData
    {
      public IEnumerable<PokemonData> Pokemon { get; set; }
      public IEnumerable<MoveData> Moves { get; set; }
      public IEnumerable<AbilityData> Abilities { get; set; }
      public IEnumerable<ItemData> Items { get; set; } 
    }
  }
}
