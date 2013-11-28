using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon;
using SmogonWP.Services;

namespace SmogonWP.Design
{
  public class DesignDataLoadingService : IDataLoadingService
  {
    private readonly ISchmogonClient _schmogonClient;

    public DesignDataLoadingService(ISchmogonClient schmogonClient)
    {
      _schmogonClient = schmogonClient;
    }

    public uint MaxTries
    {
      get;
      set;
    }

    public async Task<IEnumerable<Schmogon.Data.Pokemon.Pokemon>> FetchAllPokemonAsync()
    {
      return await _schmogonClient.GetAllPokemonAsync();
    }

    public async Task<IEnumerable<Schmogon.Data.Moves.Move>> FetchAllMovesAsync()
    {
      return await _schmogonClient.GetAllMovesAsync();
    }

    public async Task<IEnumerable<Schmogon.Data.Moves.Move>> FetchAllMovesOfTypeAsync(Schmogon.Data.Types.Type type)
    {
      return await _schmogonClient.GetMovesOfTypeAsync(type);
    }

    public async Task<IEnumerable<Schmogon.Data.Abilities.Ability>> FetchAllAbilitiesAsync()
    {
      return await _schmogonClient.GetAllAbilitiesAsync();
    }

    public async Task<IEnumerable<Schmogon.Data.Items.Item>> FetchAllItemsAsync()
    {
      return await _schmogonClient.GetAllItemsAsync();
    }
  }
}
