using System;
using System.Reflection;

namespace Mixins.Definitions.Building
{
  public class SpecialMethodSet : Set<MethodInfo>
  {
    protected override bool ShouldAddItem (MethodInfo item)
    {
      return item != null;
    }
  }
}
