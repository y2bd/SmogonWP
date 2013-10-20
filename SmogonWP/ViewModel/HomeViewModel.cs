using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using SmogonWP.Services;
using SmogonWP.ViewModel.Home;

namespace SmogonWP.ViewModel
{
  public class HomeViewModel : ViewModelBase
  {
    private SimpleNavigationService _navigationService;

    #region props
    private NavigationItemViewModel _selectedNavItem;
    public NavigationItemViewModel SelectedNavItem
    {
      get
      {
        return _selectedNavItem;
      }
      set
      {
        if (_selectedNavItem != value)
        {
          onNavItemSelected(value);

          // resets the selected item
          _selectedNavItem = null;
          RaisePropertyChanged(() => SelectedNavItem);
        }
      }
    }			

    private ObservableCollection<NavigationItemViewModel> _nivms;
    public ObservableCollection<NavigationItemViewModel> NIVMs
    {
      get
      {
        return _nivms;
      }
      set
      {
        if (_nivms != value)
        {
          _nivms = value;
          RaisePropertyChanged(() => NIVMs);
        }
      }
    }

    #endregion props

    public HomeViewModel(SimpleNavigationService navigationService)
    {
      _navigationService = navigationService;

      setup();
    }

    private void setup()
    {
      NIVMs = new ObservableCollection<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Moves",
          Description = "Search for moves and their relevance",
          NavigationPath = ViewModelLocator.MoveSearchViewModel
        },
        new NavigationItemViewModel
        {
          Title = "Pokémon",
          Description = "Search for Pokémon and their strategies",
          NavigationPath = string.Empty
        },
        new NavigationItemViewModel
        {
          Title = "Items",
          Description = "Search for items and when to equip them",
          NavigationPath = string.Empty
        },
        new NavigationItemViewModel
        {
          Title = "Favorites",
          Description = "Look at entries you've marked as favorites",
          NavigationPath = string.Empty
        }
      };
    }

    private void onNavItemSelected(NavigationItemViewModel item)
    {
      if (item == null) return;

      if (string.IsNullOrEmpty(item.NavigationPath)) return;

      _navigationService.Navigate(item.NavigationPath);
    }
  }
}
