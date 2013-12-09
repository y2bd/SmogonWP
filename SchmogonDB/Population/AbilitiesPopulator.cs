using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Pokemon;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
  {
    private const string InsertAbilityQuery = "INSERT INTO Ability VALUES (@name, @shortdesc);";

    private const string InsertAbilityToPokemonQuery =
      "INSERT INTO AbilityToPokemon (Name_Ability, Name_Pokemon) VALUES (@ability, @pokemon);";

    private const string InsertAbilityToMovesetQuery =
      "INSERT INTO AbilityToMoveset (Name_Ability, id_Moveset) VALUES (@ability, @moveset);";

    private async Task<string> insertAbilityData(Database database, Ability ability, AbilityData abilityData)
    {
      var statement = await database.PrepareStatementAsync(InsertAbilityQuery);
      statement.BindTextParameterWithName("@name", abilityData.Name);
      statement.BindTextParameterWithName("@shortdesc", ability.Description);

      await statement.StepAsync();

      foreach (var desc in abilityData.Description)
      {
        await insertTextElement(database, desc, abilityData.Name, OwnerType.Ability, ElementType.Description);
      }

      foreach (var comp in abilityData.Competitive)
      {
        await insertTextElement(database, comp, abilityData.Name, OwnerType.Ability, ElementType.Competitive);
      }
      
      return abilityData.Name;
    }

    private async Task<long> insertPokemonAbilityConnections(Database database, PokemonData pokemon)
    {
      long lastKey = 0;

      foreach (var ability in pokemon.Abilities)
      {
        var statement = await database.PrepareStatementAsync(InsertAbilityToPokemonQuery);
        statement.BindTextParameterWithName("@ability", ability.Name);
        statement.BindTextParameterWithName("@pokemon", pokemon.Name);

        try
        {
          await statement.StepAsync();
        }
        catch (Exception)
        {
          Debugger.Break();

          continue;
        }

        lastKey = database.GetLastInsertedRowId();
      }

      return lastKey;
    }

    private async Task<long> insertMovesetAbilityConnections(Database database, Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      if (moveset.Abilities == null || !moveset.Abilities.Any()) return lastKey;

      foreach (var ability in moveset.Abilities)
      {
        var statement = await database.PrepareStatementAsync(InsertAbilityToMovesetQuery);
        statement.BindTextParameterWithName("@ability", ability.Name);
        statement.BindInt64ParameterWithName("@moveset", movesetId);

        try
        {
          await statement.StepAsync();
        }
        catch (Exception)
        {
          Debugger.Break();

          continue;
        }

        lastKey = database.GetLastInsertedRowId();
      }

      return lastKey;
    }
  }
}
