using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Stats;
using SchmogonDB.Population;
using Type = SchmogonDB.Model.Types.Type;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string FetchPokemonSearchDataQuery =
      @"SELECT rows.Name,
               rows.Tier,
               rows.HP,
               rows.Attack,
               rows.Defense,
               rows.SpecialAttack,
               rows.SpecialDefense,
               rows.Speed,
               rows.Types,
               GROUP_CONCAT(DISTINCT rows.Name_Ability) as Abilities 
        FROM (
         SELECT p.*, 
                GROUP_CONCAT(DISTINCT pt.Type) as Types, 
                atp.Name_Ability 
         FROM Pokemon p
         INNER JOIN PokemonType pt ON pt.Name_Pokemon = p.Name
         INNER JOIN AbilityToPokemon atp ON atp.Name_Pokemon = p.Name
         GROUP BY atp.Name_Ability, p.Name
       ) as rows
       GROUP BY rows.Name";

    private const string FetchPokemonSpritePathQuery =
      @"SELECT p.SpritePath FROM Pokemon p
        WHERE p.Name = @name";

    private const string FetchMovesetsQuery =
      @"SELECT rows.Name,
               rows.HP, 
               rows.Attack, 
               rows.Defense, 
               rows.SpecialAttack, 
               rows.SpecialDefense, 
               rows.Speed, 
               rows.Abilities, 
               rows.Natures,
               rows.Items, 
               GROUP_CONCAT(rows.Moves, '|'),
               rows.mid 
        FROM (
          SELECT m.Name_Pokemon as Pokemon,
                 m.Name AS Name,
                 m.id AS mid,
                 m.EV_HP AS HP,
                 m.EV_Attack AS Attack,
                 m.EV_Defense AS Defense,
                 m.EV_SpecialAttack AS SpecialAttack,
                 m.EV_SpecialDefense AS SpecialDefense,
                 m.EV_Speed AS Speed,
                 GROUP_CONCAT(DISTINCT am.Name_Ability) AS Abilities,
                 GROUP_CONCAT(DISTINCT mn.Nature) AS Natures,
                 GROUP_CONCAT(DISTINCT im.Name_Item) AS Items,
                 GROUP_CONCAT(DISTINCT mmc.Move_FullName) AS Moves
          FROM Pokemon p
          INNER JOIN Moveset m ON m.Name_Pokemon = p.Name
          LEFT JOIN AbilityToMoveset am ON am.id_Moveset = m.id
          LEFT JOIN ItemToMoveset im ON im.id_Moveset = m.id
          LEFT JOIN MovesetNature mn ON mn.id_Moveset = m.id
          INNER JOIN MoveCollection mc ON mc.id_Moveset = m.id
          INNER JOIN MoveToMoveCollection mmc ON mmc.id_MoveCollection = mc.id
          WHERE p.Name = @name
          GROUP BY m.Name, mc.id
        ) AS rows
        GROUP BY rows.Name";

    private IEnumerable<Pokemon> _pokeCache;

    public async Task<IEnumerable<Pokemon>> FetchPokemonSearchDataAsync()
    {
      ensureDatabaseInitialized();

      return _pokeCache ?? (_pokeCache = await fetchPokemonSearchData());
    }

    private async Task<IEnumerable<Pokemon>> fetchPokemonSearchData()
    {
      var pokemon = new List<Pokemon>();

      var statement = await _database.PrepareStatementAsync(FetchPokemonSearchDataQuery);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var tier = (Tier)statement.GetIntAt(1);
        var baseStats = new BaseStat
        (
          statement.GetIntAt(2),
          statement.GetIntAt(3),
          statement.GetIntAt(4),
          statement.GetIntAt(5),
          statement.GetIntAt(6),
          statement.GetIntAt(7)
        );

        var typeString = statement.GetTextAt(8);
        var types = typeString.Split(',').Select(t => (Type)(Int32.Parse(t)));

        var abilityString = statement.GetTextAt(9);
        var abilities = abilityString.Split(',').Select(a => new Ability(a, String.Empty, Utilities.ConstructSmogonLink(a, Utilities.AbilityBasePath)));

        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.PokemonBasePath);

        pokemon.Add(new Pokemon(name, types.ToList(), tier, abilities.ToList(), baseStats, pageLocation));
      }

      return pokemon;
    }

    public async Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon)
    {
      var spritePath = await fetchPokemonSpritePath(pokemon);

      var movepool = await fetchPokemonMoves(pokemon);
      var overview = await FetchTextElementsAsync(pokemon.Name, OwnerType.Pokemon, ElementType.Description);
      var options = await FetchTextElementsAsync(pokemon.Name, OwnerType.Pokemon, ElementType.Options);
      var counters = await FetchTextElementsAsync(pokemon.Name, OwnerType.Pokemon, ElementType.Counters);

      var movesets = await fetchMovesets(pokemon);

      return new PokemonData(
        pokemon.Name,
        pokemon.PageLocation,
        spritePath,
        pokemon.Abilities,
        pokemon.Types,
        pokemon.Tier,
        pokemon.BaseStats,
        overview, // overview
        movesets, // movesets
        options, // options
        counters, // counters
        movepool); // movepool
    }

    private async Task<string> fetchPokemonSpritePath(Pokemon pokemon)
    {
      var statement = await _database.PrepareStatementAsync(FetchPokemonSpritePathQuery);
      statement.BindTextParameterWithName("@name", pokemon.Name);
      statement.StepSync();

      return statement.GetTextAt(0);
    }

    private async Task<IEnumerable<Moveset>> fetchMovesets(Pokemon pokemon)
    {
      var movesets = new List<Moveset>();

      var statement = await _database.PrepareStatementAsync(FetchMovesetsQuery);
      statement.BindTextParameterWithName("@name", pokemon.Name);

      while (statement.StepSync())
      {
        statement.EnableColumnsProperty();

        var name = statement.GetTextAt(0);
        var evSpread = new BaseStat
        (
          statement.GetIntAt(1),
          statement.GetIntAt(2),
          statement.GetIntAt(3),
          statement.GetIntAt(4),
          statement.GetIntAt(5),
          statement.GetIntAt(6)
        );

        var abilties = statement.GetTextAt(7)
                                .Split(',')
                                .Where(s => !String.IsNullOrWhiteSpace(s))
                                .Select(s => new Ability(s, String.Empty, Utilities.ConstructSmogonLink(s, Utilities.AbilityBasePath)))
                                .ToList();

        var natures = statement.GetTextAt(8)
                                .Split(',')
                                .Where(s => !String.IsNullOrWhiteSpace(s))
                                .Select(s => (Nature)Int32.Parse(s))
                                .ToList();

        var items = statement.GetTextAt(9)
                             .Split(',')
                             .Where(s => !String.IsNullOrWhiteSpace(s))
                             .Select(s => new Item(s, String.Empty, Utilities.ConstructSmogonLink(s, Utilities.ItemBasePath)))
                             .ToList();

        var moveCollections = statement.GetTextAt(10)
          .Split('|')
          .Select(
            s => s.Split(',')
                  .Select(ss => new Move(ss, String.Empty, Utilities.ConstructSmogonLink(ss, Utilities.MoveBasePath), Type.Normal))
                  .ToList()
          ).ToList();

        var mid = statement.GetIntAt(11);

        var desc = await FetchTextElementsAsync(mid.ToString(CultureInfo.InvariantCulture), OwnerType.Moveset, ElementType.Description);

        movesets.Add(
          new Moveset
          {
            Abilities = abilties,
            Description = desc.ToList(),
            EVSpread = evSpread,
            Items = items,
            Moves = moveCollections,
            Name = name,
            Natures = natures
          });
      }

      return movesets;
    }
  }
}
