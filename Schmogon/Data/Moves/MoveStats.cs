using System.Dynamic;

namespace Schmogon.Data.Moves
{
  public class MoveStats
  {
    internal MoveStats(string type, string power, string accuracy, string pp, string priority, string damage, string target)
    {
      Damage = damage;
      Target = target;
      Priority = priority;
      PP = pp;
      Accuracy = accuracy;
      Power = power;
      Type = type;
    }

    public string Type { get; private set; }

    /// <summary>
    /// Gets the base power of the move.
    /// A power of '-' means that the base power is not relevant to the move (e.g. the power is decided by other means like Seismic Toss, or the move is a stat modifier)
    /// </summary>
    public string Power { get; private set; }

    /// <summary>
    /// Gets the accuracy of the move. 
    /// An accuracy of '-' means that accuracy is not relevant to the move (e.g. the move always hits, the move is a stat booster)
    /// </summary>
    public string Accuracy { get; private set; }

    public string PP { get; private set; }
    public string Priority { get; private set; }
    public string Damage { get; private set; }
    public string Target { get; private set; }

  }
}
