using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schmogon.Data.Pokemon;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string InsertPokemonQuery = 
      "INSERT INTO Pokemon VALUES (@name, @spritePath, @tier, @hp, @attack, @defense, @specialAttack, @specialDefense, @speed);";

    private const string InsertPokemonTypeQuery =
      "INSERT INTO PokemonType (Type, Name_Pokemon) VALUES (@type, @pokemon)";

    private async Task<string> insertPokemonData(PokemonData pokemon)
    {
      var statement = await _database.PrepareStatementAsync(InsertPokemonQuery);
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
        await insertTextElement(desc, pokemon.Name, OwnerType.Pokemon, ElementType.Description);
      }

      foreach (var option in pokemon.OtherOptions)
      {
        await insertTextElement(option, pokemon.Name, OwnerType.Pokemon, ElementType.Options);
      }

      foreach (var counter in pokemon.ChecksAndCounters)
      {
        await insertTextElement(counter, pokemon.Name, OwnerType.Pokemon, ElementType.Counters);
      }

      await insertPokemonAbilityConnections(pokemon);

      await insertPokemonTypeConnections(pokemon);

      await insertPokemonMovesConnections(pokemon);

      foreach (var moveset in pokemon.Movesets)
      {
        await insertMoveset(moveset, pokemon.Name);
      }

      return pokemon.Name;
    }

    private async Task<long> insertPokemonTypeConnections(PokemonData pokemon)
    {
      long lastKey = 0;

      foreach (var type in pokemon.Types)
      {
        var statement = await _database.PrepareStatementAsync(InsertPokemonTypeQuery);
        statement.BindIntParameterWithName("@type", (int) type);
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
  }
}
