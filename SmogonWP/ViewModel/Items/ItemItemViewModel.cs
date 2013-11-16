using GalaSoft.MvvmLight;
using Schmogon.Data.Items;

namespace SmogonWP.ViewModel.Items
{
  public class ItemItemViewModel : ViewModelBase
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

    public ItemItemViewModel(Item item)
    {
      Item = item;
    }
  }
}
