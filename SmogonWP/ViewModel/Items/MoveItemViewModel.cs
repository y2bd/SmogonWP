using GalaSoft.MvvmLight;
using Schmogon.Data.Moves;

namespace SmogonWP.ViewModel.Items
{
  public class MoveItemViewModel : ViewModelBase
  {
    internal readonly Move Move;

    #region props

    public string Name
    {
      get
      {
        return Move.Name.ToLowerInvariant();
      }
    }

    public string Description
    {
      get
      {
        return Move.Description.ToLowerInvariant().Trim(new [] {'.'});
      }
    }

    public string PageLocation
    {
      get
      {
        return Move.PageLocation;
      }
    }

    #endregion

    public MoveItemViewModel(Move move)
    {
      Move = move;
    }
  }
}
