using SchmogonDB.Model.Types;

namespace SchmogonDB.Model.Moves
{
  public class Move : ISearchItem
  {
    public Move(string name, string description, string pageLocation, Type type)
    {
      PageLocation = pageLocation;
      Description = description;
      Name = name;
      Type = type;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public string PageLocation { get; private set; }

    public Type Type { get; private set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
