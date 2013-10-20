namespace Schmogon.Data.Moves
{
  public class Move : ISearchItem
  {
    public Move(string name, string desc, string pageLocation)
    {
      PageLocation = pageLocation;
      Description = desc;
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
