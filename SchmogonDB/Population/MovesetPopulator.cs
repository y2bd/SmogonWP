using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
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

    private async Task<long> insertMoveset(Database database, Moveset moveset, string pokemonOwnerName)
    {
      var statement = await database.PrepareStatementAsync(InsertMovesetQuery);
      statement.BindTextParameterWithName("@name", moveset.Name);
      statement.BindIntParameterWithName("@hp", moveset.EVSpread.HP);
      statement.BindIntParameterWithName("@attack", moveset.EVSpread.Attack);
      statement.BindIntParameterWithName("@defense", moveset.EVSpread.Defense);
      statement.BindIntParameterWithName("@specialAttack", moveset.EVSpread.SpecialAttack);
      statement.BindIntParameterWithName("@specialDefense", moveset.EVSpread.SpecialDefense);
      statement.BindIntParameterWithName("@speed", moveset.EVSpread.Speed);
      statement.BindTextParameterWithName("@pokemon", pokemonOwnerName);

      await statement.StepAsync();

      long key = database.GetLastInsertedRowId();

      foreach (var desc in moveset.Description)
      {
        await insertTextElement(database, desc, key, OwnerType.Moveset, ElementType.Description);
      }

      await insertMovesetNatureConnections(database, moveset, key);

      await insertMovesetItemConnections(database, moveset, key);

      await insertMovesetAbilityConnections(database, moveset, key);

      await insertMoveCollections(database, moveset, key);

      return key;
    }

    private async Task<long> insertMovesetNatureConnections(Database database, Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      foreach (var nature in moveset.Natures)
      {
        var statement = await database.PrepareStatementAsync(InsertMovesetNatureQuery);
        statement.BindIntParameterWithName("@nature", (int)nature);
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

    private async Task<long> insertMoveCollections(Database database, Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      foreach (var moveCollection in moveset.Moves)
      {
        var statement = await database.PrepareStatementAsync(InsertMoveCollectionQuery);
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

        await insertMoveToMoveCollectionRelationships(database, moveCollection, lastKey);
      }

      return lastKey;
    }

    private async Task<long> insertMoveToMoveCollectionRelationships(Database database, IEnumerable<Move> moveCollection, long moveCollectionId)
    {
      long lastKey = 0;

      foreach (var move in moveCollection)
      {
        var shortName = move.Name;

        // sometimes we get specific variants of hidden power instead of hidden power
        // kill em softly
        if (shortName.Contains("Hidden Power")) shortName = "Hidden Power";

        var statement = await database.PrepareStatementAsync(InsertMoveToMoveCollectionQuery);
        statement.BindTextParameterWithName("@move", shortName);
        statement.BindTextParameterWithName("@fullName", move.Name);
        statement.BindInt64ParameterWithName("@moveCollection", moveCollectionId);

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
