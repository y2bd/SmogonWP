using GalaSoft.MvvmLight;
using SchmogonDB.Model.Types;
using System.Collections.Generic;
using System.Windows.Media;

namespace SmogonWP.ViewModel.Items
{
  public class NavigationItemViewModel : ViewModelBase
  {
    private string _title;
    public string Title
    {
      get
      {
        return _title.ToLower();
      }
      set
      {
        if (_title != value)
        {
          _title = value;
          RaisePropertyChanged(() => Title);
        }
      }
    }

    private string _description;
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        if (_description != value)
        {
          _description = value;
          RaisePropertyChanged(() => Description);
        }
      }
    }

    private string _navigationPath;
    public string NavigationPath
    {
      get
      {
        return _navigationPath;
      }
      set
      {
        if (_navigationPath != value)
        {
          _navigationPath = value;
          RaisePropertyChanged(() => NavigationPath);
        }
      }
    }

    private SolidColorBrush _backgroundBrush;
    public SolidColorBrush BackgroundBrush
    {
      get
      {
        return _backgroundBrush;
      }
      set
      {
        if (_backgroundBrush != value)
        {
          _backgroundBrush = value;
          RaisePropertyChanged(() => BackgroundBrush);
        }
      }
    }

    private string _iconPath;
    public string IconPath
    {
      get
      {
        return _iconPath;
      }
      set
      {
        if (_iconPath != value)
        {
          _iconPath = value;
          RaisePropertyChanged(() => IconPath);
        }
      }
    }
  }

  public static class NavigationItemFactory
  {
    public static IEnumerable<NavigationItemViewModel> MakeStratNavItems()
    {
      return new List<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Pokemon",
          Description = "Search through Pokemon and compose your team",
          NavigationPath = ViewModelLocator.PokemonSearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Water]),
          IconPath = "/Assets/Icons/pokeball2.png"
        },
        new NavigationItemViewModel
        {
          Title = "Moves",
          Description = "Learn about every single move that your Pokemon can battle with",
          NavigationPath = ViewModelLocator.MoveSearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Fire]),
          IconPath = "/Assets/Icons/conflict.png"
        },
        new NavigationItemViewModel
        {
          Title = "Abilities",
          Description = "Explore the various innate powers that your Pokemon possess",
          NavigationPath = ViewModelLocator.AbilitySearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Grass]),
          IconPath = "/Assets/Icons/idea.png"
        },
        new NavigationItemViewModel
        {
          Title = "Items",
          Description = "Shop through various items that can give boosts in battle",
          NavigationPath = ViewModelLocator.ItemSearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Ground]),
          IconPath = "/Assets/Icons/pill.png"
        },
      };
    }

    public static IEnumerable<NavigationItemViewModel> MakeToolNavItems()
    {
      return new List<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Natures",
          Description = "Check out how natures affect your Pokemon's stats",
          NavigationPath = ViewModelLocator.NaturePath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Psychic]),
          IconPath = "/Assets/Icons/emotion.png"
        },
        new NavigationItemViewModel
        {
          Title = "Types",
          Description = "See how typing affects your Pokemon's performance in battle",
          NavigationPath = ViewModelLocator.TypePath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Bug]),
          IconPath = "/Assets/Icons/fire.png"
        },
        new NavigationItemViewModel
        {
          Title = "Stat Calculator",
          Description = "Fine-tune the stats of your perfect Pokemon",
          NavigationPath = ViewModelLocator.StatsPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Dragon]),
          IconPath = "/Assets/Icons/calc.png"
        }
      };
    }
  }
}
