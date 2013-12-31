using System;
using SmogonWP.ViewModel;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.Messages
{
  public class OffenseTypeMessage : ViewToVmMessage<Type, TypeViewModel>
  {
    public OffenseTypeMessage(Type content) : base(content)
    {
    }

    public OffenseTypeMessage(object sender, Type content) : base(sender, content)
    {
    }

    public OffenseTypeMessage(object sender, TypeViewModel target, Type content) : base(sender, target, content)
    {
    }
  }

  public class DefenseTypeMessage : ViewToVmMessage<Type, TypeViewModel>
  {
    public DefenseTypeMessage(Type content) : base(content)
    {
    }

    public DefenseTypeMessage(object sender, Type content) : base(sender, content)
    {
    }

    public DefenseTypeMessage(object sender, TypeViewModel target, Type content) : base(sender, target, content)
    {
    }
  }

  public class DualDefenseTypeMessage : ViewToVmMessage<Tuple<Type, Type>, TypeViewModel>
  {
    public DualDefenseTypeMessage(Tuple<Type, Type> content) : base(content)
    {
    }

    public DualDefenseTypeMessage(object sender, Tuple<Type, Type> content) : base(sender, content)
    {
    }

    public DualDefenseTypeMessage(object sender, TypeViewModel target, Tuple<Type, Type> content) : base(sender, target, content)
    {
    }
  }
}
