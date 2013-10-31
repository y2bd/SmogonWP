namespace Schmogon.Data.Abilities
{
  public class AbilityData
  {
    public AbilityData(string name, string description, string competitive)
    {
      Name = name;
      Description = description;
      Competitive = competitive;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Competitive { get; private set; }
  }
}
