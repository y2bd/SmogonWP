using System.Collections.Generic;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Text;

namespace SmogonWP.ViewModel.Items
{
  public class ItemDataItemViewModel : ViewModelBase
  {
    internal ItemData ItemData;

    #region props

    public string Name
    {
      get { return ItemData.Name.ToLower(); }
    }

    public IEnumerable<ITextElement> Description
    {
      get { return ItemData.Description; }
    }

    public IEnumerable<ITextElement> Competitive
    {
      get { return ItemData.Competitive; }
    }
    
    #endregion props

    public ItemDataItemViewModel(ItemData itemData)
    {
      ItemData = itemData;
    }
  }
}
