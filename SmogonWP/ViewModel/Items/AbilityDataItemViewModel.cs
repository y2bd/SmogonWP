using System.Collections.Generic;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Text;

namespace SmogonWP.ViewModel.Items
{
  public class AbilityDataItemViewModel : ViewModelBase
  {
    internal AbilityData AbilityData;

    #region props

    public string Name
    {
      get { return AbilityData.Name.ToLower(); }
    }

    public IEnumerable<ITextElement> Description
    {
      get { return AbilityData.Description; }
    }

    public IEnumerable<ITextElement> Competitive
    {
      get { return AbilityData.Competitive; }
    }
    
    #endregion props

    public AbilityDataItemViewModel(AbilityData abilityData)
    {
      AbilityData = abilityData;
    }
  }
}
