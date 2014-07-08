using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SchmogonDB.Converters;
using SchmogonDB.Model;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;

namespace SchmogonDB
{
  internal class Loader
  {
    private const string PokeSearchPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/pokemon.xy.json";
    private const string MoveSearchPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/moves.xy.json";
    private const string AbilSearchPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/abilities.xy.json";
    private const string ItemSearchPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/items.xy.json";

    private const string PokeDataPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/pokedata.xy.json";
    private const string MoveDataPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/movedata.xy.json";
    private const string AbilDataPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/abildata.xy.json";
    private const string ItemDataPath = "https://raw.githubusercontent.com/y2bd/SmogonWPData/master/itemdata.xy.json";

    public async Task<LoaderData> LoadAllData()
    {
      var handler = new HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip
      };

      string pokesres, movesres, abilsres, itemsres;
      string pokedres, movedres, abildres, itemdres;

      using (var client = new HttpClient(handler, true))
      {
        pokesres = await client.GetStringAsync(PokeSearchPath);
        movesres = await client.GetStringAsync(MoveSearchPath);
        abilsres = await client.GetStringAsync(AbilSearchPath);
        itemsres = await client.GetStringAsync(ItemSearchPath);

        pokedres = await client.GetStringAsync(PokeDataPath);
        movedres = await client.GetStringAsync(MoveDataPath);
        abildres = await client.GetStringAsync(AbilDataPath);
        itemdres = await client.GetStringAsync(ItemDataPath);
      }
      
      return new LoaderData
      {
        PokemonSearch = await DeserializeDataListAsync<Pokemon>(pokesres),
        MovesSearch = await DeserializeDataListAsync<Move>(movesres),
        AbilitiesSearch = await DeserializeDataListAsync<Ability>(abilsres),
        ItemsSearch = await DeserializeDataListAsync<Item>(itemsres),
        Pokemon = await DeserializeDataListAsync<PokemonData>(pokedres),
        Abilities = await DeserializeDataListAsync<AbilityData>(abildres),
        Moves = await DeserializeDataListAsync<MoveData>(movedres),
        Items = await DeserializeDataListAsync<ItemData>(itemdres),
      };
    }

    private async Task<IEnumerable<T>> DeserializeDataListAsync<T>(string serialized)
    {
      var settings = new JsonSerializerSettings
      {
        Converters = new List<JsonConverter>
        {
          new TextElementConverter()
        }
      };

      Debug.WriteLine("Deserializing one!");
      return await JsonConvert.DeserializeObjectAsync<IEnumerable<T>>(serialized, settings);
    }
    
    internal class LoaderData
    {
      public IEnumerable<Pokemon> PokemonSearch { get; set; }
      public IEnumerable<Move> MovesSearch { get; set; }
      public IEnumerable<Ability> AbilitiesSearch { get; set; }
      public IEnumerable<Item> ItemsSearch { get; set; } 

      public IEnumerable<PokemonData> Pokemon { get; set; }
      public IEnumerable<MoveData> Moves { get; set; }
      public IEnumerable<AbilityData> Abilities { get; set; }
      public IEnumerable<ItemData> Items { get; set; } 
    }
  }
}
