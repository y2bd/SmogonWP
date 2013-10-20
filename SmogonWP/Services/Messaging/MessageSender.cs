using GalaSoft.MvvmLight.Messaging;

namespace SmogonWP.Services.Messaging
{
  public class MessageSender<T> where T : InitMessageBase
  {
    private T _message;

    public MessageSender()
    {
      // Send back item when a constructor asks for it
      Messenger.Default.Register<T>(this, message =>
      {
        if (_message != null && _message != message && !_message.Received)
        {
          // Send back original message
          Messenger.Default.Send(_message);
        }
      });
    }

    public void SendMessage(T message)
    {
      // Store value
      _message = message;

      // Try and send message
      Messenger.Default.Send(message);
    }
  }
}
