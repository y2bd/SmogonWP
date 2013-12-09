using SchmogonDB;
using SchmogonDB.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Text;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.Design
{
  public class DesignSchmogonDBClient : ISchmogonDBClient
  {
    public async Task<IEnumerable<Item>> FetchItemSearchDataAsync()
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

    public async Task<ItemData> FetchItemDataAsync(Item item)
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

    public async Task<IEnumerable<Move>> FetchMoveSearchDataAsync()
    {
      await Task.Delay(0);

      return new List<Move>
      {
        new Move("kung pao", "throws chicken at the opponent, causing confusion", "", Type.Fire),
        new Move("drum machine", "quick strikes, can hit two to five times", "", Type.Rock),
        new Move("stackflip", "damage builds as move is used in succession", "", Type.Fighting),
        new Move("hypertension", "boosts attack three stages at the cost of 1/8 max HP", "", Type.Fighting),
        new Move("whiplash", "quick strike that does more damage the slower the opponent", "", Type.Water),
        new Move("kung pao", "throws chicken at the opponent, causing confusion", "", Type.Fire),
      };
    }

    public async Task<MoveData> FetchMoveDataAsync(Move move)
    {
      await Task.Delay(0);

      var stats = new MoveStats(
        Type.Steel,
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
        new List<Move> { new Move("Swords Dance", "Swords Dance raises attack by two stages without a health penalty.", "", Type.Normal) });

      return data;
    }

    public async Task<IEnumerable<Pokemon>> FetchPokemonSearchDataAsync()
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

    public async Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon)
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
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)},
              new List<Move>
              {
                new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal),
                new Move("Milk Drain", "gotta get dat white stuff", "", Type.Normal)
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
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)},
              new List<Move> {new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)},
              new List<Move>
              {
                new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal),
                new Move("Milk Drain", "gotta get dat white stuff", "", Type.Normal)
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
          new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal),
          new Move("Milk Drain", "gotta get dat white stuff", "", Type.Normal)
        }
        );
    }

    public async Task<IEnumerable<Ability>> FetchAbilitySearchDataAsync()
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

    public async Task<AbilityData> FetchAbilityDataAsync(Ability ability)
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

    public async Task InitializeDatabase()
    {
      await Task.Delay(0);
    }
  }
}
