using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SmogonWP.Utilities;

namespace SmogonWP.Controls
{
  public class SetupChoiceTemplateSelector : DataTemplateSelector
  {
    public DataTemplate OneItem { get; set; }
    public DataTemplate TwoItems { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var list = item as IEnumerable<object>;

      if (list != null)
      {
        var enumerable = list as IList<object> ?? list.ToList();
        if (enumerable.Count() == 1)
        {
          return OneItem;
        }
        else if (enumerable.Count() == 2)
        {
          return TwoItems;
        }
      }

      return base.SelectTemplate(item, container);
    }
  }
}
