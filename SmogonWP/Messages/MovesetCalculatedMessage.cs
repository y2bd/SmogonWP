using SmogonWP.ViewModel.Items;

namespace SmogonWP.Messages
{
  public class MovesetCalculatedMessage : ItemSearchedMessage<MovesetItemViewModel>
  {
    public MovesetCalculatedMessage(MovesetItemViewModel msivm)
      : base(msivm)
    {
    }

    public MovesetCalculatedMessage()
    {
    }
  }
}
