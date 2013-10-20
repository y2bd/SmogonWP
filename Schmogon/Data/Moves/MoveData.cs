using System.Collections.Generic;

namespace Schmogon.Data.Moves
{
  public class MoveData
  {
    internal MoveData(string name, MoveStats stats, string description, string competitive, IEnumerable<Move> relatedMoves)
    {
      RelatedMoves = relatedMoves;
      Competitive = competitive;
      Description = description;
      Stats = stats;
      Name = name;
    }

    public string Name { get; private set; }
    
    public MoveStats Stats { get; private set; }

    public string Description { get; private set; }

    public string Competitive { get; private set; }

    public IEnumerable<Move> RelatedMoves { get; private set; }
  }
}
