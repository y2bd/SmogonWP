﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
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

      _selectedTileStyle = _settingsService.Load(LiveTileService.TileStyleKey, 0);
      _selectedTileImage = _settingsService.Load(LiveTileService.TileImageKey, 0);

      TileListEnabled = _selectedTileStyle == 2;
    }

    private IEnumerable<ImageSource> loadTileImages()
    {
      var tilePaths = _tileService.GetSecretTilePaths();

      return tilePaths.Select(uri => new BitmapImage(uri));
    }

    private async void onSelectedStyleChanged(int value)
    {
      TrayService.AddJob("tilemake", "Generating tile...");
      
      _settingsService.Save(LiveTileService.TileStyleKey, value);

      TileListEnabled = value == 2;

      // for aesthetic reasons
      await Task.Delay(250);

      await _tileService.GenerateFlipTileAsync();

      TrayService.RemoveJob("tilemake");
    }

    private async void onSelectedImageChanged(int value)
    {
      TrayService.AddJob("tilemake", "Generating tile...");

      _settingsService.Save(LiveTileService.TileImageKey, value);

      await _tileService.GenerateFlipTileAsync();

      TrayService.RemoveJob("tilemake");
    }
  }
}
