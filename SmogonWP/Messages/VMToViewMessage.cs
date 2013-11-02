using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace SmogonWP.Messages
{
  public class VmToViewMessage<T, TV> : GenericMessage<T>
    where TV : PhoneApplicationPage
  {
    public VmToViewMessage(T content)
      : base(content)
    {
    }

    public VmToViewMessage(object sender, T content)
      : base(sender, content)
    {
    }

    public VmToViewMessage(object sender, TV target, T content)
      : base(sender, target, content)
    {
    }
  }

}
