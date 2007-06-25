using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class BT3Mixin7Base : Mixin<IBaseType31, ICBaseType3BT3Mixin4>
  {
    [Override]
    public string IfcMethod()
    {
      return "BT3Mixin7Base.IfcMethod-" + Base.Foo() + "-" + ((IBaseType31)Base).IfcMethod() + "-" + Base.IfcMethod2();
    }
  }
}
