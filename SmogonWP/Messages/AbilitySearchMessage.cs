using Schmogon.Data.Abilities;
using SmogonWP.Services.Messaging;

namespace SmogonWP.Messages
{
  public class AbilitySearchMessage : InitMessageBase
  {
    public Ability Ability { get; private set; }

    public AbilitySearchMessage(Ability ability)
      : this()
    {
      Ability = ability;
    }

    public AbilitySearchMessage() { }
  }
}
