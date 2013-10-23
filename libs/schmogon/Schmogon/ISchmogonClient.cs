using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Moves;

namespace Schmogon
{
  public interface ISchmogonClient
  {
    Task<IEnumerable<Move>> GetAllMovesAsync();

    Task<IEnumerable<Move>> SearchMovesAsync(string query);

    Task<MoveData> GetMoveDataAsync(Move move);

    Task<IEnumerable<Ability>> GetAllAbilitiesAsync();

    Task<IEnumerable<Ability>> SearchAbilitiesAsync(string query);

    Task<AbilityData> GetAbilityDataAsync(Ability ability);
  }
}
