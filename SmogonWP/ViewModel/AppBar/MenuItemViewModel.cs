using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SmogonWP.ViewModel.AppBar
{
  public class MenuItemViewModel : ViewModelBase
  {
    private RelayCommand _command;
    public RelayCommand Command
    {
      get { return _command; }
      set
      {
        _command = value;
        RaisePropertyChanged(() => Command);
      }
    }

    private object _commandParameter;
    public object CommandParameter
    {
      get { return _commandParameter; }
      set
      {
        _commandParameter = value;
        RaisePropertyChanged(() => CommandParameter);
      }
    }

    private string _text;
    public string Text
    {
      get { return _text; }
      set
      {
        _text = value;
        RaisePropertyChanged(() => Text);
      }
    }
  }
}
