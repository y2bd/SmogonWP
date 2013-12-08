using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.Model
{
  public enum DefenseType
  {
    StrongDefense,
    WeakDefense,
    VeryStrongDefense,
    VeryWeakDefense,
    FullDefense
  }

  public class DefenseTypeGroup : ObservableCollection<TypeItemViewModel>
  {
    public DefenseType Key { get; private set; }

    public string DisplayName
    {
      get
      {
        switch (Key)
        {
          case DefenseType.StrongDefense:
            return "STRONG DEFENSE AGAINST";
          case DefenseType.WeakDefense:
            return "WEAK DEFENSE AGAINST";
          case DefenseType.VeryStrongDefense:
            return "VERY STRONG DEFENSE AGAINST";
          case DefenseType.VeryWeakDefense:
            return "VERY WEAK DEFENSE AGAINST";
          case DefenseType.FullDefense:
            return "TAKES NO DAMAGE FROM";
          default:
            return string.Empty;
        }
      }
    }

    public DefenseTypeGroup(DefenseType key)
    {
      Key = key;
    }

    public DefenseTypeGroup(IEnumerable<TypeItemViewModel> collection, DefenseType key)
      : base(collection)
    {
      Key = key;
    }
  }
}
