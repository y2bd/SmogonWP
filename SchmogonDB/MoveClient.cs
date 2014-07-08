using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Population;
using Type = SchmogonDB.Model.Types.Type;

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
               m.Target,
               m.Type 
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

    private const string FetchPokemonMovesQuery =
      @"SELECT mtp.Name_Move, 
               mtp.RelationDescription,
               om.Type
        FROM Pokemon p
        INNER JOIN MoveToPokemon mtp ON mtp.Name_Pokemon = p.Name
        INNER JOIN Move om ON om.Name = mtp.Name_Move
        WHERE p.Name = @name";

    private IEnumerable<Move> _moveCache;

    public async Task<IEnumerable<Move>> FetchMoveSearchDataAsync()
    {
      ensureDatabaseInitialized();

      return _moveCache ?? (_moveCache = await fetchMoveSearchData());
    }

    private async Task<IEnumerable<Move>> fetchMoveSearchData()
    {
      var moves = new List<Move>();

      var statement = await _database.PrepareStatementAsync(FetchMoveSearchDataQuery);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);
        var type = (Type) statement.GetIntAt(2);

        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.MoveBasePath);

        moves.Add(new Move(name, desc, pageLocation, type));
      }
      
      return moves;
    }

    public async Task<MoveData> FetchMoveDataAsync(Move move)
    {
      // that dang hidden power
      var name = move.Name.Contains("Hidden Power") ? Utilities.ConsolidateHiddenPower(move.Name) : move.Name;
      var desc = await FetchTextElementsAsync(name, OwnerType.Move, ElementType.Description);
      var comp = await FetchTextElementsAsync(name, OwnerType.Move, ElementType.Competitive);
      var stat = await fetchMoveStats(move);
      var relm = await fetchRelatedMoves(move);
      
      return new MoveData(
        name,
        stat,
        desc,
        comp,
        relm
        );
    }

    private async Task<MoveStats> fetchMoveStats(Move move)
    {
      // that dang hidden power
      var name = move.Name.Contains("Hidden Power") ? Utilities.ConsolidateHiddenPower(move.Name) : move.Name;

      var statement = await _database.PrepareStatementAsync(FetchMoveStatsQuery);
      statement.BindTextParameterWithName("@name", name);

      statement.StepSync();
      
      return new MoveStats
      (
        (Type)statement.GetIntAt(6), 
        statement.GetTextAt(0), 
        statement.GetTextAt(1), 
        statement.GetTextAt(2),
        statement.GetTextAt(3), 
        statement.GetTextAt(4), 
        statement.GetTextAt(5)
      );
    }

    private async Task<IEnumerable<Move>> fetchRelatedMoves(Move move)
    {
      // that dang hidden power
      var moveName = move.Name.Contains("Hidden Power") ? Utilities.ConsolidateHiddenPower(move.Name) : move.Name;

      var relatedMoves = new List<Move>();

      var statement = await _database.PrepareStatementAsync(FetchRelatedMovesQuery);
      statement.BindTextParameterWithName("@name", moveName);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);
        var type = (Type) statement.GetIntAt(2);
        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.MoveBasePath);

        relatedMoves.Add(new Move(name, desc, pageLocation, type));
      }

      return relatedMoves;
    }

    private async Task<IEnumerable<Move>> fetchPokemonMoves(Pokemon pokemon)
    {
      var pokemonMoves = new List<Move>();

      var statement = await _database.PrepareStatementAsync(FetchPokemonMovesQuery);
      statement.BindTextParameterWithName("@name", pokemon.Name);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);
        var type = (Type)statement.GetIntAt(2);
        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.MoveBasePath);

        pokemonMoves.Add(new Move(name, desc, pageLocation, type));
      }

      return pokemonMoves;
    }
  }
}
