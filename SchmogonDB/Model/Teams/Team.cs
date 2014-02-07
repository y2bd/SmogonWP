using System.Collections.Generic;

namespace SchmogonDB.Model.Teams
{
  public class Team
  {
    public string Name { get; set; }
    
    public TeamType TeamType { get; set; }

    public IEnumerable<TeamMember> TeamMembers { get; set; } 

    public Team()
    {
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
