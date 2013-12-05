﻿using GalaSoft.MvvmLight;
using Schmogon.Data;
using Schmogon.Data.Moves;
using SchmogonDB.Model;

namespace SmogonWP.ViewModel.Items
{
  public class MoveItemViewModel : ViewModelBase, ISearchItem
  {
    internal readonly Move Move;

    #region props

    public string Name
    {
      get
      {
        return Move.Name.ToLowerInvariant();
      }
    }

    public string Description
    {
      get
      {
        return Move.Description.ToLowerInvariant().Trim(new [] {'.'});
      }
    }

    public string PageLocation
    {
      get
      {
        return Move.PageLocation;
      }
    }
    
    #endregion
    
    public MoveItemViewModel(Move move)
    {
      Move = move;
    }
  }
}
