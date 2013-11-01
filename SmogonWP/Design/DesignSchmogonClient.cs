using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon;
using Schmogon.Data.Abilities;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Stats;
using Schmogon.Data.Types;

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

    public async Task<IEnumerable<Ability>> GetAllAbilitiesAsync()
    {
      await Task.Delay(0);

      return new List<Ability>
      {
        new Ability("kung pao", "throws chicken at the opponent, causing confusion", ""),
        new Ability("drum machine", "quick strikes, can hit two to five times", ""),
        new Ability("stackflip", "damage builds as move is used in succession", ""),
        new Ability("hypertension", "boosts attack three stages at the cost of 1/8 max HP", ""),
        new Ability("whiplash", "quick strike that does more damage the slower the opponent", ""),
        new Ability("kung pao", "throws chicken at the opponent, causing confusion", ""),
      };
    }

    public async Task<IEnumerable<Ability>> SearchAbilitiesAsync(string query)
    {
      return await GetAllAbilitiesAsync();
    }

    public async Task<AbilityData> GetAbilityDataAsync(Ability ability)
    {
      await Task.Delay(0);

      return new AbilityData("lime", "makes pokemon super sour", "it's super good because sour pokemon are the worst.");
    }

    public IEnumerable<NatureEffect> GetAllNatureEffects()
    {
      var s = new SchmogonClient();
      return s.GetAllNatureEffects();
    }

    public NatureEffect GetNatureEffect(Nature nature)
    {
      var s = new SchmogonClient();
      return s.GetNatureEffect(Nature.Adamant);
    }

    public IEnumerable<NatureEffect> GetNatureEffectsWhere(StatType increased, StatType decreased)
    {
      var s = new SchmogonClient();
      return s.GetNatureEffectsWhere(StatType.Attack, StatType.Speed);
    }

    public IEnumerable<NatureEffect> GetNatureEffectsWhereIncreased(StatType increased)
    {
      var s = new SchmogonClient();
      return s.GetNatureEffectsWhereIncreased(StatType.SpecialAttack);
    }

    public IEnumerable<NatureEffect> GetNatureEffectsWhereDecreased(StatType decreased)
    {
      var s = new SchmogonClient();
      return s.GetNatureEffectsWhereDecreased(StatType.Defense);
    }

    public IEnumerable<TypeOffenseEffect> GetAllTypeOffenseEffects()
    {
      var s = new SchmogonClient();
      return s.GetAllTypeOffenseEffects();
    }

    public IEnumerable<TypeDefenseEffect> GetAllTypeDefenseEffects()
    {
      var s = new SchmogonClient();
      return s.GetAllTypeDefenseEffects();
    }

    public TypeOffenseEffect GetTypeOffenseEffect(Type type)
    {
      var s = new SchmogonClient();
      return s.GetTypeOffenseEffect(type);
    }

    public TypeDefenseEffect GetTypeDefenseEffect(Type type)
    {
      var s = new SchmogonClient();
      return s.GetTypeDefenseEffect(type);
    }
  }
}
