using Schmogon.Data.Abilities;

namespace SmogonWP.ViewModel.Items
{
  public class AbilityDataItemViewModel
  {
    internal AbilityData AbilityData;

    #region props

    public string Name
    {
      get { return AbilityData.Name.ToLower(); }
    }

    public string Description
    {
      get { return AbilityData.Description; }
    }

    public string Competitive
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
