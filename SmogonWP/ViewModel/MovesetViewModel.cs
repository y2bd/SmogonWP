using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Text;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel
{
  public class MovesetViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly TombstoneService _tombstoneService;

    private readonly MessageReceiver<ItemSelectedMessage<MovesetItemViewModel>> _movesetSelectedReceiver;
    private readonly MessageSender<ItemSelectedMessage<Ability>> _abilitySelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Nature>> _natureSelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Item>> _itemSelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Move>> _moveSelectedSender;
    private readonly MessageSender<MovesetCalculatedMessage> _movesetCalculatedSender;

    private MovesetItemViewModel _msivm;
    public MovesetItemViewModel MSIVM
    {
      get
      {
        return _msivm;
      }
      set
      {
        if (_msivm != value)
        {
          _msivm = value;
          RaisePropertyChanged(() => MSIVM);

          RaisePropertyChanged(() => Name);
          RaisePropertyChanged(() => OwnerName);

        }
      }
    }

    public string Name
    {
      get
      {
        return MSIVM == null ? string.Empty : MSIVM.Data.Name.ToUpperInvariant();
      }
    }

    public string OwnerName
    {
      get
      {
        return MSIVM == null ? string.Empty : MSIVM.OwnerName.ToUpperInvariant();
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

    private string _selectedNature;
    public string SelectedNature
    {
      get
      {
        return _selectedNature;
      }
      set
      {
        if (_selectedNature != value)
        {
          onNatureSelected(value);

          _selectedNature = null;
          RaisePropertyChanged(() => SelectedNature);
        }
      }
    }

    private ItemItemViewModel _selectedItem;
    public ItemItemViewModel SelectedItem
    {
      get
      {
        return _selectedItem;
      }
      set
      {
        if (_selectedItem != value)
        {
          onItemSelected(value);

          _selectedItem = null;
          RaisePropertyChanged(() => SelectedItem);
        }
      }
    }

    private GroupedMoveItemViewModel _selectedMove;
    public GroupedMoveItemViewModel SelectedMove
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

    private ObservableCollection<MenuButtonViewModel> _menuButtons;
    public ObservableCollection<MenuButtonViewModel> MenuButtons
    {
      get
      {
        return _menuButtons;
      }
      set
      {
        if (_menuButtons != value)
        {
          _menuButtons = value;
          RaisePropertyChanged(() => MenuButtons);
        }
      }
    }

    public MovesetViewModel(SimpleNavigationService navigationService, TombstoneService tombstoneService)
    {
      _navigationService = navigationService;
      _tombstoneService = tombstoneService;

      _movesetSelectedReceiver = new MessageReceiver<ItemSelectedMessage<MovesetItemViewModel>>(onMovesetSelected, true);

      _abilitySelectedSender = new MessageSender<ItemSelectedMessage<Ability>>();
      _natureSelectedSender = new MessageSender<ItemSelectedMessage<Nature>>();
      _itemSelectedSender = new MessageSender<ItemSelectedMessage<Item>>();
      _moveSelectedSender = new MessageSender<ItemSelectedMessage<Move>>();

      _movesetCalculatedSender = new MessageSender<MovesetCalculatedMessage>();

      #region design data
      if (IsInDesignMode || IsInDesignModeStatic)
      {
        var ms = new Moveset
        {
          Name = "Kickapow",
          Abilities = new List<Ability> { new Ability("Burnt Toast", "Burns toast 100% of the time", "") },
          Description = new List<ITextElement>
          {
            new Paragraph("THIS MOVESET IS REALL GOOD ECAUSE IT HITS HARD NO SQUISHY FLAMEBATE LOL")
          },
          EVSpread = new BaseStat(6, 252, 0, 0, 0, 252),
          Items = new List<Item>
          {
            new Item("Life Orb", "Kicks life in the orbs", "")
          },
          Moves = new List<IEnumerable<Move>>
          {
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)
            },
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)
            },
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal)
            },
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "", Type.Normal),
              new Move("Milk Drain", "gotta get dat white stuff", "", Type.Normal)
            }
          },
          Natures = new List<Nature> { Nature.Adamant, Nature.Brave }
        };

        MSIVM = new MovesetItemViewModel("Barbasaur", ms);
      }
      #endregion design data

      setupAppBar();

      MessengerInstance.Register(this, new Action<TombstoneMessage<MovesetViewModel>>(m => tombstone()));
      MessengerInstance.Register(this, new Action<RestoreMessage<MovesetViewModel>>(m => restore()));
    }

    private void setupAppBar()
    {
      MenuButtons = new ObservableCollection<MenuButtonViewModel>
      {
        new MenuButtonViewModel
        {
          Text = "stats",
          IconUri = new Uri("/Assets/AppBar/appbar.calc.png", UriKind.RelativeOrAbsolute),
          Command = new RelayCommand(navigateToStats)
        }
      };
    }

    private void navigateToStats()
    {
      _movesetCalculatedSender.SendMessage(new MovesetCalculatedMessage(MSIVM));
      _navigationService.Navigate(ViewModelLocator.StatsPath);
    }

    private void onMovesetSelected(ItemSelectedMessage<MovesetItemViewModel> msg)
    {
      MSIVM = msg.Item;
    }

    private void onAbilitySelected(AbilityItemViewModel aivm)
    {
      if (aivm == null) return;

      _abilitySelectedSender.SendMessage(new ItemSelectedMessage<Ability>(aivm.Ability));
      _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
    }

    private void onNatureSelected(string nats)
    {
      if (string.IsNullOrWhiteSpace(nats)) return;

      Nature nature;

      if (Enum.TryParse(nats, true, out nature))
      {
        _natureSelectedSender.SendMessage(new ItemSelectedMessage<Nature>(nature));
        _navigationService.Navigate(ViewModelLocator.NaturePath);
      }
    }

    private void onItemSelected(ItemItemViewModel iivm)
    {
      if (iivm == null) return;

      _itemSelectedSender.SendMessage(new ItemSelectedMessage<Item>(iivm.Item));
      _navigationService.Navigate(ViewModelLocator.ItemDataPath);

    }

    private void onMoveSelected(GroupedMoveItemViewModel gmivm)
    {
      if (gmivm == null) return;

      _moveSelectedSender.SendMessage(new ItemSelectedMessage<Move>(gmivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath);
    }

    private async void tombstone()
    {
      if (MSIVM != null)
      {
        var sms = SerializeableMoveset.FromMoveset(MSIVM.Data, MSIVM.OwnerName);

        await _tombstoneService.Store("ts_moveset", sms);
      }

      //await _tombstoneService.Save();
    }

    private async void restore()
    {
      if (MSIVM != null) return;

      var loaded = await _tombstoneService.Load<SerializeableMoveset>("ts_moveset");

      if (loaded != null)
      {
        var ownerName = loaded.OwnerName;
        var ms = loaded.ToMoveset();

        MSIVM = new MovesetItemViewModel(ownerName, ms);
      }
    }

    #region serialization
    public class SerializeableMoveset : Moveset
    {
      public new ICollection<SerializeableTextElement> Description { get; set; }

      public string OwnerName { get; set; }
      
      public Moveset ToMoveset()
      {
        return new Moveset
        {
          Name = this.Name,
          Items = this.Items,
          Abilities = this.Abilities,
          Natures = this.Natures,
          Moves = this.Moves,
          EVSpread = this.EVSpread,
          Description = this.Description.Select(SerializeableTextElement.ConvertBack).ToList()
        };
      }

      public static SerializeableMoveset FromMoveset(Moveset ms, string ownerName)
      {
        return new SerializeableMoveset
        {
          OwnerName = ownerName,
          Name = ms.Name,
          Items = ms.Items,
          Abilities = ms.Abilities,
          Natures = ms.Natures,
          Moves = ms.Moves,
          EVSpread = ms.EVSpread,
          Description = ms.Description.Select(SerializeableTextElement.Convert).ToList()
        };
      }
    }

    [KnownType(typeof(SerializeableParagraph))]
    [KnownType(typeof(SerializeableUnorderedList))]
    public abstract class SerializeableTextElement
    {
      public static SerializeableTextElement Convert(ITextElement t)
      {
        var paragraph = t as Paragraph;
        if (paragraph != null)
        {
          return new SerializeableParagraph { Content = paragraph.Content };
        }

        var list = t as UnorderedList;
        if (list != null)
        {
          return new SerializeableUnorderedList { Elements = list.Elements };
        }

        return null;
      }

      public static ITextElement ConvertBack(SerializeableTextElement t)
      {
        var paragraph = t as SerializeableParagraph;
        if (paragraph != null)
        {
          return new Paragraph { Content = paragraph.Content };
        }

        var list = t as SerializeableUnorderedList;
        if (list != null)
        {
          return new UnorderedList { Elements = list.Elements.ToList() };
        }

        return null;
      }
    }

    public class SerializeableParagraph : SerializeableTextElement
    {
      public string Content { get; set; }
    }

    public class SerializeableUnorderedList : SerializeableTextElement
    {
      public IEnumerable<string> Elements { get; set; }
    }
    #endregion serialization
  }


}
