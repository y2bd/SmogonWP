using System;
using Newtonsoft.Json.Linq;
using SchmogonDB.Model.Text;

namespace SchmogonDB.Converters
{
  public class TextElementConverter : JsonCreationConverter<ITextElement>
  {
    protected override ITextElement Create(Type objectType, JObject jObject)
    {
      if (FieldExists("Content", jObject))
      {
        return new Paragraph(jObject["Content"].Value<string>());
      }
      else if (FieldExists("Elements", jObject))
      {
        return new UnorderedList(jObject["Elements"].Values<string>());
      }

      throw new ArgumentException("Mismatched ITextElement");
    }

    private bool FieldExists(string fieldName, JObject jObject)
    {
      return jObject[fieldName] != null;
    }
  }
}
