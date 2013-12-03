using Schmogon.Data.Moves;
using Schmogon.Data.Types;

namespace SchmogonDB.Model
{
  public class TypedMove : Move
  {
    public Type Type { get; private set; }

    public TypedMove(string name, string description, string pageLocation, Type type)
      : base(name, description, pageLocation)
    {
      Type = type;
    }
  }
}
