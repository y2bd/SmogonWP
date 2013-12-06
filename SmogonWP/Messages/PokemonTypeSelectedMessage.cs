using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.Messages
{
  public class PokemonTypeSelectedMessage : ItemSelectedMessage<Type>
  {
    public PokemonTypeSelectedMessage(Type item)
      : base(item)
    {

    }

    public PokemonTypeSelectedMessage()
    {
    }
  }
}
