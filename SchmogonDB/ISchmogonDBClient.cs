using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;

namespace SchmogonDB
{
  public interface ISchmogonDBClient
  {
    Task<IEnumerable<Item>> FetchItemSearchDataAsync();

    Task<ItemData> FetchItemDataAsync(Item item);

    Task<IEnumerable<Move>> FetchMoveSearchDataAsync();

    Task<MoveData> FetchMoveDataAsync(Move move);

    Task<IEnumerable<Pokemon>> FetchPokemonSearchDataAsync();

    Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon);

    Task<IEnumerable<Ability>> FetchAbilitySearchDataAsync();

    Task<AbilityData> FetchAbilityDataAsync(Ability ability);

    Task InitializeDatabase();
  }
}