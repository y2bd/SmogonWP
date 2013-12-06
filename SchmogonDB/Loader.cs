using System.Collections.Generic;
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
    private const string PokeSearchPath = "http://162.243.147.104/schmogon/pokemon.json";
    private const string MoveSearchPath = "http://162.243.147.104/schmogon/moves.json";
    private const string AbilSearchPath = "http://162.243.147.104/schmogon/abilities.json";
    private const string ItemSearchPath = "http://162.243.147.104/schmogon/items.json";

    private const string PokeDataPath = "http://162.243.147.104/schmogon/pokedata.json";
    private const string MoveDataPath = "http://162.243.147.104/schmogon/movedata.json";
    private const string AbilDataPath = "http://162.243.147.104/schmogon/abildata.json";
    private const string ItemDataPath = "http://162.243.147.104/schmogon/itemdata.json";

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
