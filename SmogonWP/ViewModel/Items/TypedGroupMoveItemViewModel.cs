using System.Collections.Generic;
using System.Windows.Media;
using SchmogonDB.Model;

namespace SmogonWP.ViewModel.Items
{
  public class TypedGroupMoveItemViewModel : TypedMoveItemViewModel
  {
    private static readonly IReadOnlyList<SolidColorBrush> BackgroundBrushes = new List<SolidColorBrush>
    {
      new SolidColorBrush(Color.FromArgb(255, 216, 0, 115)),
      new SolidColorBrush(Color.FromArgb(255, 0, 171, 169)),
      new SolidColorBrush(Color.FromArgb(255, 118, 96, 138)),
      new SolidColorBrush(Color.FromArgb(255, 240, 163, 10))
    };

    private int _groupIndex;
    public int GroupIndex
    {
      get
      {
        return _groupIndex;
      }
      set
      {
        if (_groupIndex != value)
        {
          _groupIndex = value;
          RaisePropertyChanged(() => GroupIndex);
        }
      }
    }

    public SolidColorBrush BackgroundBrush
    {
      get
      {
        return BackgroundBrushes[_groupIndex];
      }
    }

    public TypedGroupMoveItemViewModel(TypedMove move, int groupIndex)
      : base(move)
    {
      _groupIndex = groupIndex;
    }
  }
}
