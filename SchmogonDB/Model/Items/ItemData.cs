﻿using System.Collections.Generic;
using SchmogonDB.Model.Text;

namespace SchmogonDB.Model.Items
{
  public class ItemData : IDataItem
  {
    public ItemData(string name, IEnumerable<ITextElement> description, IEnumerable<ITextElement> competitive)
    {
      Name = name;
      Description = description;
      Competitive = competitive;
    }

    public string Name { get; private set; }
    public IEnumerable<ITextElement> Description { get; private set; }
    public IEnumerable<ITextElement> Competitive { get; private set; }
  }
}
