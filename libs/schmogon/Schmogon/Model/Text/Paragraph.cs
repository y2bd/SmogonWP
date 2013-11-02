namespace Schmogon.Model.Text
{
  public class Paragraph : ITextElement
  {
    public string Content { get; private set; }

    public Paragraph(string content)
    {
      Content = content.Trim();
    }
  }
}