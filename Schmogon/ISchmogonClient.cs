using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data;

namespace Schmogon
{
  public interface ISchmogonClient
  {
    Task<IEnumerable<Move>> SearchMovesAsync(string query);
  }
}
