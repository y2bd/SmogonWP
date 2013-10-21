using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Schmogon.Data.Moves;

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

    public string Description
    {
      get { return MoveData.Description; }
    }

    public string Competitive
    {
      get { return MoveData.Competitive; }
    }

    public List<Move> RelatedMoves
    {
      get
      {
        return MoveData.RelatedMoves.Select(
          m => new Move(m.Name.ToLower(), m.Description, m.PageLocation)
        ).ToList();
      }
    }

    public string Type
    {
      get { return MoveData.Stats.Type.ToLower(); }
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
