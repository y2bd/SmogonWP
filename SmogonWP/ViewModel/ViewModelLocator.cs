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
    public const string MoveSearchViewModel = "/View/MoveSearchView.xaml";

    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    public ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      if (ViewModelBase.IsInDesignModeStatic)
      {
        SimpleIoc.Default.Register<ISchmogonClient, Design.DesisgnSchmogonClient>();
      }
      else
      {
        SimpleIoc.Default.Register<ISchmogonClient, SchmogonClient>();
      }

      SimpleIoc.Default.Register<SimpleNavigationService>();
      SimpleIoc.Default.Register<TombstoneService>();
      SimpleIoc.Default.Register<TrayService>();

      SimpleIoc.Default.Register<HomeViewModel>();
      SimpleIoc.Default.Register<MoveSearchViewModel>();
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

    public static void Cleanup()
    {
      // TODO Clear the ViewModels
    }
  }
}