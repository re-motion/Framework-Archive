using System;
using Remotion.Core.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  public class MixinWithThisAsBase : Mixin<BaseType3, IBaseType31>
  {
    [OverrideTarget]
    public string IfcMethod()
    {
      return "MixinWithThisAsBase.IfcMethod-" + Base.IfcMethod();
    }
  }
}
