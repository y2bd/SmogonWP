using Schmogon.Data.Types;
using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class MoveTypeSelectedMessage : InitMessageBase
  {
    public Type Type { get; private set; }

    public MoveTypeSelectedMessage(Type type)
      : this()
    {
      Type = type;
    }

    public MoveTypeSelectedMessage()
    {
    }
  }
}