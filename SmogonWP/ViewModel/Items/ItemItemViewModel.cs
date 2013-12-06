using GalaSoft.MvvmLight;
using SchmogonDB.Model;
using SchmogonDB.Model.Items;

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

    public ItemItemViewModel(Item item)
    {
      Item = item;
    }

    public ItemItemViewModel(Item item, int rowIndex)
      : this(item)
    {
      _rowIndex = rowIndex;
    }
  }
}
