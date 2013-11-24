using System.Windows.Media;
using GalaSoft.MvvmLight;

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
}
