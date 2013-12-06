using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SchmogonDB.Model;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
  {
    private const string InsertMovesQuery = 
      "INSERT INTO Move VALUES (@name, @shortdesc, @type, @power, @accuracy, @pp, @priorty, @damage, @target);";

    private const string InsertMoveToMoveQuery =
      "INSERT INTO MoveToMove (RelationDescription, Name_MoveFrom, Name_MoveTo, MoveTo_FullName) VALUES (@reldesc, @moveFrom, @moveTo, @fullName);";

    private const string InsertMoveToPokemonQuery =
      "INSERT INTO MoveToPokemon (RelationDescription, Name_Pokemon, Name_Move) VALUES (@reldesc, @pokemon, @move);";

    private async Task<string> insertMoveData(Database database, Move move, MoveData moveData)
    {
      //Type moveType;
      //if (!Enum.TryParse(moveData.Stats.Type, true, out moveType)) return null;

      var statement = await database.PrepareStatementAsync(InsertMovesQuery);
      statement.BindTextParameterWithName("@name", moveData.Name);
      statement.BindTextParameterWithName("@shortdesc", move.Description);
      statement.BindIntParameterWithName("@type", (int)move.Type);
      statement.BindTextParameterWithName("@power", moveData.Stats.Power);
      statement.BindTextParameterWithName("@accuracy", moveData.Stats.Accuracy);
      statement.BindTextParameterWithName("@pp", moveData.Stats.PP);
      statement.BindTextParameterWithName("@priorty", moveData.Stats.Priority);
      statement.BindTextParameterWithName("@damage", moveData.Stats.Damage);
      statement.BindTextParameterWithName("@target", moveData.Stats.Target);

      await statement.StepAsync();
      
      foreach (var desc in moveData.Description)
      {
        await insertTextElement(database, desc, moveData.Name, OwnerType.Move, ElementType.Description);
      }

      foreach (var comp in moveData.Competitive)
      {
        await insertTextElement(database, comp, moveData.Name, OwnerType.Move, ElementType.Competitive);
      }

      return moveData.Name;
    }

    private async Task<string> insertRelatedMoveConnections(Database database, MoveData moveData)
    {
      string lastKey = null;

      foreach (var relatedMove in moveData.RelatedMoves)
      {
        var shortName = relatedMove.Name;

        // sometimes they list explicit hidden powers, that doesn't help us
        if (shortName.Contains("Hidden Power")) shortName = "Hidden Power";

        var statement = await database.PrepareStatementAsync(InsertMoveToMoveQuery);
        statement.BindTextParameterWithName("@reldesc", relatedMove.Description);
        statement.BindTextParameterWithName("@moveFrom", moveData.Name);
        statement.BindTextParameterWithName("@moveTo", shortName);
        statement.BindTextParameterWithName("@fullName", relatedMove.Name);

        await statement.StepAsync();

        lastKey = relatedMove.Name;
      }

      return lastKey;
    }

    private async Task<long> insertPokemonMovesConnections(Database database, PokemonData pokemon)
    {
      long lastKey = 0;

      foreach (var move in pokemon.Moves)
      {
        var statement = await database.PrepareStatementAsync(InsertMoveToPokemonQuery);
        statement.BindTextParameterWithName("@reldesc", move.Description);
        statement.BindTextParameterWithName("@pokemon", pokemon.Name);
        statement.BindTextParameterWithName("@move", move.Name);

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
