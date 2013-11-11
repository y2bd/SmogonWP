using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Schmogon.Data.Pokemon;

namespace SmogonWP.ViewModel.Items
{
  public class PokemonItemViewModel : ViewModelBase
  {
    public Pokemon Pokemon { get; private set; }

    public string Name
    {
      get
      {
        return Pokemon.Name.ToLowerInvariant();
      }
    }

    public IEnumerable<TypeItemViewModel> Types { get; private set; }

    public PokemonItemViewModel(Pokemon pokemon)
    {
      Pokemon = pokemon;

      Types = pokemon.Types.Select(t => new TypeItemViewModel(t));
    }
  }
}