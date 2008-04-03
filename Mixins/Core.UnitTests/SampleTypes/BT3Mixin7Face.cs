using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Serializable]
  public class BT3Mixin7Face : Mixin<ICBaseType3BT3Mixin4>
  {
    public string InvokeThisMethods()
    {
      return ((IBaseType31)This).IfcMethod() + "-" + This.Foo();
    }
  }
}
