using Newtonsoft.Json;

namespace Schmogon.Data.Moves
{
  public class Move : ISearchItem
  {
    public Move(string name, string description, string pageLocation)
    {
      PageLocation = pageLocation;
      Description = description;
      Name = name;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public string PageLocation { get; private set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
