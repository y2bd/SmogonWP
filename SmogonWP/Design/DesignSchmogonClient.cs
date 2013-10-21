using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon;
using Schmogon.Data.Moves;

namespace SmogonWP.Design
{
  public class DesisgnSchmogonClient : ISchmogonClient
  {
    public async Task<IEnumerable<Move>> GetAllMovesAsync()
    {
      await Task.Delay(0);

      return new List<Move>
      {
        new Move("kung pao", "throws chicken at the opponent, causing confusion", ""),
        new Move("drum machine", "quick strikes, can hit two to five times", ""),
        new Move("stackflip", "damage builds as move is used in succession", ""),
        new Move("hypertension", "boosts attack three stages at the cost of 1/8 max HP", ""),
        new Move("whiplash", "quick strike that does more damage the slower the opponent", ""),
        new Move("kung pao", "throws chicken at the opponent, causing confusion", ""),
      };
    }

    public async Task<IEnumerable<Move>> SearchMovesAsync(string query)
    {
      await Task.Delay(0);

      return new List<Move>
      {
        new Move("kung pao", "throws chicken at the opponent, causing confusion", ""),
        new Move("drum machine", "quick strikes, can hit two to five times", ""),
        new Move("stackflip", "damage builds as move is used in succession", ""),
        new Move("hypertension", "boosts attack three stages at the cost of 1/8 max HP", ""),
        new Move("whiplash", "quick strike that does more damage the slower the opponent", ""),
        new Move("kung pao", "throws chicken at the opponent, causing confusion", ""),
      };
    }

    public async Task<MoveData> GetMoveDataAsync(Move move)
    {
      await Task.Delay(0);

      var stats = new MoveStats(
        "Steel",
        "-",
        "-",
        "15",
        "0",
        "-",
        "Single non-user");

      var data = new MoveData(
        "Hypertension",
        stats,
        "Boosts the attack of the user by three stages at the cost of 1/8 the user's max HP.",
        "Hypertension is an interesting move. It seems that it would best suit power sweepers, but the problem is that not only does it spend a turn, but many power sweeper builds have low defense to begin with, so a health penalty isn't always the best choice. For most setups, Swords Dance is a better choice if the pokemon can learn it.",
        new List<Move> { new Move("Swords Dance", "Swords Dance raises attack by two stages without a health penalty.", "") });

      return data;
    }

    
  }
}
