using System;
using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class MoveSearchView : PhoneApplicationPage
  {
    public MoveSearchView()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.New && 
          NavigationContext.QueryString.ContainsKey("voiceCommandName"))
      {
        var voiceCommandName = NavigationContext.QueryString["voiceCommandName"];

        if (voiceCommandName.Equals("SearchMoves"))
        {
          var moveName = NavigationContext.QueryString["Moves"];

          Messenger.Default.Send(new ViewToVmMessage<string, MoveSearchViewModel>(moveName));
        }
      }

    }

    private void SearchBox_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) MoveList.Focus();
    }

    private void FilterButton_OnClick(object sender, EventArgs e)
    {
      FilterPicker.Open();
    }
  }
}