using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace SmogonWP.Messages
{
  public class TombstoneMessage<T> : MessageBase
    where T : ViewModelBase
  {
    public TombstoneMessage()
    {}

    public TombstoneMessage(object sender)
      : base(sender)
    {}
  }

  public class RestoreMessage<T> : MessageBase
    where T : ViewModelBase
  {
    public RestoreMessage()
    { }

    public RestoreMessage(object sender)
      : base(sender)
    { }
  }
}
