using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using SchmogonDB.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmogonWP.Services
{
  public interface IDataLoadingService
  {
    Task<IEnumerable<Pokemon>> FetchAllPokemonAsync();
    Task<IEnumerable<TypedMove>> FetchAllMovesAsync();
    Task<IEnumerable<Ability>> FetchAllAbilitiesAsync();
    Task<IEnumerable<Item>> FetchAllItemsAsync();

    Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon);
    Task<MoveData> FetchMoveDataAsync(TypedMove move);
    Task<AbilityData> FetchAbilityDataAsync(Ability ability);
    Task<ItemData> FetchItemDataAsync(Item item);
  }
}