using System.Windows;
using Schmogon.Model.Text;
using SmogonWP.Utilities;

namespace SmogonWP.Controls
{
  public class TextElementTemplateSelector : DataTemplateSelector
  {
    public DataTemplate Paragraph { get; set; }
    public DataTemplate UnorderedList { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item != null)
      {
        if (item is Paragraph)
        {
          return Paragraph;
        }
        else if (item is UnorderedList)
        {
          return UnorderedList;
        }
      }

      return base.SelectTemplate(item, container);
    }
  }
}
