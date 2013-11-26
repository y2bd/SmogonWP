using Schmogon;
using Schmogon.Data;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.Services
{
  public class DataLoadingService
  {
    private const string PokemonListFilename = "pokemon.txt";
    private const string AbilityListFilename = "abilities.txt";
    private const string ItemListFilename = "items.txt";
    private const string MoveListFilename = "moves.txt";
    private const string TypedMoveListFilename = "movetype_{0}.txt";

    private readonly ISchmogonClient _schmogonClient;
    private readonly IsolatedStorageService _storageService;

    public uint MaxTries { get; set; }

    public DataLoadingService(ISchmogonClient schmogonClient, IsolatedStorageService storageService)
    {
      _schmogonClient = schmogonClient;
      _storageService = storageService;

      MaxTries = 2;
    }

    public async Task<IEnumerable<Pokemon>> FetchAllPokemonAsync()
    {
      return await fetchSearchItems(PokemonListFilename, _schmogonClient.GetAllPokemonAsync());
    }

    public async Task<IEnumerable<Move>> FetchAllMovesAsync()
    {
      return await fetchSearchItems(MoveListFilename, _schmogonClient.GetAllMovesAsync());
    }

    public async Task<IEnumerable<Move>> FetchAllMovesOfTypeAsync(Type type)
    {
      var name = Enum.GetName(typeof (Type), type);
      if (name == null) throw new ArgumentException(@"type must be in the range of the Type enum", "type");

      var filename = string.Format(TypedMoveListFilename, name.ToLower());

      return await fetchSearchItems(filename, _schmogonClient.GetMovesOfTypeAsync(type));
    }

    public async Task<IEnumerable<Ability>> FetchAllAbilitiesAsync()
    {
      return await fetchSearchItems(AbilityListFilename, _schmogonClient.GetAllAbilitiesAsync());
    }

    public async Task<IEnumerable<Item>> FetchAllItemsAsync()
    {
      return await fetchSearchItems(ItemListFilename, _schmogonClient.GetAllItemsAsync());
    }

    private async Task<IEnumerable<T>> fetchSearchItems<T>(
      string cacheLocation,
      Task<IEnumerable<T>> searchTask)
      where T : ISearchItem
    {
      // first just try to fetch from storage
      var searchItems = await fetchSearchItemsFromStorage<T>(cacheLocation);

      if (searchItems != null) return searchItems;

      int tries = 0;
      bool success = false;

      Exception finalException = null;

      while (!success && tries < MaxTries)
      {
        tries++;

        try
        {
          searchItems = await searchTask;
        }
        catch (HttpRequestException e)
        {
          // this is an exception we expect
          // store it for later and try it again
          finalException = e;
        }

        success = true;
      }

      // if we failed
      if (searchItems == null)
      {
        throw finalException != null ?
          new Exception("No search items could be found", finalException) :
          new InvalidOperationException("No search items could be found, and the reason is unknown.");
      }

      // cache the data
      await cacheSearchItemList(searchItems, cacheLocation);

      return searchItems;
    }

    private async Task cacheSearchItemList<T>(IEnumerable<T> searchItemList, string location)
      where T : ISearchItem
    {
      var cereal = await _schmogonClient.SerializeSearchItemListAsync(searchItemList);
      await _storageService.WriteStringToFileAsync(location, cereal);
    }

    private async Task<IEnumerable<T>> fetchSearchItemsFromStorage<T>(string location)
      where T : ISearchItem
    {
      IEnumerable<T> cache = null;

      if (await _storageService.FileExistsAsync(location))
      {
        var cereal = await _storageService.ReadStringFromFileAsync(location);
        cache = await _schmogonClient.DeserializeSearchItemListAsync<T>(cereal);
      }

      return cache;
    }
  }
}