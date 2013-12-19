using SchmogonDB.Model.Types;

namespace SchmogonDB.Model.Moves
{
  public class Move : ISearchItem
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public string PageLocation { get; set; }

    public Type Type { get; set; }

    public Move(string name, string description, string pageLocation, Type type)
    {
      Name = name;
      Description = description;
      PageLocation = pageLocation;
      Type = type;
    }

    public Move()
    {
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
