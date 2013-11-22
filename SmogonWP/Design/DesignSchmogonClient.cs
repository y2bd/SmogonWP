using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Stats;
using Schmogon.Data.Types;
using Schmogon.Model.Text;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.Design
{
  public class DesignSchmogonClient : ISchmogonClient
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

    public async Task<IEnumerable<Move>> GetMovesOfTypeAsync(Type type)
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

    public async Task<IEnumerable<Move>> SearchMovesOfTypeAsync(Type type, string query)
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

      var descData = new List<ITextElement>
      {
        new Paragraph("Boosts the attack of the user by three stages at the cost of 1/8 the user's max HP.")
      };

      var compData = new List<ITextElement>
      {
        new Paragraph(
          "Hypertension is an interesting move. It seems that it would best suit power sweepers, but the problem is that not only does it spend a turn, but many power sweeper builds have low defense to begin with, so a health penalty isn't always the best choice."),
        new Paragraph("For most setups, Dragon Dance or Swords Dance is a better choice if the pokemon can learn it.")
      };

      var data = new MoveData(
        "Hypertension",
        stats,
        descData,
        compData,
        new List<Move> { new Move("Swords Dance", "Swords Dance raises attack by two stages without a health penalty.", "") });

      return data;
    }

    public async Task<string> SerializeMoveListAsync()
    {
      await Task.Delay(0);

      return "[]";
    }

    public async Task<string> SerializeMoveListAsync(Type type)
    {
      await Task.Delay(0);

      return "[]";
    }

    public async Task<IEnumerable<Move>> DeserializeMoveListAsync(string moves)
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

    public async Task<IEnumerable<Move>> DeserializeMoveListAsync(Type type, string moves)
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

      var descData = new List<ITextElement>
      {
        new Paragraph("makes pokemon super sour"),
        new Paragraph("does not work on the following pokemon"),
        new UnorderedList(
          new List<string>
          {
            "chingling",
            "chimecho",
            "chocobo"
          })
      };

      var compData = new List<ITextElement>
      {
        new Paragraph("it's super good because sour pokemon are the worst.")
      };

      return new AbilityData("lime", descData, compData);
    }

    public async Task<string> SerializeAbilityListAsync()
    {
      await Task.Delay(0);

      return "[]";
    }

    public async Task<IEnumerable<Ability>> DeserializeAbilityListAsync(string abilities)
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

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
      await Task.Delay(0);

      return new List<Item>
      {
        new Item("kung pao", "throws chicken at the opponent, causing confusion", ""),
        new Item("drum machine", "quick strikes, can hit two to five times", ""),
        new Item("stackflip", "damage builds as move is used in succession", ""),
        new Item("hypertension", "boosts attack three stages at the cost of 1/8 max HP", ""),
        new Item("whiplash", "quick strike that does more damage the slower the opponent", ""),
        new Item("kung pao", "throws chicken at the opponent, causing confusion", ""),
      };
    }

    public async Task<IEnumerable<Item>> SearchItemsAsync(string query)
    {
      return await GetAllItemsAsync();
    }

    public async Task<ItemData> GetItemDataAsync(Item item)
    {
      await Task.Delay(0);

      var descData = new List<ITextElement>
      {
        new Paragraph("makes pokemon super sour"),
        new Paragraph("does not work on the following pokemon"),
        new UnorderedList(
          new List<string>
          {
            "chingling",
            "chimecho",
            "chocobo"
          })
      };

      var compData = new List<ITextElement>
      {
        new Paragraph("it's super good because sour pokemon are the worst.")
      };

      return new ItemData("lime", descData, compData);
    }

    public async Task<string> SerializeItemListAsync()
    {
      await Task.Delay(0);

      return "[]";
    }

    public async Task<IEnumerable<Item>> DeserializeItemListAsync(string abilities)
    {
      await Task.Delay(0);

      return new List<Item>
      {
        new Item("kung pao", "throws chicken at the opponent, causing confusion", ""),
        new Item("drum machine", "quick strikes, can hit two to five times", ""),
        new Item("stackflip", "damage builds as move is used in succession", ""),
        new Item("hypertension", "boosts attack three stages at the cost of 1/8 max HP", ""),
        new Item("whiplash", "quick strike that does more damage the slower the opponent", ""),
        new Item("kung pao", "throws chicken at the opponent, causing confusion", ""),
      };
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

    public async Task<IEnumerable<Pokemon>> GetAllPokemonAsync()
    {
      await Task.Delay(0);

      return new List<Pokemon>
      {
        new Pokemon(
          "Barbasaur",
          new List<Type> {Type.Grass, Type.Fire},
          Tier.Uber,
          new List<Ability>
          {
            new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
            new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
          },
          new BaseStat(0, 0, 0, 0, 0, 255),
          ""),
        new Pokemon(
          "Barbasaur",
          new List<Type> {Type.Grass, Type.Fire},
          Tier.Uber,
          new List<Ability>
          {
            new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
            new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
          },
          new BaseStat(0, 0, 0, 0, 0, 255),
          ""),
        new Pokemon(
          "Barbasaur",
          new List<Type> {Type.Grass, Type.Fire},
          Tier.Uber,
          new List<Ability>
          {
            new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
            new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
          },
          new BaseStat(0, 0, 0, 0, 0, 255),
          ""),
      };
    }

    public async Task<string> SerializePokemonListAsync()
    {
      await Task.Delay(0);

      return "[]";
    }

    public async Task<PokemonData> GetPokemonDataAsync(Pokemon pokemon)
    {
      await Task.Delay(0);

      return new PokemonData(
        "Barbasaur",
        "",
        "http://www.smogon.com/download/sprites/bw/206.png",
        new List<Ability>
        {
          new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
          new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
        },
        new List<Type> { Type.Grass, Type.Fire },
        Tier.Uber,
        new BaseStat(0, 0, 0, 0, 0, 255),
        new List<ITextElement>
        {
          new Paragraph("it's a pretty good pokemon honestly, but it's really purple and im not a fan of much purple")
        },
        new List<Moveset>
        {
          new Moveset
          {
            Name = "Kickapow",
            Abilities = new List<Ability> {new Ability("Burnt Toast", "Burns toast 100% of the time", "")},
            Description =
              new List<ITextElement>
              {
                new Paragraph("THIS MOVESET IS REALL GOOD ECAUSE IT HITS HARD NO SQUISHY FLAMEBATE LOL")
              },
            EVSpread = new BaseStat(6, 252, 0, 0, 0, 252),
            Items = new List<Item> {new Item("Life Orb", "Kicks life in the orbs", "")},
            Moves = new List<IEnumerable<Move>>
            {
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "")},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "")},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "")},
              new List<Move>
              {
                new Move("Chocolate Sauce", "mm dat drizzle", ""),
                new Move("Milk Drain", "gotta get dat white stuff", "")
              }
            },
            Natures = new List<Nature> {Nature.Adamant, Nature.Brave}
          },
          new Moveset
          {
            Name = "Kickapow",
            Abilities = new List<Ability> {new Ability("Burnt Toast", "Burns toast 100% of the time", "")},
            Description =
              new List<ITextElement>
              {
                new Paragraph("THIS MOVESET IS REALL GOOD ECAUSE IT HITS HARD NO SQUISHY FLAMEBATE LOL")
              },
            EVSpread = new BaseStat(6, 252, 0, 0, 0, 252),
            Items = new List<Item> {new Item("Life Orb", "Kicks life in the orbs", "")},
            Moves = new List<IEnumerable<Move>>
            {
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "")},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "")},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "")},
              new List<Move>
              {
                new Move("Chocolate Sauce", "mm dat drizzle", ""),
                new Move("Milk Drain", "gotta get dat white stuff", "")
              }
            },
            Natures = new List<Nature> {Nature.Adamant, Nature.Brave}
          }
        },
        new List<ITextElement>
        {
          new Paragraph("it's a pretty good pokemon honestly, but it's really purple and im not a fan of much purple")
        },
        new List<ITextElement>
        {
          new Paragraph("it's a pretty good pokemon honestly, but it's really purple and im not a fan of much purple")
        },
        new List<Move>
        {
          new Move("Chocolate Sauce", "mm dat drizzle", ""),
          new Move("Milk Drain", "gotta get dat white stuff", "")
        }
        );
    }

    public async Task<IEnumerable<Pokemon>> DeserializePokemonListAsync(string pokemon)
    {
      await Task.Delay(0);

      return new List<Pokemon>
      {
        new Pokemon(
          "Barbasaur",
          new List<Type> {Type.Grass, Type.Fire},
          Tier.Uber,
          new List<Ability>
          {
            new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
            new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
          },
          new BaseStat(0, 0, 0, 0, 0, 255),
          ""),
        new Pokemon(
          "Barbasaur",
          new List<Type> {Type.Grass, Type.Fire},
          Tier.Uber,
          new List<Ability>
          {
            new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
            new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
          },
          new BaseStat(0, 0, 0, 0, 0, 255),
          ""),
        new Pokemon(
          "Barbasaur",
          new List<Type> {Type.Grass, Type.Fire},
          Tier.Uber,
          new List<Ability>
          {
            new Ability("Burnt Toast", "Burns toast 100% of the time", ""),
            new Ability("Chocolate Silk", "Like milk except it comes out a spider's butt", "")
          },
          new BaseStat(0, 0, 0, 0, 0, 255),
          ""),
      };
    }
  }
}
