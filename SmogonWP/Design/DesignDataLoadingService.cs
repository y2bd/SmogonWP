﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using SchmogonDB;
using SchmogonDB.Model;
using SmogonWP.Services;

namespace SmogonWP.Design
{
  public class DesignDataLoadingService : IDataLoadingService
  {
    private readonly ISchmogonDBClient _schmogonClient;

    public DesignDataLoadingService(ISchmogonDBClient schmogonClient)
    {
      _schmogonClient = schmogonClient;
    }
    
    public async Task<IEnumerable<Pokemon>> FetchAllPokemonAsync()
    {
      return await _schmogonClient.FetchPokemonSearchDataAsync();
    }

    public async Task<IEnumerable<TypedMove>> FetchAllMovesAsync()
    {
      return await _schmogonClient.FetchMoveSearchDataAsync();
    }
    
    public async Task<IEnumerable<Ability>> FetchAllAbilitiesAsync()
    {
      return await _schmogonClient.FetchAbilitySearchDataAsync();
    }

    public async Task<IEnumerable<Item>> FetchAllItemsAsync()
    {
      return await _schmogonClient.FetchItemSearchDataAsync();
    }

    public async Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon)
    {
      return await _schmogonClient.FetchPokemonDataAsync(pokemon);
    }

    public async Task<MoveData> FetchMoveDataAsync(TypedMove move)
    {
      return await _schmogonClient.FetchMoveDataAsync(move);
    }

    public async Task<AbilityData> FetchAbilityDataAsync(Ability ability)
    {
      return await _schmogonClient.FetchAbilityDataAsync(ability);
    }

    public async Task<ItemData> FetchItemDataAsync(Item item)
    {
      return await _schmogonClient.FetchItemDataAsync(item);
    }
  }
}
