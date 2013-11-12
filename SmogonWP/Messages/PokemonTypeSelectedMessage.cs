using SmogonWP.Services.Messaging;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.Messages
{
  public class PokemonTypeSelectedMessage : InitMessageBase
  {
    public Type Type { get; private set; }

    public PokemonTypeSelectedMessage(Type type)
      : this()
    {
      Type = type;
    }

    public PokemonTypeSelectedMessage()
    {
    }
  }
}
