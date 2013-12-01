using System;
using System.Threading.Tasks;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using Type = Schmogon.Data.Types.Type;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string InsertMovesQuery = 
      "INSERT INTO Move VALUES (@name, @type, @power, @accuracy, @pp, @priorty, @damage, @target);";

    private const string InsertMoveToMoveQuery =
      "INSERT INTO MoveToMove (Name_MoveFrom, Name_MoveTo, MoveTo_FullName) VALUES (@moveFrom, @moveTo, @fullName);";

    private const string InsertMoveToPokemonQuery =
      "INSERT INTO MoveToPokemon (Name_Pokemon, Name_Move) VALUES (@pokemon, @move);";

    private async Task<string> insertMoveData(MoveData move)
    {
      Type moveType;
      if (!Enum.TryParse(move.Stats.Type, true, out moveType)) return null;

      var statement = await _database.PrepareStatementAsync(InsertMovesQuery);
      statement.BindTextParameterWithName("@name", move.Name);
      statement.BindIntParameterWithName("@type", (int)moveType);
      statement.BindTextParameterWithName("@power", move.Stats.Power);
      statement.BindTextParameterWithName("@accuracy", move.Stats.Accuracy);
      statement.BindTextParameterWithName("@pp", move.Stats.PP);
      statement.BindTextParameterWithName("@priorty", move.Stats.Priority);
      statement.BindTextParameterWithName("@damage", move.Stats.Damage);
      statement.BindTextParameterWithName("@target", move.Stats.Target);

      await statement.StepAsync();
      
      foreach (var desc in move.Description)
      {
        await insertTextElement(desc, move.Name, OwnerType.Move, ElementType.Description);
      }

      foreach (var comp in move.Competitive)
      {
        await insertTextElement(comp, move.Name, OwnerType.Move, ElementType.Competitive);
      }

      return move.Name;
    }

    private async Task<string> insertRelatedMoveConnections(MoveData move)
    {
      string lastKey = null;

      foreach (var relatedMove in move.RelatedMoves)
      {
        var shortName = relatedMove.Name;

        // sometimes they list explicit hidden powers, that doesn't help us
        if (shortName.Contains("Hidden Power")) shortName = "Hidden Power";

        var statement = await _database.PrepareStatementAsync(InsertMoveToMoveQuery);
        statement.BindTextParameterWithName("@moveFrom", move.Name);
        statement.BindTextParameterWithName("@moveTo", shortName);
        statement.BindTextParameterWithName("@fullName", relatedMove.Name);

        await statement.StepAsync();

        lastKey = relatedMove.Name;
      }

      return lastKey;
    }

    private async Task<long> insertPokemonMovesConnections(PokemonData pokemon)
    {
      long lastKey = 0;

      foreach (var move in pokemon.Moves)
      {
        var statement = await _database.PrepareStatementAsync(InsertMoveToPokemonQuery);
        statement.BindTextParameterWithName("@pokemon", pokemon.Name);
        statement.BindTextParameterWithName("@move", move.Name);

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
