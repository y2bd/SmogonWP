using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Stats;
using Schmogon.Data.Types;

namespace Schmogon
{
  public interface ISchmogonClient
  {
    Task<IEnumerable<Move>> GetAllMovesAsync();

    Task<IEnumerable<Move>> GetMovesOfTypeAsync(Type type);

    Task<IEnumerable<Move>> SearchMovesAsync(string query);

    Task<IEnumerable<Move>> SearchMovesOfTypeAsync(Type type, string query);

    Task<MoveData> GetMoveDataAsync(Move move);

    Task<IEnumerable<Ability>> GetAllAbilitiesAsync();

    Task<IEnumerable<Ability>> SearchAbilitiesAsync(string query);

    Task<AbilityData> GetAbilityDataAsync(Ability ability);

    IEnumerable<NatureEffect> GetAllNatureEffects();

    NatureEffect GetNatureEffect(Nature nature);

    IEnumerable<NatureEffect> GetNatureEffectsWhere(StatType increased, StatType decreased);

    IEnumerable<NatureEffect> GetNatureEffectsWhereIncreased(StatType increased);

    IEnumerable<NatureEffect> GetNatureEffectsWhereDecreased(StatType decreased);

    IEnumerable<TypeOffenseEffect> GetAllTypeOffenseEffects();

    IEnumerable<TypeDefenseEffect> GetAllTypeDefenseEffects();

    TypeOffenseEffect GetTypeOffenseEffect(Type type);

    TypeDefenseEffect GetTypeDefenseEffect(Type type);
  }
}
