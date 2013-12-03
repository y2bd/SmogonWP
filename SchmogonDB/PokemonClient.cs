using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Stats;
using Type = Schmogon.Data.Types.Type;

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
        var tier = (Tier) statement.GetIntAt(1);
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
        var types = typeString.Split(',').Select(t => (Type) (int.Parse(t)));

        var abilityString = statement.GetTextAt(9);
        var abilities = abilityString.Split(',').Select(a => new Ability(a, string.Empty, Utilities.ConstructSmogonLink(a, Utilities.AbilityBasePath)));

        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.PokemonBasePath);

        pokemon.Add(new Pokemon(name, types.ToList(), tier, abilities.ToList(), baseStats, pageLocation));
      }

      return pokemon;
    }
  }
}
