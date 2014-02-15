using System;
using System.Windows;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight;
using SmogonWP.Model;

namespace SmogonWP.ViewModel.Items
{
  public class StatBarItemViewModel : ViewModelBase
  {
    private const double Epsilon = 0.001;

    private readonly double _barWidth;

    private double _currentValue;
    public double CurrentValue
    {
      get
      {
        return _currentValue;
      }
      set
      {
        if (Math.Abs(_currentValue - value) > Epsilon)
        {
          _currentValue = value;
          RaisePropertyChanged(() => CurrentValue);
          RaisePropertyChanged(() => DisplayFraction);
          RaisePropertyChanged(() => Percentage);
          animatePercentageWidth();
        }
      }
    }

    private double _maxValue;
    public double MaxValue
    {
      get
      {
        return _maxValue;
      }
      set
      {
        if (Math.Abs(_maxValue - value) > Epsilon)
        {
          _maxValue = value;
          RaisePropertyChanged(() => MaxValue);
          RaisePropertyChanged(() => DisplayFraction);
          RaisePropertyChanged(() => Percentage);
          animatePercentageWidth();
        }
      }
    }

    public double Percentage
    {
      get
      {
        return CurrentValue/MaxValue*100;
      }
    }
    
    public string DisplayFraction
    {
      get
      {
        return string.Format("{0}/{1}", CurrentValue, MaxValue);
      }
    }
    
    public PercentageWidthDP PercentageWidth { get; private set; }

    public StatBarItemViewModel(double currentValue, double maxValue, double barWidth)
    {
      _barWidth = barWidth;

      _currentValue = currentValue;
      _maxValue = maxValue;

      PercentageWidth = new PercentageWidthDP(0);

      animatePercentageWidth();
    }

    private Storyboard _currentStoryboard;

    private void animatePercentageWidth()
    {
      var newWidth = Percentage / 100.0 * _barWidth;

      var da = new DoubleAnimation
      {
        To = newWidth,
        Duration = new Duration(TimeSpan.FromMilliseconds(300)),
        EasingFunction = new BackEase {EasingMode = EasingMode.EaseOut, Amplitude = 0.5}
      };

      if (_currentStoryboard != null) _currentStoryboard.Stop();

      _currentStoryboard = new Storyboard();
      _currentStoryboard.Children.Add(da);
      
      Storyboard.SetTarget(da, PercentageWidth);
      Storyboard.SetTargetProperty(da, new PropertyPath(PercentageWidthDP.ValueProperty));

      _currentStoryboard.Begin();
    }
  }
}
