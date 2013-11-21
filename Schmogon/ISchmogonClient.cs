using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Stats;
using Schmogon.Data.Types;

namespace Schmogon
{
  public interface ISchmogonClient
  {
    #region moves

    Task<IEnumerable<Move>> GetAllMovesAsync();

    Task<IEnumerable<Move>> GetMovesOfTypeAsync(Type type);

    Task<IEnumerable<Move>> SearchMovesAsync(string query);

    Task<IEnumerable<Move>> SearchMovesOfTypeAsync(Type type, string query);

    Task<MoveData> GetMoveDataAsync(Move move);

    Task<string> SerializeMoveListAsync();

    Task<string> SerializeMoveListAsync(Type type);

    Task<IEnumerable<Move>> DeserializeMoveListAsync(string moves);

    Task<IEnumerable<Move>> DeserializeMoveListAsync(Type type, string moves);
    
    #endregion moves

    #region abilities

    Task<IEnumerable<Ability>> GetAllAbilitiesAsync();

    Task<IEnumerable<Ability>> SearchAbilitiesAsync(string query);

    Task<AbilityData> GetAbilityDataAsync(Ability ability);

    Task<string> SerializeAbilityListAsync();

    Task<IEnumerable<Ability>> DeserializeAbilityListAsync(string abilities); 

    #endregion abilities

    #region items

    Task<IEnumerable<Item>> GetAllItemsAsync();

    Task<IEnumerable<Item>> SearchItemsAsync(string query);

    Task<ItemData> GetItemDataAsync(Item item);

    Task<string> SerializeItemListAsync();

    Task<IEnumerable<Item>> DeserializeItemListAsync(string items); 

    #endregion items

    #region natures

    IEnumerable<NatureEffect> GetAllNatureEffects();

    NatureEffect GetNatureEffect(Nature nature);

    IEnumerable<NatureEffect> GetNatureEffectsWhere(StatType increased, StatType decreased);

    IEnumerable<NatureEffect> GetNatureEffectsWhereIncreased(StatType increased);

    IEnumerable<NatureEffect> GetNatureEffectsWhereDecreased(StatType decreased);

    #endregion natures

    #region types

    IEnumerable<TypeOffenseEffect> GetAllTypeOffenseEffects();

    IEnumerable<TypeDefenseEffect> GetAllTypeDefenseEffects();

    TypeOffenseEffect GetTypeOffenseEffect(Type type);

    TypeDefenseEffect GetTypeDefenseEffect(Type type);

    #endregion types

    #region pokemon

    Task<IEnumerable<Pokemon>> GetAllPokemonAsync();

    Task<string> SerializePokemonListAsync();

    Task<PokemonData> GetPokemonDataAsync(Pokemon pokemon);

    Task<IEnumerable<Pokemon>> DeserializePokemonListAsync(string pokemon);

    #endregion pokemon
  }
}
