using GalaSoft.MvvmLight;
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

  public class ViewToVmMessage<T, TV> : GenericMessage<T>
    where TV : ViewModelBase
  {
    public ViewToVmMessage(T content)
      : base(content)
    {
    }

    public ViewToVmMessage(object sender, T content)
      : base(sender, content)
    {
    }

    public ViewToVmMessage(object sender, TV target, T content)
      : base(sender, target, content)
    {
    }
  }
}
