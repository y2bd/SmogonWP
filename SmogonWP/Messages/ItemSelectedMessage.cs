using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class ItemSelectedMessage<T> : InitMessageBase
  {
    public T Item { get; private set; }

    public ItemSelectedMessage(T item)
      : this()
    {
      Item = item;
    }

    public ItemSelectedMessage()
    {}
  }
}
