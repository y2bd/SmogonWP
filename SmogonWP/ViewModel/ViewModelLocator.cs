using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Schmogon;
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
    public const string MoveSearchPath = "/View/MoveSearchView.xaml";
    public const string MoveDataPath = "/View/MoveDataView.xaml";
    public const string AbilitySearchPath = "/View/AbilitySearchView.xaml";
    public const string AbilityDataPath = "/View/AbilityDataView.xaml";
    public const string NaturePath = "/View/NatureView.xaml";
    public const string TypePath = "/View/TypeView.xaml";

    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    public ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      if (ViewModelBase.IsInDesignModeStatic)
      {
        RegisterIfUnregistered<ISchmogonClient, Design.DesisgnSchmogonClient>();
      }
      else
      {
        RegisterIfUnregistered<ISchmogonClient, SchmogonClient>();
      }

      SimpleIoc.Default.Register<SimpleNavigationService>();
      SimpleIoc.Default.Register<TombstoneService>();
      SimpleIoc.Default.Register<TrayService>();

      SimpleIoc.Default.Register<HomeViewModel>();
      SimpleIoc.Default.Register<MoveSearchViewModel>();
      SimpleIoc.Default.Register<MoveDataViewModel>();
      SimpleIoc.Default.Register<AbilitySearchViewModel>();
      SimpleIoc.Default.Register<AbilityDataViewModel>();
      SimpleIoc.Default.Register<NatureViewModel>();
      SimpleIoc.Default.Register<TypeViewModel>();
    }

    public HomeViewModel Home
    {
      get
      {
        return ServiceLocator.Current.GetInstance<HomeViewModel>();
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