namespace SchmogonDB.Model.Items
{
  public class Item : ISearchItem
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public string PageLocation { get; set; }

    public Item(string name, string description, string pageLocation)
    {
      Name = name;
      Description = description;
      PageLocation = pageLocation;
    }

    public Item() {}

    public override string ToString()
    {
      return Name;
    }
  }
}
