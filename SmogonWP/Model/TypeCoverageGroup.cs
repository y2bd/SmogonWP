using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.Model
{
  public enum CoverageType
  {
    OffenseCoverage,
    DefenseCoverage
  }

  public class TypeCoverageGroup : ObservableCollection<TypeItemViewModel>
  {
    public CoverageType Key { get; set; }

    public string DisplayName
    {
      get
      {
        switch (Key)
        {
          case CoverageType.OffenseCoverage:
            return "NO OFFENSIVE COVERAGE";
          case CoverageType.DefenseCoverage:
            return "NO DEFENSIVE COVERAGE";
          default:
            return string.Empty;
        }
      }
    }

    public TypeCoverageGroup(CoverageType key)
    {
      Key = key;
    }

    public TypeCoverageGroup(IEnumerable<TypeItemViewModel> collection, CoverageType key)
      : base(collection)
    {
      Key = key;
    }
  }
}
