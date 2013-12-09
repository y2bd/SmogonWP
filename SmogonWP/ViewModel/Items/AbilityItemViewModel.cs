using GalaSoft.MvvmLight;
using SchmogonDB.Model;
using SchmogonDB.Model.Abilities;

namespace SmogonWP.ViewModel.Items
{
  public class AbilityItemViewModel : ViewModelBase, ISearchItem
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

    private int _rowIndex;
    public int RowIndex
    {
      get
      {
        return _rowIndex;
      }
      set
      {
        if (_rowIndex != value)
        {
          _rowIndex = value;
          RaisePropertyChanged(() => RowIndex);
        }
      }
    }			

    #endregion

    public AbilityItemViewModel(Ability ability)
    {
      Ability = ability;
    }

    public AbilityItemViewModel(Ability ability, int rowIndex)
      : this(ability)
    {
      _rowIndex = rowIndex;
    }
  }
}
