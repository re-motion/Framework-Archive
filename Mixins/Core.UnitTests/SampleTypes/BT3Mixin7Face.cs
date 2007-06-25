using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public class BT3Mixin7Face: Mixin<ICBaseType3BT3Mixin4>
  {
    public string InvokeThisMethods()
    {
      return ((IBaseType31)This).IfcMethod() + "-" + This.Foo();
    }
  }
}
