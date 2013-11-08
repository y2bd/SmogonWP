using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using Schmogon;
using Type = Schmogon.Data.Types.Type;

namespace SchmogonTest
{
  class Program
  {
    static void Main(string[] args)
    {
      Task.WaitAll(test());

      return;
    }

    static async Task test()
    {
      var s = new SchmogonClient();

      Console.Write("Please enter a query: ");
      var query = Console.ReadLine();

      /*
      var moves =
        await
          s.DeserializeMoveListAsync(Type.Psychic,
            "[{\"Name\":\"Agility\",\"Description\":\"Boosts user's Speed by 2 stages.\",\"PageLocation\":\"/bw/moves/agility\"},{\"Name\":\"Ally Switch\",\"Description\":\"Switches position with another Pokemon on user's side of the field.\",\"PageLocation\":\"/bw/moves/ally_switch\"},{\"Name\":\"Amnesia\",\"Description\":\"Boosts user's Special Defense by 2 stages.\",\"PageLocation\":\"/bw/moves/amnesia\"},{\"Name\":\"Barrier\",\"Description\":\"Boosts user's Defense by 2 stages.\",\"PageLocation\":\"/bw/moves/barrier\"},{\"Name\":\"Calm Mind\",\"Description\":\"Boosts user's Special Attack and Special Defense by 1 stage.\",\"PageLocation\":\"/bw/moves/calm_mind\"},{\"Name\":\"Confusion\",\"Description\":\"10% chance to confuse opponent.\",\"PageLocation\":\"/bw/moves/confusion\"},{\"Name\":\"Cosmic Power\",\"Description\":\"Boosts user's Defense and Special Defense by 1 stage.\",\"PageLocation\":\"/bw/moves/cosmic_power\"},{\"Name\":\"Dream Eater\",\"Description\":\"Leeches 50% of the damage dealt. Only works if target is asleep.\",\"PageLocation\":\"/bw/moves/dream_eater\"},{\"Name\":\"Extrasensory\",\"Description\":\"10% chance to flinch the target.\",\"PageLocation\":\"/bw/moves/extrasensory\"},{\"Name\":\"Future Sight\",\"Description\":\"Deals damage at the end of the turn after three turns.\",\"PageLocation\":\"/bw/moves/future_sight\"},{\"Name\":\"Gravity\",\"Description\":\"For 5 turns, prevents flying or jumping. Also negates Ground immunities.\",\"PageLocation\":\"/bw/moves/gravity\"},{\"Name\":\"Guard Split\",\"Description\":\"Averages Defense and Special Defense stats of the user with the target.\",\"PageLocation\":\"/bw/moves/guard_split\"},{\"Name\":\"Guard Swap\",\"Description\":\"Swaps user's Defense and Special Defense stages with the target.\",\"PageLocation\":\"/bw/moves/guard_swap\"},{\"Name\":\"Heal Block\",\"Description\":\"Prevents opponents from healing for 5 turns.\",\"PageLocation\":\"/bw/moves/heal_block\"},{\"Name\":\"Heal Pulse\",\"Description\":\"Heals the target by 50% its max HP.\",\"PageLocation\":\"/bw/moves/heal_pulse\"},{\"Name\":\"Healing Wish\",\"Description\":\"Faints the user, but heals the replacement.\",\"PageLocation\":\"/bw/moves/healing_wish\"},{\"Name\":\"Heart Stamp\",\"Description\":\"30% chance to flinch the target.\",\"PageLocation\":\"/bw/moves/heart_stamp\"},{\"Name\":\"Heart Swap\",\"Description\":\"Switches user's stat changes with target's.\",\"PageLocation\":\"/bw/moves/heart_swap\"},{\"Name\":\"Hypnosis\",\"Description\":\"Puts the target to sleep.\",\"PageLocation\":\"/bw/moves/hypnosis\"},{\"Name\":\"Imprison\",\"Description\":\"No opponent can use any of the user's moves.\",\"PageLocation\":\"/bw/moves/imprison\"},{\"Name\":\"Kinesis\",\"Description\":\"Lowers target's accuracy by 1 stage.\",\"PageLocation\":\"/bw/moves/kinesis\"},{\"Name\":\"Light Screen\",\"Description\":\"Doubles your team's Special Defense for 5 turns.\",\"PageLocation\":\"/bw/moves/light_screen\"},{\"Name\":\"Lunar Dance\",\"Description\":\"Faints the user, but the replacement is healed completely.\",\"PageLocation\":\"/bw/moves/lunar_dance\"},{\"Name\":\"Luster Purge\",\"Description\":\"50% chance to lower target's Special Defense by 1 stage.\",\"PageLocation\":\"/bw/moves/luster_purge\"},{\"Name\":\"Magic Coat\",\"Description\":\"Bounces back certain non-damaging moves.\",\"PageLocation\":\"/bw/moves/magic_coat\"},{\"Name\":\"Magic Room\",\"Description\":\"Held items have no effect for 5 turns.\",\"PageLocation\":\"/bw/moves/magic_room\"},{\"Name\":\"Meditate\",\"Description\":\"Boosts user's Attack by 1 stage.\",\"PageLocation\":\"/bw/moves/meditate\"},{\"Name\":\"Miracle Eye\",\"Description\":\"Blocks evasion modifiers. Allows user's Psychic-type moves to hit Dark-types.\",\"PageLocation\":\"/bw/moves/miracle_eye\"},{\"Name\":\"Mirror Coat\",\"Description\":\"If hit by a special attack, returns double the damage.\",\"PageLocation\":\"/bw/moves/mirror_coat\"},{\"Name\":\"Mist Ball\",\"Description\":\"50% chance to lower target's Special Attack by 1 stage.\",\"PageLocation\":\"/bw/moves/mist_ball\"},{\"Name\":\"Power Split\",\"Description\":\"Averages Attack and Special Attack with the target.\",\"PageLocation\":\"/bw/moves/power_split\"},{\"Name\":\"Power Swap\",\"Description\":\"Switches Attack and Special Attack boosts with the target.\",\"PageLocation\":\"/bw/moves/power_swap\"},{\"Name\":\"Power Trick\",\"Description\":\"Switches Attack and Defense stats.\",\"PageLocation\":\"/bw/moves/power_trick\"},{\"Name\":\"Psybeam\",\"Description\":\"10% chance to confuse the target.\",\"PageLocation\":\"/bw/moves/psybeam\"},{\"Name\":\"Psychic\",\"Description\":\"10% chance to lower target's Special Defense by 1 stage.\",\"PageLocation\":\"/bw/moves/psychic\"},{\"Name\":\"Psycho Boost\",\"Description\":\"Lowers user's Special Attack by 2 stages.\",\"PageLocation\":\"/bw/moves/psycho_boost\"},{\"Name\":\"Psycho Cut\",\"Description\":\"Has a high critical hit rate.\",\"PageLocation\":\"/bw/moves/psycho_cut\"},{\"Name\":\"Psycho Shift\",\"Description\":\"Transfers status ailments to the target.\",\"PageLocation\":\"/bw/moves/psycho_shift\"},{\"Name\":\"Psyshock\",\"Description\":\"Damage is based on the target's Defense stat.\",\"PageLocation\":\"/bw/moves/psyshock\"},{\"Name\":\"Psystrike\",\"Description\":\"Damage is based on the target's Defense stat.\",\"PageLocation\":\"/bw/moves/psystrike\"},{\"Name\":\"Psywave\",\"Description\":\"Does random damage equal to .5x-1.5x the user's level.\",\"PageLocation\":\"/bw/moves/psywave\"},{\"Name\":\"Reflect\",\"Description\":\"Lowers damage from physical attacks to the user's team for 5 turns.\",\"PageLocation\":\"/bw/moves/reflect\"},{\"Name\":\"Rest\",\"Description\":\"The user goes to sleep for two turns and restores all HP.\",\"PageLocation\":\"/bw/moves/rest\"},{\"Name\":\"Role Play\",\"Description\":\"Copies the foe's ability and overwrites the user's ability.\",\"PageLocation\":\"/bw/moves/role_play\"},{\"Name\":\"Skill Swap\",\"Description\":\"The user and the target trade abilities.\",\"PageLocation\":\"/bw/moves/skill_swap\"},{\"Name\":\"Stored Power\",\"Description\":\"Varies in power depending on number of boosts accumulated by user.\",\"PageLocation\":\"/bw/moves/stored_power\"},{\"Name\":\"Synchronoise\",\"Description\":\"Only damages targets of the same type as the user.\",\"PageLocation\":\"/bw/moves/synchronoise\"},{\"Name\":\"Telekinesis\",\"Description\":\"Lasts 3 turns and makes all moves hit the target.  Makes target immune to Ground-type moves.\",\"PageLocation\":\"/bw/moves/telekinesis\"},{\"Name\":\"Teleport\",\"Description\":\"Flee from wild Pokemon battles.\",\"PageLocation\":\"/bw/moves/teleport\"},{\"Name\":\"Trick\",\"Description\":\"Switches items with the target.\",\"PageLocation\":\"/bw/moves/trick\"},{\"Name\":\"Trick Room\",\"Description\":\"Slower Pokemon move first for 5 turns.\",\"PageLocation\":\"/bw/moves/trick_room\"},{\"Name\":\"Wonder Room\",\"Description\":\"Swaps Defense and Special Defense of all Pokemon.\",\"PageLocation\":\"/bw/moves/wonder_room\"},{\"Name\":\"Zen Headbutt\",\"Description\":\"20% chance to flinch the target.\",\"PageLocation\":\"/bw/moves/zen_headbutt\"}]");


      foreach (var move in moves)
      {
        Console.WriteLine(move.Name);
      }

      await s.SerializeMoveListAsync(Type.Psychic);
       * */


      var abilties = await s.GetAllAbilitiesAsync();

      var d = await s.GetAbilityDataAsync(abilties.First());

      var c = await s.SerializeAbilityListAsync();
    }
  }
}
