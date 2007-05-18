using System;
using System.Reflection;
using Mixins.Utilities;

namespace Mixins.Definitions.Building
{
  internal class SpecialMethodSet : Set<MethodInfo>
  {
    protected override bool ShouldAddItem (MethodInfo item)
    {
      return item != null;
    }
  }
}
