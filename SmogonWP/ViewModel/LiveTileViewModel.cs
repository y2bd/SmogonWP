using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Data;
using SmogonWP.Services;

namespace SmogonWP.ViewModel
{
  public class LiveTileViewModel : ViewModelBase
  {
    private readonly LiveTileService _tileService;
    private readonly ISettingsService _settingsService;

    private ObservableCollection<string> _tileStyles;
    public ObservableCollection<string> TileStyles
    {
      get
      {
        return _tileStyles;
      }
      set
      {
        if (_tileStyles != value)
        {
          _tileStyles = value;
          RaisePropertyChanged(() => TileStyles);
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

    private int _selectedTileStyle;
    public int SelectedTileStyle
    {
      get
      {
        return _selectedTileStyle;
      }
      set
      {
        if (_selectedTileStyle != value)
        {
          _selectedTileStyle = value;
          RaisePropertyChanged(() => SelectedTileStyle);

          onSelectedStyleChanged(value);
        }
      }
    }
    
    private bool _tileListEnabled;
    public bool TileListEnabled
    {
      get
      {
        return _tileListEnabled;
      }
      set
      {
        if (_tileListEnabled != value)
        {
          _tileListEnabled = value;
          RaisePropertyChanged(() => TileListEnabled);
        }
      }
    }			
    
    private ObservableCollection<ImageSource> _tileImages;
    public ObservableCollection<ImageSource> TileImages
    {
      get
      {
        return _tileImages;
      }
      set
      {
        if (_tileImages != value)
        {
          _tileImages = value;
          RaisePropertyChanged(() => TileImages);
        }
      }
    }

    private int _selectedTileImage;
    public int SelectedTileImage
    {
      get
      {
        return _selectedTileImage;
      }
      set
      {
        if (_selectedTileImage != value)
        {
          _selectedTileImage = value;
          RaisePropertyChanged(() => SelectedTileImage);

          onSelectedImageChanged(value);
        }
      }
    }

    private bool _flipTile;
    public bool FlipTile
    {
      get
      {
        return _flipTile;
      }
      set
      {
        if (_flipTile != value)
        {
          _flipTile = value;
          RaisePropertyChanged(() => FlipTile);

          onFlipTileChanged(value);
        }
      }
    }			

    public LiveTileViewModel(LiveTileService tileService, ISettingsService settingsService, TrayService trayService)
    {
      _tileService = tileService;
      _settingsService = settingsService;
      _trayService = trayService;

      TileStyles = new ObservableCollection<string>
      {
        "use default logo tile",
        "shuffle image tiles",
        "choose image tile below"
      };

      TileImages = new ObservableCollection<ImageSource>(loadTileImages());

      _selectedTileStyle = _settingsService.Load(LiveTileService.TileStyleKey, 1);
      _selectedTileImage = _settingsService.Load(LiveTileService.TileImageKey, 0);

      _flipTile = _settingsService.Load(LiveTileService.FlipTileKey, true);

      TileListEnabled = _selectedTileStyle == 2;
    }

    private IEnumerable<ImageSource> loadTileImages()
    {
      var tilePaths = _tileService.GetSecretTilePaths();

      return tilePaths.Select(uri => new BitmapImage(uri));
    }

    private async void onSelectedStyleChanged(int value)
    {
      _settingsService.Save(LiveTileService.TileStyleKey, value);

      TileListEnabled = value == 2;

      await updateTile();
    }

    private async void onSelectedImageChanged(int value)
    {
      _settingsService.Save(LiveTileService.TileImageKey, value);

      await updateTile();

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = LiveTileService.GetTileName(value),
        Category = "Live Tile Choice",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    private async void onFlipTileChanged(bool value)
    {
      _settingsService.Save(LiveTileService.FlipTileKey, value);

      await updateTile();

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = value.ToString(),
        Category = "Flip Tile Choice",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    private async Task updateTile()
    {
      TrayService.AddJob("tilemake", "Generating tile...");
      
      // for aesthetic reasons
      await Task.Delay(250);

      await _tileService.GenerateFlipTileAsync();

      TrayService.RemoveJob("tilemake");
    }
  }
}
