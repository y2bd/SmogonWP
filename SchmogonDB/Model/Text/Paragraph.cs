namespace SchmogonDB.Model.Text
{
  public class Paragraph : ITextElement
  {
    public string Content { get; set; }

    public Paragraph(string content)
    {
      Content = content.Trim();
    }

    public Paragraph() { }
  }
}