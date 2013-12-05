using System;
using System.Diagnostics;
using Nito.AsyncEx;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using SchmogonDB;
using SchmogonDB.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmogonWP.Services
{
  public class DataLoadingService : IDataLoadingService
  {
    private readonly ISchmogonDBClient _schmogonDBClient;

    // we have different locks for each call
    // because we don't want a move request locking up a pokemon request
    // because those two requests will never mess each other up
    private readonly AsyncLock _pokeLock;
    private readonly AsyncLock _moveLock;
    private readonly AsyncLock _abilLock;
    private readonly AsyncLock _itemLock;

    private readonly AsyncLock _dbInitLock; 
    
    public DataLoadingService(ISchmogonDBClient schmogonDBClient)
    {
      _schmogonDBClient = schmogonDBClient;
        
      _pokeLock = new AsyncLock();
      _moveLock = new AsyncLock();
      _abilLock = new AsyncLock();
      _itemLock = new AsyncLock();

      _dbInitLock = new AsyncLock();
    }

    public async Task<IEnumerable<Pokemon>> FetchAllPokemonAsync()
    {
      return await fetchData(_pokeLock, _schmogonDBClient.FetchPokemonSearchDataAsync);
    }

    public async Task<IEnumerable<TypedMove>> FetchAllMovesAsync()
    {
      return await fetchData(_moveLock, _schmogonDBClient.FetchMoveSearchDataAsync);
    }

    public async Task<IEnumerable<Ability>> FetchAllAbilitiesAsync()
    {
      return await fetchData(_abilLock, _schmogonDBClient.FetchAbilitySearchDataAsync);
    }

    public async Task<IEnumerable<Item>> FetchAllItemsAsync()
    {
      return await fetchData(_itemLock, _schmogonDBClient.FetchItemSearchDataAsync);
    }

    public async Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon)
    {
      return await fetchData(_pokeLock, _schmogonDBClient.FetchPokemonDataAsync, pokemon);
    }

    public async Task<MoveData> FetchMoveDataAsync(TypedMove move)
    {
      return await fetchData(_moveLock, _schmogonDBClient.FetchMoveDataAsync, move);
    }

    public async Task<AbilityData> FetchAbilityDataAsync(Ability ability)
    {
      return await fetchData(_abilLock, _schmogonDBClient.FetchAbilityDataAsync, ability);
    }

    public async Task<ItemData> FetchItemDataAsync(Item item)
    {
      return await fetchData(_itemLock, _schmogonDBClient.FetchItemDataAsync, item);
    }

    private async Task<TR> fetchData<T, TR>(AsyncLock alock, Func<T, Task<TR>> fetchTask, T param)
    {
      Debug.WriteLine("Starting {0}", typeof(TR));

      using (await _dbInitLock.LockAsync())
      {
        Debug.WriteLine("Starting lock {0}", typeof(TR));
        await _schmogonDBClient.InitializeDatabase();
        Debug.WriteLine("Finished lock {0}", typeof(TR));
      }

      Debug.WriteLine("Finished init {0}", typeof(TR));

      // we use a lock so only one instance can be fetched at a time
      // wait for the last caller to get the list before the next one gets it
      // so the second is guaranteed the cache
      using (await alock.LockAsync())
      {
        return await fetchTask(param);
      }
    }

    private async Task<TR> fetchData<TR>(AsyncLock alock, Func<Task<TR>> fetchTask)
    {
      Debug.WriteLine("Starting {0}", typeof(TR));

      using (await _dbInitLock.LockAsync())
      {
        Debug.WriteLine("Starting lock {0}", typeof(TR));
        await _schmogonDBClient.InitializeDatabase();
        Debug.WriteLine("Finished lock {0}", typeof(TR));
      }

      Debug.WriteLine("Finished init {0}", typeof(TR));

      // we use a lock so only one instance can be fetched at a time
      // wait for the last caller to get the list before the next one gets it
      // so the second is guaranteed the cache
      using (await alock.LockAsync())
      {
        return await fetchTask();
      }
    }
  }
}