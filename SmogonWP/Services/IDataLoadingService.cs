using SchmogonDB.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;

namespace SmogonWP.Services
{
  public interface IDataLoadingService
  {
    Task<IEnumerable<Pokemon>> FetchAllPokemonAsync();
    Task<IEnumerable<Move>> FetchAllMovesAsync();
    Task<IEnumerable<Ability>> FetchAllAbilitiesAsync();
    Task<IEnumerable<Item>> FetchAllItemsAsync();

    Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon);
    Task<MoveData> FetchMoveDataAsync(Move move);
    Task<AbilityData> FetchAbilityDataAsync(Ability ability);
    Task<ItemData> FetchItemDataAsync(Item item);
  }
}