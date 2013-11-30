using Schmogon.Data.Moves;
using Type = Schmogon.Data.Types.Type;

namespace SchmogonTest
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
