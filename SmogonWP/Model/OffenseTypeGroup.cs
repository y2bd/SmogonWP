using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.Model
{
  public enum OffenseType
  {
    SuperEffective,
    NotVeryEffective,
    NoEffect
  }

  public class OffenseTypeGroup : ObservableCollection<TypeItemViewModel>
  {
    public OffenseType Key { get; private set; }

    public string DisplayName
    {
      get
      {
        switch (Key)
        {
          case OffenseType.SuperEffective:
            return "SUPER EFFECTIVE AGAINST";
          case OffenseType.NotVeryEffective:
            return "NOT VERY EFFECTIVE AGAINST";
          case OffenseType.NoEffect:
            return "NO EFFECT AGAINST";
          default:
            return string.Empty;
        }
      }
    }

    public OffenseTypeGroup(OffenseType key)
    {
      Key = key;
    }

    public OffenseTypeGroup(IEnumerable<TypeItemViewModel> collection, OffenseType key)
      : base (collection)
    {
      Key = key;
    }
  }
}
