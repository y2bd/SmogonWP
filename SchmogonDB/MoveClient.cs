using Schmogon.Data.Moves;
using SchmogonDB.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB.Population;
using Type = Schmogon.Data.Types.Type;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string FetchMoveSearchDataQuery =
      @"SELECT m.Name, m.ShortDescription, m.Type FROM Move m";

    private const string FetchMoveStatsQuery =
      @"SELECT m.Power, 
               m.Accuracy, 
               m.PP, 
               m.Priority, 
               m.Damage,
               m.Target 
        FROM Move m
        WHERE m.Name = @name";

    private const string FetchRelatedMovesQuery =
      @"SELECT mtm.MoveTo_FullName, 
               mtm.RelationDescription, 
               om.Type 
        FROM Move m
        INNER JOIN MoveToMove mtm ON mtm.Name_MoveFrom = m.Name
        INNER JOIN Move om ON om.Name = mtm.Name_MoveTo
        WHERE m.Name = @name";

    private IEnumerable<TypedMove> _moveCache;

    public async Task<IEnumerable<TypedMove>> FetchMoveSearchDataAsync()
    {
      ensureDatabaseInitialized();

      return _moveCache ?? (_moveCache = await fetchMoveSearchData());
    }

    private async Task<IEnumerable<TypedMove>> fetchMoveSearchData()
    {
      var moves = new List<TypedMove>();

      var statement = await _database.PrepareStatementAsync(FetchMoveSearchDataQuery);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);
        var type = (Type) statement.GetIntAt(2);

        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.MoveBasePath);

        moves.Add(new TypedMove(name, desc, pageLocation, type));
      }
      
      return moves;
    }

    public async Task<MoveData> FetchMoveDataAsync(TypedMove move)
    {
      var desc = await fetchTextElements(move.Name, OwnerType.Move, ElementType.Description);
      var comp = await fetchTextElements(move.Name, OwnerType.Move, ElementType.Competitive);
      var stat = await fetchMoveStats(move);
      var relm = await fetchRelatedMoves(move);
      
      return new MoveData(
        move.Name,
        stat,
        desc,
        comp,
        relm
        );
    }

    private async Task<MoveStats> fetchMoveStats(TypedMove move)
    {
      var typeString = Enum.GetName(typeof(Type), move.Type);

      var statement = await _database.PrepareStatementAsync(FetchMoveStatsQuery);
      statement.BindTextParameterWithName("@name", move.Name);

      statement.StepSync();
      return new MoveStats
      (
        typeString, 
        statement.GetTextAt(0), 
        statement.GetTextAt(1), 
        statement.GetTextAt(2),
        statement.GetTextAt(3), 
        statement.GetTextAt(4), 
        statement.GetTextAt(5)
      );
    }

    private async Task<IEnumerable<TypedMove>> fetchRelatedMoves(TypedMove move)
    {
      var relatedMoves = new List<TypedMove>();

      var statement = await _database.PrepareStatementAsync(FetchRelatedMovesQuery);
      statement.BindTextParameterWithName("@name", move.Name);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);
        var type = (Type) statement.GetIntAt(2);
        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.MoveBasePath);

        relatedMoves.Add(new TypedMove(name, desc, pageLocation, type));
      }

      return relatedMoves;
    }
  }
}
