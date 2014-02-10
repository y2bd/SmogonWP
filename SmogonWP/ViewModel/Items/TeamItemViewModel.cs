using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Teams;

namespace SmogonWP.ViewModel.Items
{
  public class TeamItemViewModel : ViewModelBase
  {
    private Team _team;
    public Team Team
    {
      get
      {
        return _team;
      }
      private set
      {
        if (_team != value)
        {
          _team = value;
          RaisePropertyChanged(() => Team);
        }
      }
    }

    public string Name
    {
      get
      {
        return Team.Name.ToLower();
      }
    }

    public int MemberCount
    {
      get
      {
        return Team.TeamMembers.Count;
      }
    }

    public string TeamType
    {
      get
      {
        var name = Enum.GetName(typeof(TeamType), Team.TeamType);
        return name != null ? name.ToLower() : string.Empty;
      }
    }

    private List<TypeItemViewModel> _types;
    public List<TypeItemViewModel> Types
    {
      get
      {
        return _types ??
               (_types =
                 Team.TeamMembers.Select(m => m.Pokemon.Types.Last()).Select(t => new TypeItemViewModel(t)).ToList());
      }
    }

    public TeamItemViewModel(Team team)
    {
      Team = team;
    }
  }
}
