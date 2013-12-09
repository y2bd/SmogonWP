using System.Collections.Generic;
using System.Windows.Media;
using SchmogonDB.Model.Moves;

namespace SmogonWP.ViewModel.Items
{
  public class GroupedMoveItemViewModel : MoveItemViewModel
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

    public SolidColorBrush BackgroundBrush
    {
      get
      {
        return BackgroundBrushes[_groupIndex];
      }
    }

    public GroupedMoveItemViewModel(Move move, int groupIndex, int rowIndex) : base(move)
    {
      _groupIndex = groupIndex;
      _rowIndex = rowIndex;
    }
  }
}
