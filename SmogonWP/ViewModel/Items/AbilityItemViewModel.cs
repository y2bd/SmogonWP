using GalaSoft.MvvmLight;
using Schmogon.Data.Abilities;

namespace SmogonWP.ViewModel.Items
{
  public class AbilityItemViewModel : ViewModelBase
  {
    internal readonly Ability Ability;

    #region props

    public string Name
    {
      get
      {
        return Ability.Name.ToLowerInvariant();
      }
    }

    public string Description
    {
      get
      {
        return Ability.Description.ToLowerInvariant().Trim(new[] { '.' });
      }
    }

    public string PageLocation
    {
      get
      {
        return Ability.PageLocation;
      }
    }

    #endregion

    public AbilityItemViewModel(Ability ability)
    {
      Ability = ability;
    }
  }
}
