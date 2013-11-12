using Schmogon.Data.Abilities;
using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class PokemonAbilitySelectedMessage : InitMessageBase
  {
    public Ability Ability { get; private set; }

    public PokemonAbilitySelectedMessage(Ability ability) :
      this()
    {
      Ability = ability;
    }

    public PokemonAbilitySelectedMessage()
    {
    }
  }
}
