using System;
using System.Collections.Generic;
using System.Text;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests.Mixins.CodeGenSampleTypes
{
  public class MixinWithThisAsBase : Mixin<BaseType3, IBaseType31>
  {
    [Override]
    public string IfcMethod()
    {
      return "MixinWithThisAsBase.IfcMethod-" + Base.IfcMethod();
    }
  }
}
