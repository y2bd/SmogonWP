using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using SchmogonDB.Model;

namespace SchmogonDB
{
  public interface ISchmogonDBClient
  {
    Task<IEnumerable<Item>> FetchItemSearchDataAsync();

    Task<ItemData> FetchItemDataAsync(Item item);

    Task<IEnumerable<TypedMove>> FetchMoveSearchDataAsync();

    Task<MoveData> FetchMoveDataAsync(TypedMove move);

    Task<IEnumerable<Pokemon>> FetchPokemonSearchDataAsync();

    Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon);

    Task<IEnumerable<Ability>> FetchAbilitySearchDataAsync();

    Task<AbilityData> FetchAbilityDataAsync(Ability ability);

    Task InitializeDatabase();
  }
}