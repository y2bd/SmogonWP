using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Data;
using Nito.AsyncEx;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.View;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmogonWP.ViewModel
{
  public class PokemonDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string SmogonDexPrefix = "http://www.smogon.com/dex/bw/pokemon/";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly IDataLoadingService _dataService;
    private readonly SimpleNavigationService _navigationService;
    private readonly TombstoneService _tombstoneService;

    private readonly MessageReceiver<ItemSearchedMessage<Pokemon>> _pokemonSearchReceiver;
    private readonly MessageSender<PokemonTypeSelectedMessage> _pokemonTypeSelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Ability>> _pokemonAbilitySelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Move>> _pokemonMoveSelectedSender;
    private readonly MessageSender<ItemSelectedMessage<MovesetItemViewModel>> _movesetSelectedSender; 
    
    private string _pageLocation;

    // used for tombstoning
    private Pokemon _rawPokemon;

    #region props

    private string _name = string.Empty;
    public string Name
    {
      get
      {
        return _name.ToUpper();
      }
      set
      {
        if (_name != value)
        {
          _name = value;
          RaisePropertyChanged(() => Name);
        }
      }
    }

    private PokemonDataItemViewModel _pdvm;
    public PokemonDataItemViewModel PDVM
    {
      get
      {
        return _pdvm;
      }
      set
      {
        if (_pdvm != value)
        {
          _pdvm = value;
          RaisePropertyChanged(() => PDVM);
        }
      }
    }

    private ImageSource _sprite;
    public ImageSource Sprite
    {
      get
      {
        return _sprite;
      }
      set
      {
        if (_sprite != value)
        {
          _sprite = value;
          RaisePropertyChanged(() => Sprite);
        }
      }
    }

    private Uri _gifImageSource;
    public Uri GifImageSource
    {
      get { return _gifImageSource; }
      set
      {
        if (_gifImageSource != value)
        {
          _gifImageSource = value;
          RaisePropertyChanged(() => GifImageSource);
        }
      }
    }			

    private AbilityItemViewModel _selectedAbility;
    public AbilityItemViewModel SelectedAbility
    {
      get
      {
        return _selectedAbility;
      }
      set
      {
        if (_selectedAbility != value)
        {
          onAbilitySelected(value);

          _selectedAbility = null;
          RaisePropertyChanged(() => SelectedAbility);
        }
      }
    }

    private TypeItemViewModel _selectedtype;
    public TypeItemViewModel SelectedType
    {
      get
      {
        return _selectedtype;
      }
      set
      {
        if (_selectedtype != value)
        {
          onTypeSelected(value);

          _selectedtype = null;
          RaisePropertyChanged(() => SelectedType);
        }
      }
    }

    private MoveItemViewModel _selectedMove;
    public MoveItemViewModel SelectedMove
    {
      get
      {
        return _selectedMove;
      }
      set
      {
        if (_selectedMove != value)
        {
          onMoveSelected(value);

          _selectedMove = null;
          RaisePropertyChanged(() => SelectedMove);
        }
      }
    }

    private MovesetItemViewModel _selectedMoveset;
    public MovesetItemViewModel SelectedMoveset
    {
      get
      {
        return _selectedMoveset;
      }
      set
      {
        if (_selectedMoveset != value)
        {
          onMovesetSelected(value);

          _selectedMoveset = null;
          RaisePropertyChanged(() => SelectedMoveset);
        }
      }
    }			

    private TrayService _trayService;
    public TrayService TrayService
    {
      get
      {
        return _trayService;
      }
      set
      {
        if (_trayService != value)
        {
          _trayService = value;
          RaisePropertyChanged(() => TrayService);
        }
      }
    }

    private ObservableCollection<MenuItemViewModel> _menuItems;
    public ObservableCollection<MenuItemViewModel> MenuItems
    {
      get
      {
        return _menuItems;
      }
      set
      {
        if (_menuItems != value)
        {
          _menuItems = value;
          RaisePropertyChanged(() => MenuItems);
        }
      }
    }

    #endregion props

    #region async handlers

    public INotifyTaskCompletion FetchPokemonDataNotifier { get; private set; }

    #endregion async handlers

    public PokemonDataViewModel(IDataLoadingService dataService, SimpleNavigationService navigationService, TrayService trayService, TombstoneService tombstoneService)
    {
      _dataService = dataService;
      _navigationService = navigationService;
      _trayService = trayService;
      _tombstoneService = tombstoneService;

      _pokemonSearchReceiver = new MessageReceiver<ItemSearchedMessage<Pokemon>>(onPokemonSearched, true);
      _pokemonTypeSelectedSender = new MessageSender<PokemonTypeSelectedMessage>();
      _pokemonAbilitySelectedSender = new MessageSender<ItemSelectedMessage<Ability>>();
      _pokemonMoveSelectedSender = new MessageSender<ItemSelectedMessage<Move>>();
      _movesetSelectedSender = new MessageSender<ItemSelectedMessage<MovesetItemViewModel>>();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        SchedulePokemonFetch(null);
      }

      setupAppBar();

      MessengerInstance.Register(this, new Action<TombstoneMessage<PokemonDataViewModel>>(m => tombstone()));
      MessengerInstance.Register(this, new Action<RestoreMessage<PokemonDataViewModel>>(m => restore()));
    }

    #region event handlers

    private void onPokemonSearched(ItemSearchedMessage<Pokemon> msg)
    {
      // clear the current pokemon if it exists
      // otherwise we run into stack issues

      PDVM = null;

      Name = msg.Item.Name;

      SchedulePokemonFetch(msg.Item);
    }

    private void onAbilitySelected(AbilityItemViewModel aivm)
    {
      _pokemonAbilitySelectedSender.SendMessage(new ItemSelectedMessage<Ability>(aivm.Ability));
      _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
    }

    private void onTypeSelected(TypeItemViewModel tivm)
    {
      if (PDVM.Types.Count() > 1)
      {
        var secondType = PDVM.Types.First(t => t.Type != tivm.Type).Type;

        _pokemonTypeSelectedSender.SendMessage(new PokemonTypeSelectedMessage(tivm.Type, secondType));
      }
      else
      {
        _pokemonTypeSelectedSender.SendMessage(new PokemonTypeSelectedMessage(tivm.Type));
      }
      
      _navigationService.Navigate(ViewModelLocator.TypePath);
    }

    private void onMoveSelected(MoveItemViewModel mivm)
    {
      _pokemonMoveSelectedSender.SendMessage(new ItemSelectedMessage<Move>(mivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath);
    }

    private void onMovesetSelected(MovesetItemViewModel msivm)
    {
      _movesetSelectedSender.SendMessage(new ItemSelectedMessage<MovesetItemViewModel>(msivm));
      _navigationService.Navigate((ViewModelLocator.MovesetPath));
    }

    #endregion event handlers

    #region appbar

    private void setupAppBar()
    {
      var smogon = new MenuItemViewModel
      {
        Command = new RelayCommand(onOpenSmogonPressed),
        Text = "open Smogon in browser..."
      };

      var bulb = new MenuItemViewModel
      {
        Command = new RelayCommand(onOpenBulbapediaPressed),
        Text = "open Bulbapedia in browser..."
      };

      MenuItems = new ObservableCollection<MenuItemViewModel> { smogon, bulb };
    }

    private void onOpenSmogonPressed()
    {
      var wbt = new WebBrowserTask
      {
        Uri = new Uri(SmogonDexPrefix + SpritePathConstructor.ReplaceNameIfNecessary(PDVM.Data.Name.ToLower()))
      };

      wbt.Show();
    }

    private void onOpenBulbapediaPressed()
    {
      var name = toTitleCase(Name);

      // dammit ho-oh!
      if (name.Contains("-") && !name.Equals("Ho-oh"))
      {
        // take every character before the hyphen, bulbapedia doesn't like forme suffixes
        name = string.Join("", name.TakeWhile(c => c != '-'));
      }

      var wbt = new WebBrowserTask
      {
        Uri = new Uri(BulbaPrefix + Uri.EscapeDataString(name))
      };

      wbt.Show();
    }

    private static string toTitleCase(string word)
    {
      IEnumerable<string> split = word.Split(' ').ToList();
      split = split.Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower());

      return string.Join(" ", split);
    }

    #endregion appbar

    private void SchedulePokemonFetch(Pokemon pokemon)
    {
      FetchPokemonDataNotifier = NotifyTaskCompletion.Create(fetchPokemonData(pokemon));

      FetchPokemonDataNotifier.PropertyChanged += (sender, args) =>
      {
        // we broked
        if (FetchPokemonDataNotifier == null) return;

        if (FetchPokemonDataNotifier.IsFaulted)
        {
          throw FetchPokemonDataNotifier.InnerException;
        }
      };
    }

    private async Task fetchPokemonData(Pokemon pokemon)
    {
      TrayService.AddJob("fetchdata", "Fetching pokemon data...");

      _rawPokemon = pokemon;

      Sprite = null;

      string gifPath = SpritePathConstructor.ConstructSpritePath(pokemon.Name);
      GifImageSource = new Uri(gifPath);

      PokemonData pokemonData;

      try
      {
        pokemonData = await _dataService.FetchPokemonDataAsync(pokemon);
      }
      catch (Exception)
      {
        MessageBox.Show(
          "Your pokemon data may be corrupted. Please restart the app and try again. If this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        Debugger.Break();

        cleanup();

        return;
      }

      PDVM = new PokemonDataItemViewModel(pokemonData);
      Name = PDVM.Name;

      //await fetchThumbnailImage(PDVM.SpritePath);

      _pageLocation = pokemon.PageLocation;

      TrayService.RemoveJob("fetchdata");

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = PDVM.Data.Name,
        Category = "Pokemon Search",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
      
      MessengerInstance.Send(new VmToViewMessage<string, PokemonDataView>("loadedAnim"));
    }
    
    private async Task fetchThumbnailImage(string path)
    {
      if (!NetUtilities.IsNetwork()) return;

      Stream s;

      using (var client = new HttpClient())
      {
        var bytes = await client.GetByteArrayAsync(path);

        s = new MemoryStream(bytes);
      }

      DispatcherHelper.CheckBeginInvokeOnUI(() =>
      {
        var wbs = new WriteableBitmap(96, 96);
        wbs.SetSource(s);

        Sprite = wbs.Resize(140, 140, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
        
      });
    }

    private void thumbnailFetchFaulted(Task t)
    {
      var ex = t.Exception;
    }

    private void cleanup()
    {
      PDVM = null;
      FetchPokemonDataNotifier = null;
      TrayService.RemoveAllJobs();
    }

    private async void tombstone()
    {
      if (_rawPokemon != null)
        await _tombstoneService.Store("ts_pokemon", _rawPokemon);

      Debug.WriteLine("Done tombstoning Pokemon");

      //await _tombstoneService.Save();
    }

    private async void restore()
    {
      if (PDVM != null) return;

      var loaded = await _tombstoneService.Load<Pokemon>("ts_pokemon");

      if (loaded != null)
      {
        Name = loaded.Name;
        SchedulePokemonFetch(loaded);
      }
    }
  }
}