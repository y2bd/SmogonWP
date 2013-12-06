using GalaSoft.MvvmLight;
using SchmogonDB.Model;
using SchmogonDB.Model.Moves;

namespace SmogonWP.ViewModel.Items
{
  public class MoveItemViewModel : ViewModelBase, ISearchItem
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

    private TypeItemViewModel _type;
    public TypeItemViewModel Type
    {
      get
      {
        return _type;
      }
      set
      {
        if (_type != value)
        {
          _type = value;
          RaisePropertyChanged(() => Type);
        }
      }
    }			
    
    #endregion
    
    public MoveItemViewModel(Move move)
    {
      Move = move;

      _type = new TypeItemViewModel(move.Type);
    }
  }
}
