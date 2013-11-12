using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.Messages
{
  public class MovesetSelectedMessage : InitMessageBase
  {
    public MovesetItemViewModel MSIVM { get; private set; }

    public MovesetSelectedMessage(MovesetItemViewModel msivm)
      : this()
    {
      MSIVM = msivm;
    }

    public MovesetSelectedMessage()
    {
    }
  }
}
