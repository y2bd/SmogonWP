using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.Phone.Speech.VoiceCommands;
using GalaSoft.MvvmLight;
using SmogonWP.Services;
using SmogonWP.ViewModel.Items;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class HubViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;

    #region props 

    private ObservableCollection<NavigationItemViewModel> _stratNavItems;
    public ObservableCollection<NavigationItemViewModel> StratNavItems
    {
      get
      {
        return _stratNavItems;
      }
      set
      {
        if (_stratNavItems != value)
        {
          _stratNavItems = value;
          RaisePropertyChanged(() => StratNavItems);
        }
      }
    }

    private NavigationItemViewModel _selectedStratItem;
    public NavigationItemViewModel SelectedStratItem
    {
      get
      {
        return _selectedStratItem;
      }
      set
      {
        if (_selectedStratItem != value)
        {
          onNavItemSelected(value);

          _selectedStratItem = null;
          RaisePropertyChanged(() => SelectedStratItem);
        }
      }
    }

    private ObservableCollection<NavigationItemViewModel> _toolNavItems;
    public ObservableCollection<NavigationItemViewModel> ToolNavItems
    {
      get
      {
        return _toolNavItems;
      }
      set
      {
        if (_toolNavItems != value)
        {
          _toolNavItems = value;
          RaisePropertyChanged(() => ToolNavItems);
        }
      }
    }

    private NavigationItemViewModel _selectedToolItem;
    public NavigationItemViewModel SelectedToolItem
    {
      get
      {
        return _selectedToolItem;
      }
      set
      {
        if (_selectedToolItem != value)
        {
          onNavItemSelected(value);

          _selectedToolItem = null;
          RaisePropertyChanged(() => SelectedToolItem);
        }
      }
    }			

    #endregion props

    public HubViewModel(SimpleNavigationService navigationService)
    {
      _navigationService = navigationService;

      setup();
    }

    private async void setup()
    {
      StratNavItems = new ObservableCollection<NavigationItemViewModel>
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

      ToolNavItems = new ObservableCollection<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Natures",
          Description = "Check out how natures affect your Pokemon's stats",
          NavigationPath = ViewModelLocator.NaturePath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Psychic]),
          IconPath = "/Assets/Icons/smile.png"
        },
        new NavigationItemViewModel
        {
          Title = "Types",
          Description = "See how typing affects your Pokemon's performance in battle",
          NavigationPath = ViewModelLocator.TypePath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Bug]),
          IconPath = "/Assets/Icons/fire.png"
        },
      };

      if (!IsInDesignMode && !IsInDesignModeStatic)
        await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///BaseVCD.xml"));
    }

    private void onNavItemSelected(NavigationItemViewModel item)
    {
      if (item == null) return;

      if (string.IsNullOrEmpty(item.NavigationPath))
      {
        MessageBox.Show("That feature isn't available yet! Stay tuned though, it should be coming soon.");
        return;
      }

      _navigationService.Navigate(item.NavigationPath);
    }
  }
}
