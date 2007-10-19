using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.CodeGenSampleTypes
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
