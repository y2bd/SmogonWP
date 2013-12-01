using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schmogon.Data.Abilities;
using Schmogon.Data.Pokemon;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string InsertAbilityQuery = "INSERT INTO Ability VALUES (@name);";

    private const string InsertAbilityToPokemonQuery =
      "INSERT INTO AbilityToPokemon (Name_Ability, Name_Pokemon) VALUES (@ability, @pokemon);";

    private const string InsertAbilityToMovesetQuery =
      "INSERT INTO AbilityToMoveset (Name_Ability, id_Moveset) VALUES (@ability, @moveset);";

    private async Task<string> insertAbilityData(AbilityData ability)
    {
      var statement = await _database.PrepareStatementAsync(InsertAbilityQuery);
      statement.BindTextParameterWithName("@name", ability.Name);

      await statement.StepAsync();

      foreach (var desc in ability.Description)
      {
        await insertTextElement(desc, ability.Name, OwnerType.Ability, ElementType.Description);
      }

      foreach (var comp in ability.Competitive)
      {
        await insertTextElement(comp, ability.Name, OwnerType.Ability, ElementType.Competitive);
      }

      var key = _database.GetLastInsertedRowId();

      return ability.Name;
    }

    private async Task<long> insertPokemonAbilityConnections(PokemonData pokemon)
    {
      long lastKey = 0;

      foreach (var ability in pokemon.Abilities)
      {
        var statement = await _database.PrepareStatementAsync(InsertAbilityToPokemonQuery);
        statement.BindTextParameterWithName("@ability", ability.Name);
        statement.BindTextParameterWithName("@pokemon", pokemon.Name);

        try
        {
          await statement.StepAsync();
        }
        catch (Exception e)
        {
          continue;
        }

        lastKey = _database.GetLastInsertedRowId();
      }

      return lastKey;
    }

    private async Task<long> insertMovesetAbilityConnections(Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      if (moveset.Abilities == null || !moveset.Abilities.Any()) return lastKey;

      foreach (var ability in moveset.Abilities)
      {
        var statement = await _database.PrepareStatementAsync(InsertAbilityToMovesetQuery);
        statement.BindTextParameterWithName("@ability", ability.Name);
        statement.BindInt64ParameterWithName("@moveset", movesetId);

        try
        {
          await statement.StepAsync();
        }
        catch (Exception e)
        {
          continue;
        }

        lastKey = _database.GetLastInsertedRowId();
      }

      return lastKey;
    }
  }
}
