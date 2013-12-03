using Schmogon;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SchmogonDB.Model;

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

      var s = new SchmogonClient();

      return new LoaderData
      {
        PokemonSearch = await s.DeserializeDataListAsync<Pokemon>(pokesres),
        MovesSearch = await s.DeserializeDataListAsync<TypedMove>(movesres),
        AbilitiesSearch = await s.DeserializeDataListAsync<Ability>(abilsres),
        ItemsSearch = await s.DeserializeDataListAsync<Item>(itemsres),
        Pokemon = await s.DeserializeDataListAsync<PokemonData>(pokedres),
        Abilities = await s.DeserializeDataListAsync<AbilityData>(abildres),
        Moves = await s.DeserializeDataListAsync<MoveData>(movedres),
        Items = await s.DeserializeDataListAsync<ItemData>(itemdres),
      };
    }

    internal class LoaderData
    {
      public IEnumerable<Pokemon> PokemonSearch { get; set; }
      public IEnumerable<TypedMove> MovesSearch { get; set; }
      public IEnumerable<Ability> AbilitiesSearch { get; set; }
      public IEnumerable<Item> ItemsSearch { get; set; } 

      public IEnumerable<PokemonData> Pokemon { get; set; }
      public IEnumerable<MoveData> Moves { get; set; }
      public IEnumerable<AbilityData> Abilities { get; set; }
      public IEnumerable<ItemData> Items { get; set; } 
    }
  }
}
