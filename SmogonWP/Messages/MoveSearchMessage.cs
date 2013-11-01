using Schmogon.Data.Moves;
using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class MoveSearchMessage : InitMessageBase
  {
    public Move Move { get; private set; }

    public MoveSearchMessage(Move move) : this()
    {
      Move = move;
    }

    public MoveSearchMessage() {}
  }
}
