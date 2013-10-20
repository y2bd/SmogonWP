using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data;
using Schmogon.Data.Moves;

namespace Schmogon
{
  public interface ISchmogonClient
  {
    Task<IEnumerable<Move>> SearchMovesAsync(string query);

    Task<MoveData> GetMoveDataAsync(Move move);
  }
}
