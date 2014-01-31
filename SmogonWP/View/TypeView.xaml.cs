using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.View
{
  public partial class TypeView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public TypeView()
    {
      InitializeComponent();
      _isNewInstance = true;
    }

    private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
    {
      HelpPanel.Visibility = Visibility.Visible;
    }

    private void TypeView_OnBackKeyPress(object sender, CancelEventArgs e)
    {
      if (HelpPanel.Visibility == Visibility.Visible)
      {
        HelpPanel.Visibility = Visibility.Collapsed;
        e.Cancel = true;
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (e.NavigationMode != NavigationMode.Back)
      {
        this.State["tombstoned"] = true;
        Messenger.Default.Send(new TombstoneMessage<TypeViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<TypeViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }

      if (e.NavigationMode == NavigationMode.New &&
          NavigationContext.QueryString.ContainsKey("voiceCommandName"))
      {
        var vcn = NavigationContext.QueryString["voiceCommandName"];

        if (vcn.Contains("SearchOffenseTypes"))
        {
          var typeName = NavigationContext.QueryString["Types"];

          Type type;

          if (Enum.TryParse(typeName, true, out type))
          {
            Messenger.Default.Send(new OffenseTypeMessage(type));
          }
        }
        else if (vcn.Contains("SearchDefenseTypes"))
        {
          var typeName = NavigationContext.QueryString["Types"];

          Type type;

          if (Enum.TryParse(typeName, true, out type))
          {
            Messenger.Default.Send(new DefenseTypeMessage(type));
          }
        }
        else if (vcn.Contains("SearchDualDefenseTypes"))
        {
          var typeName = NavigationContext.QueryString["Types"];
          var secondTypeName = NavigationContext.QueryString["SecondaryTypes"];

          Type type, secondaryType;

          if (Enum.TryParse(typeName, true, out type) &&
              Enum.TryParse(secondTypeName, true, out secondaryType))
          {
            Messenger.Default.Send(new DualDefenseTypeMessage(new Tuple<Type, Type>(type, secondaryType)));
          }
        }
      }
    }

  }
}