using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon;
using Schmogon.Data.Moves;

namespace SmogonWP.Design
{
  public class DesisgnSchmogonClient : ISchmogonClient
  {
    public async Task<IEnumerable<Move>> SearchMovesAsync(string query)
    {
      await Task.Delay(0);

      return new List<Move>();
    }

    public async Task<MoveData> GetMoveDataAsync(Move move)
    {
      await Task.Delay(0);

      return null;
    }
  }
}
