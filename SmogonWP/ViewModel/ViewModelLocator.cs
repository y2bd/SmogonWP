using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using SchmogonDB;
using SchmogonDB.Tools;
using SmogonWP.Services;

namespace SmogonWP.ViewModel
{
  /// <summary>
  /// This class contains static references to all the view models in the
  /// application and provides an entry point for the bindings.
  /// </summary>
  public class ViewModelLocator
  {
    public const string HomePath = "/View/HomeView.xaml";
    public const string HubPath = "/View/HubView.xaml";
    public const string MoveSearchPath = "/View/MoveSearchView.xaml";
    public const string MoveDataPath = "/View/MoveDataView.xaml";
    public const string AbilitySearchPath = "/View/AbilitySearchView.xaml";
    public const string AbilityDataPath = "/View/AbilityDataView.xaml";
    public const string ItemSearchPath = "/View/ItemSearchView.xaml";
    public const string ItemDataPath = "/View/ItemDataView.xaml";
    public const string NaturePath = "/View/NatureView.xaml";
    public const string TypePath = "/View/TypeView.xaml";
    public const string PokemonSearchPath = "/View/PokemonSearchView.xaml";
    public const string PokemonDataPath = "/View/PokemonDataView.xaml";
    public const string MovesetPath = "/View/MovesetView.xaml";

    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    public ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      if (ViewModelBase.IsInDesignModeStatic)
      {
        RegisterIfUnregistered<IDataLoadingService, Design.DesignDataLoadingService>();
        RegisterIfUnregistered<ISchmogonDBClient, Design.DesignSchmogonDBClient>();
      }
      else
      {
        RegisterIfUnregistered<IDataLoadingService, DataLoadingService>();
        RegisterIfUnregistered<ISchmogonDBClient, SchmogonDBClient>();
      }

      RegisterIfUnregistered<SchmogonToolset>();

      SimpleIoc.Default.Register<SimpleNavigationService>();
      SimpleIoc.Default.Register<TombstoneService>();
      SimpleIoc.Default.Register<TrayService>();
      SimpleIoc.Default.Register<IsolatedStorageService>();
      SimpleIoc.Default.Register<LiveTileService>();

      SimpleIoc.Default.Register<HomeViewModel>();
      SimpleIoc.Default.Register<HubViewModel>();

      SimpleIoc.Default.Register<MoveSearchViewModel>();
      SimpleIoc.Default.Register<MoveDataViewModel>();

      SimpleIoc.Default.Register<AbilitySearchViewModel>();
      SimpleIoc.Default.Register<AbilityDataViewModel>();

      SimpleIoc.Default.Register<ItemSearchViewModel>();
      SimpleIoc.Default.Register<ItemDataViewModel>();

      SimpleIoc.Default.Register<NatureViewModel>();
      SimpleIoc.Default.Register<TypeViewModel>();

      SimpleIoc.Default.Register<PokemonSearchViewModel>();
      SimpleIoc.Default.Register<PokemonDataViewModel>();
      SimpleIoc.Default.Register<MovesetViewModel>();
    }

    public HomeViewModel Home
    {
      get
      {
        return ServiceLocator.Current.GetInstance<HomeViewModel>();
      }
    }

    public HubViewModel Hub
    {
      get
      {
        return ServiceLocator.Current.GetInstance<HubViewModel>();
      }
    }

    public MoveSearchViewModel MoveSearch
    {
      get
      {
        return ServiceLocator.Current.GetInstance<MoveSearchViewModel>();
      }
    }

    public MoveDataViewModel MoveData
    {
      get
      {
        return ServiceLocator.Current.GetInstance<MoveDataViewModel>();
      }
    }

    public AbilitySearchViewModel AbilitySearch
    {
      get
      {
        return ServiceLocator.Current.GetInstance<AbilitySearchViewModel>();
      }
    }

    public AbilityDataViewModel AbilityData
    {
      get
      {
        return ServiceLocator.Current.GetInstance<AbilityDataViewModel>();
      }
    }

    public ItemSearchViewModel ItemSearch
    {
      get
      {
        return ServiceLocator.Current.GetInstance<ItemSearchViewModel>();
      }
    }

    public ItemDataViewModel ItemData
    {
      get
      {
        return ServiceLocator.Current.GetInstance<ItemDataViewModel>();
      }
    }

    public NatureViewModel Nature
    {
      get
      {
        return ServiceLocator.Current.GetInstance<NatureViewModel>();
      }
    }

    public TypeViewModel Type
    {
      get
      {
        return ServiceLocator.Current.GetInstance<TypeViewModel>();
      }
    }

    public PokemonSearchViewModel PokemonSearch
    {
      get
      {
        return ServiceLocator.Current.GetInstance<PokemonSearchViewModel>();
      }
    }

    public PokemonDataViewModel PokemonData
    {
      get
      {
        return ServiceLocator.Current.GetInstance<PokemonDataViewModel>();
      }
    }

    public MovesetViewModel Moveset
    {
      get
      {
        return ServiceLocator.Current.GetInstance<MovesetViewModel>();
      }
      
    }

    private void RegisterIfUnregistered<T>() where T : class
    {
      if (SimpleIoc.Default.IsRegistered<T>() != true) SimpleIoc.Default.Register<T>();
    }

    private void RegisterIfUnregistered<TI, TV>()
      where TI : class
      where TV : class
    {
      if (SimpleIoc.Default.IsRegistered<TI>() != true) SimpleIoc.Default.Register<TI, TV>();
    }

    public static void Cleanup()
    {
      // TODO Clear the ViewModels
    }
  }
}