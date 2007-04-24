using System;
using System.Reflection;

namespace Mixins.Definitions.Building
{
  public class SpecialMethodSet : Set<MethodInfo>
  {
    public override void Add (MethodInfo item)
    {
      if (item != null)
        base.Add (item);
    }
  }
}
