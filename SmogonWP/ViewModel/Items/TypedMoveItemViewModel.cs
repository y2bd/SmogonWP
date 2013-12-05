using SchmogonDB.Model;

namespace SmogonWP.ViewModel.Items
{
  public class TypedMoveItemViewModel : MoveItemViewModel
  {
    internal readonly TypedMove TypedMove;

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

    public TypedMoveItemViewModel(TypedMove move)
      : base (move)
    {
      TypedMove = move;

      Type = new TypeItemViewModel(TypedMove.Type);
    }
  }
}
