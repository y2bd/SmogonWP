namespace SchmogonDB.Model.Abilities
{
  public class Ability : ISearchItem
  {
    public Ability(string name, string description, string pageLocation)
    {
      Name = name;
      Description = description;
      PageLocation = pageLocation;
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
