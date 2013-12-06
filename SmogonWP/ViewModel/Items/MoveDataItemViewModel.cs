using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Text;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel.Items
{
  public class MoveDataItemViewModel : ViewModelBase
  {
    internal MoveData MoveData;

    #region props

    public string Name
    {
      get { return MoveData.Name.ToLower(); }
    }

    public IEnumerable<ITextElement> Description
    {
      get { return MoveData.Description; }
    }

    public IEnumerable<ITextElement> Competitive
    {
      get { return MoveData.Competitive; }
    }

    public List<Move> RelatedMoves
    {
      get
      {
        return MoveData.RelatedMoves.ToList();
      }
    }

    public string Type
    {
      get
      {
        var name = Enum.GetName(typeof(Type), MoveData.Stats.Type);
        return name != null ? name.ToLower() : "ERROR !!";
      }
    }

    public string Damage
    {
      get { return MoveData.Stats.Damage.ToLower(); }
    }

    public string Target
    {
      get { return MoveData.Stats.Target.ToLower(); }
    }

    public string Power
    {
      get { return MoveData.Stats.Power; }
    }

    public string Accuracy
    {
      get { return MoveData.Stats.Accuracy; }
    }

    public string PP
    {
      get { return MoveData.Stats.PP; }
    }

    public string Priority
    {
      get { return MoveData.Stats.Priority; }
    }

    #endregion props

    public MoveDataItemViewModel(MoveData moveData)
    {
      MoveData = moveData;
    }
  }
}
