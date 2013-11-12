using Schmogon.Data.Moves;
using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class PokemonMoveSelectedMessage : InitMessageBase
  {
    public Move Move { get; private set; }

    public PokemonMoveSelectedMessage(Move move)
      : this()
    {
      Move = move;
    }

    public PokemonMoveSelectedMessage() { }
  }
}
