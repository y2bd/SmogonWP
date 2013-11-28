using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Types;

namespace SmogonWP.Services
{
  public interface IDataLoadingService
  {
    uint MaxTries { set; }
    Task<IEnumerable<Pokemon>> FetchAllPokemonAsync();
    Task<IEnumerable<Move>> FetchAllMovesAsync();
    Task<IEnumerable<Move>> FetchAllMovesOfTypeAsync(Type type);
    Task<IEnumerable<Ability>> FetchAllAbilitiesAsync();
    Task<IEnumerable<Item>> FetchAllItemsAsync();
  }
}