﻿using System.Collections.Generic;
using Schmogon.Data.Abilities;
using Schmogon.Model.Text;

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
