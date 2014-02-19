using System;
using System.Windows.Input;

namespace SmogonWP.View.UserControl
{
  public partial class AddPanel : System.Windows.Controls.UserControl
  {
    public event KeyEventHandler InputKeyUp;
    public event EventHandler PanelClosed;
    
    public AddPanel()
    {
      InitializeComponent();
    }

    private void TeamNameInput_OnKeyUp(object sender, KeyEventArgs e)
    {
      var handler = InputKeyUp;
      if (handler != null) handler(sender, e);

      if (e.Key == Key.Enter)
      {
        TeamTypePicker.Focus();
      }
    }

    private void AddCollapsed_Completed(object sender, EventArgs e)
    {
      var handler = PanelClosed;
      if (handler != null) handler(sender, e);
    }

    private void AddVisible_Completed(object sender, EventArgs e)
    {
      TeamNameInput.Focus();
    }
  }
}
