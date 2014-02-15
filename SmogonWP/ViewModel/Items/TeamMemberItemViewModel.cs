using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Teams;

namespace SmogonWP.ViewModel.Items
{
  public class TeamMemberItemViewModel : ViewModelBase
  {
    private TeamMember _teamMember;
    public TeamMember TeamMember
    {
      get
      {
        return _teamMember;
      }
      set
      {
        if (_teamMember != value)
        {
          _teamMember = value;
          RaisePropertyChanged(() => TeamMember);
          RaisePropertyChanged(() => Name);
          RaisePropertyChanged(() => Types);
          RaisePropertyChanged(() => PrimaryType);
        }
      }
    }

    public string Name
    {
      get
      {
        return TeamMember.Pokemon.Name.ToLower();
      }
    }

    public IEnumerable<TypeItemViewModel> Types
    {
      get
      {
        return TeamMember.Pokemon.Types.Select(t => new TypeItemViewModel(t));
      }
    }

    public TypeItemViewModel PrimaryType
    {
      get
      {
        return new TypeItemViewModel(TeamMember.Pokemon.Types.First());
      }
    }
    
    private ImageSource _sprite;
    public ImageSource Sprite
    {
      get
      {
        return _sprite;
      }
      set
      {
        if (_sprite != value)
        {
          _sprite = value;
          RaisePropertyChanged(() => Sprite);
        }
      }
    }	

    public TeamMemberItemViewModel(TeamMember member)
    {
      TeamMember = member;
    }
  }
}
