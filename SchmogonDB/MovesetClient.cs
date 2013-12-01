using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string InsertMovesetQuery =
      "INSERT INTO Moveset (Name, EV_HP, EV_Attack, EV_Defense, EV_SpecialAttack, EV_SpecialDefense, EV_Speed, Name_Pokemon)" +
      "VALUES (@name, @hp, @attack, @defense, @specialAttack, @specialDefense, @speed, @pokemon);";

    private const string InsertMovesetNatureQuery =
      "INSERT INTO MovesetNature (Nature, id_Moveset) VALUES (@nature, @moveset);";

    private const string InsertMoveCollectionQuery =
      "INSERT INTO MoveCollection (id_Moveset) VALUES (@moveset);";

    private const string InsertMoveToMoveCollectionQuery =
      "INSERT INTO MoveToMoveCollection (name_Move, Move_FullName, id_MoveCollection) VALUES (@move, @fullName, @moveCollection)";

    private async Task<long> insertMoveset(Moveset moveset, string pokemonOwnerName)
    {
      var statement = await _database.PrepareStatementAsync(InsertMovesetQuery);
      statement.BindTextParameterWithName("@name", moveset.Name);
      statement.BindIntParameterWithName("@hp", moveset.EVSpread.HP);
      statement.BindIntParameterWithName("@attack", moveset.EVSpread.Attack);
      statement.BindIntParameterWithName("@defense", moveset.EVSpread.Defense);
      statement.BindIntParameterWithName("@specialAttack", moveset.EVSpread.SpecialAttack);
      statement.BindIntParameterWithName("@specialDefense", moveset.EVSpread.SpecialDefense);
      statement.BindIntParameterWithName("@speed", moveset.EVSpread.Speed);
      statement.BindTextParameterWithName("@pokemon", pokemonOwnerName);

      await statement.StepAsync();

      long key = _database.GetLastInsertedRowId();

      foreach (var desc in moveset.Description)
      {
        await insertTextElement(desc, key, OwnerType.Moveset, ElementType.Description);
      }

      await insertMovesetNatureConnections(moveset, key);

      await insertMovesetItemConnections(moveset, key);

      await insertMovesetAbilityConnections(moveset, key);

      await insertMoveCollections(moveset, key);

      return key;
    }

    private async Task<long> insertMovesetNatureConnections(Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      foreach (var nature in moveset.Natures)
      {
        var statement = await _database.PrepareStatementAsync(InsertMovesetNatureQuery);
        statement.BindIntParameterWithName("@nature", (int)nature);
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

    private async Task<long> insertMoveCollections(Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      foreach (var moveCollection in moveset.Moves)
      {
        var statement = await _database.PrepareStatementAsync(InsertMoveCollectionQuery);
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

        await insertMoveToMoveCollectionRelationships(moveCollection, lastKey);
      }

      return lastKey;
    }

    private async Task<long> insertMoveToMoveCollectionRelationships(IEnumerable<Move> moveCollection, long moveCollectionId)
    {
      long lastKey = 0;

      foreach (var move in moveCollection)
      {
        var shortName = move.Name;

        // sometimes we get specific variants of hidden power instead of hidden power
        // kill em softly
        if (shortName.Contains("Hidden Power")) shortName = "Hidden Power";

        var statement = await _database.PrepareStatementAsync(InsertMoveToMoveCollectionQuery);
        statement.BindTextParameterWithName("@move", shortName);
        statement.BindTextParameterWithName("@fullName", move.Name);
        statement.BindInt64ParameterWithName("@moveCollection", moveCollectionId);

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
