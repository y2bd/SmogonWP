using System;
using GalaSoft.MvvmLight.Messaging;

namespace SmogonWP.Services.Messaging
{
  public class MessageReceiver<T> where T : InitMessageBase
  {
    private readonly Action<T> _handler;
    private readonly T _message;

    public MessageReceiver(Action<T> handler)
    {
      _handler = handler;

      // Wait for messages
      Messenger.Default.Register<T>(this, message =>
      {
        if (_message != null && _message != message && !message.Received)
        {
          message.Received = true;

          OnMessageReceived(message);
        }
      });
    }

    /// <summary>
    /// Send empty message on instantiation
    /// </summary>
    public MessageReceiver(Action<T> handler, bool sendInitMessage)
      : this(handler)
    {
      if (sendInitMessage)
      {
        _message = Activator.CreateInstance(typeof(T)) as T;

        // Send empty message
        Messenger.Default.Send<T>(_message);
      }
    }

    private void OnMessageReceived(T message)
    {
      if (_handler != null)
        _handler(message);
    }
  }
}
