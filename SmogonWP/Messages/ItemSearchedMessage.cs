using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class ItemSearchedMessage<T> : InitMessageBase
  {
    public T Item { get; private set; }

    public ItemSearchedMessage(T item)
      : this()
    {
      Item = item;
    }

    public ItemSearchedMessage()
    { }
  }
}
