using Schmogon.Data.Pokemon;
using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class PokemonSearchMessage : InitMessageBase
  {
    public Pokemon Pokemon { get; private set; }

    public PokemonSearchMessage(Pokemon pokemon)
      : this()
    {
      Pokemon = pokemon;
    }

    public PokemonSearchMessage() { } 
  }
}