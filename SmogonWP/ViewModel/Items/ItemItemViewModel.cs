using GalaSoft.MvvmLight;
using Schmogon.Data;
using Schmogon.Data.Items;

namespace SmogonWP.ViewModel.Items
{
  public class ItemItemViewModel : ViewModelBase, ISearchItem
  {
    public Item Item { get; private set; }

    public string Name
    {
      get
      {
        return Item.Name.ToLower();
      }
    }

    public string Description
    {
      get
      {
        return Item.Description.ToLower();
      }
    }

    public string PageLocation
    {
      get
      {
        return Item.PageLocation;
      }
    }

    public ItemItemViewModel(Item item)
    {
      Item = item;
    }
  }
}
