using System.Collections.Generic;

namespace SchmogonDB.Model.Teams
{
  public class Team
  {
    internal int ID { get; set; }

    public string Name { get; set; }
    
    public TeamType TeamType { get; set; }

    public List<TeamMember> TeamMembers { get; set; }

    public Team(string name, TeamType teamType, List<TeamMember> teamMembers)
    {
      Name = name;
      TeamType = teamType;
      TeamMembers = teamMembers;
    }

    public Team()
    {
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
