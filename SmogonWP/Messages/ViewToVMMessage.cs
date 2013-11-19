using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace SmogonWP.Messages
{
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
