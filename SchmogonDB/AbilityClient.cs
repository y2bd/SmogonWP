using Schmogon.Data.Abilities;
using SchmogonDB.Population;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string FetchAbilitySearchDataQuery =
      @"SELECT * FROM Ability";

    private IEnumerable<Ability> _abilityCache;

    public async Task<IEnumerable<Ability>> FetchAbilitySearchDataAsync()
    {
      ensureDatabaseInitialized();

      return _abilityCache ?? (_abilityCache = await fetchAbilitySearchData());
    }

    private async Task<IEnumerable<Ability>> fetchAbilitySearchData()
    {
      var moves = new List<Ability>();

      var statement = await _database.PrepareStatementAsync(FetchAbilitySearchDataQuery);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);

        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.AbilityBasePath);

        moves.Add(new Ability(name, desc, pageLocation));
      }
      
      return moves;
    }

    public async Task<AbilityData> FetchAbilityDataAsync(Ability ability)
    {
      var desc = await fetchTextElements(ability.Name, OwnerType.Ability, ElementType.Description);
      var comp = await fetchTextElements(ability.Name, OwnerType.Ability, ElementType.Competitive);

      return new AbilityData(
        ability.Name,
        desc,
        comp);
    }
  }
}
