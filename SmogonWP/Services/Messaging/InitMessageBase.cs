using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;

namespace SmogonWP.Services.Messaging
{
  public abstract class InitMessageBase : MessageBase
  {
    // Used for a send
    public bool Received { get; set; }

    public Dictionary<MessageReceiver<InitMessageBase>, bool> ReceivedBy;

    protected InitMessageBase()
    {
      ReceivedBy = new Dictionary<MessageReceiver<InitMessageBase>, bool>();
    }
  }
}
