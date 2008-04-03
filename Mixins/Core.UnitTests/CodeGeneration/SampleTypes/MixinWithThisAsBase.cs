using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins.UnitTests.SampleTypes;

namespace Remotion.Mixins.UnitTests.CodeGeneration.SampleTypes
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
