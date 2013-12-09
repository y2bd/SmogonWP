using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SchmogonDB.Model.Pokemon;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
  {
    private const string InsertPokemonQuery = 
      "INSERT INTO Pokemon VALUES (@name, @spritePath, @tier, @hp, @attack, @defense, @specialAttack, @specialDefense, @speed);";

    private const string InsertPokemonTypeQuery =
      "INSERT INTO PokemonType (Type, Name_Pokemon) VALUES (@type, @pokemon)";

    private async Task<string> insertPokemonData(Database database, PokemonData pokemon)
    {
      var statement = await database.PrepareStatementAsync(InsertPokemonQuery);
      statement.BindTextParameterWithName("@name", pokemon.Name);
      statement.BindTextParameterWithName("@spritePath", pokemon.SpritePath);
      statement.BindIntParameterWithName("@tier", (int)pokemon.Tier);
      statement.BindIntParameterWithName("@hp", pokemon.BaseStats.HP);
      statement.BindIntParameterWithName("@attack", pokemon.BaseStats.Attack);
      statement.BindIntParameterWithName("@defense", pokemon.BaseStats.Defense);
      statement.BindIntParameterWithName("@specialAttack", pokemon.BaseStats.SpecialAttack);
      statement.BindIntParameterWithName("@specialDefense", pokemon.BaseStats.SpecialDefense);
      statement.BindIntParameterWithName("@speed", pokemon.BaseStats.Speed);

      await statement.StepAsync();

      foreach (var desc in pokemon.Overview)
      {
        await insertTextElement(database, desc, pokemon.Name, OwnerType.Pokemon, ElementType.Description);
      }

      foreach (var option in pokemon.OtherOptions)
      {
        await insertTextElement(database, option, pokemon.Name, OwnerType.Pokemon, ElementType.Options);
      }

      foreach (var counter in pokemon.ChecksAndCounters)
      {
        await insertTextElement(database, counter, pokemon.Name, OwnerType.Pokemon, ElementType.Counters);
      }

      await insertPokemonAbilityConnections(database, pokemon);

      await insertPokemonTypeConnections(database, pokemon);

      await insertPokemonMovesConnections(database, pokemon);

      foreach (var moveset in pokemon.Movesets)
      {
        await insertMoveset(database, moveset, pokemon.Name);
      }

      return pokemon.Name;
    }

    private async Task<long> insertPokemonTypeConnections(Database database, PokemonData pokemon)
    {
      long lastKey = 0;

      foreach (var type in pokemon.Types)
      {
        var statement = await database.PrepareStatementAsync(InsertPokemonTypeQuery);
        statement.BindIntParameterWithName("@type", (int) type);
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
  }
}
