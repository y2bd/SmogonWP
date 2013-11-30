using System.Collections.Generic;
using Schmogon.Model.Text;

namespace Schmogon.Data.Moves
{
  public class MoveData : IDataItem
  {
    public MoveData(string name, MoveStats stats, IEnumerable<ITextElement> description, IEnumerable<ITextElement> competitive, IEnumerable<Move> relatedMoves)
    {
      RelatedMoves = relatedMoves;
      Competitive = competitive;
      Description = description;
      Stats = stats;
      Name = name;
    }

    public string Name { get; private set; }
    
    public MoveStats Stats { get; private set; }

    public IEnumerable<ITextElement> Description { get; private set; }

    public IEnumerable<ITextElement> Competitive { get; private set; }

    public IEnumerable<Move> RelatedMoves { get; private set; }
  }
}
