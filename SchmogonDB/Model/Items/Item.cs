namespace SchmogonDB.Model.Items
{
  public class Item : ISearchItem
  {
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string PageLocation { get; private set; }

    public Item(string name, string description, string pageLocation)
    {
      Name = name;
      Description = description;
      PageLocation = pageLocation;
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
