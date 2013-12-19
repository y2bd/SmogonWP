namespace SchmogonDB.Model.Abilities
{
  public class Ability : ISearchItem
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public string PageLocation { get; set; }

    public Ability(string name, string description, string pageLocation)
    {
      Name = name;
      Description = description;
      PageLocation = pageLocation;
    }

    public Ability() {}

    public override string ToString()
    {
      return Name;
    }
  }
}
