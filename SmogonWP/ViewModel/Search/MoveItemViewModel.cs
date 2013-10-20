using GalaSoft.MvvmLight;
using Schmogon.Data.Moves;

namespace SmogonWP.ViewModel.Search
{
  public class MoveItemViewModel : ViewModelBase
  {
    private readonly Move _move;

    #region props

    public string Name
    {
      get
      {
        return _move.Name;
      }
    }

    public string Description
    {
      get
      {
        return _move.Description;
      }
    }

    public string PageLocation
    {
      get
      {
        return _move.PageLocation;
      }
    }

    #endregion

    public MoveItemViewModel(Move move)
    {
      _move = move;
    }
  }
}
