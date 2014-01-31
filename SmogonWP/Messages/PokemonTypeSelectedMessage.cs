using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.Messages
{
  public class PokemonTypeSelectedMessage : ItemSelectedMessage<Type>
  {
    public Type? SecondType { get; private set; }

    public PokemonTypeSelectedMessage(Type type, Type? secondaryType)
      : base(type)
    {
      SecondType = secondaryType;
    }

    public PokemonTypeSelectedMessage(Type type)
      : this(type, null)
    {

    }

    public PokemonTypeSelectedMessage()
    {
    }
  }
}
