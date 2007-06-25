using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  // no attributes
  public class BT4Mixin1 : Mixin<BaseType4, IBaseType4>
  {
    [Override]
    public string NonVirtualMethod ()
    {
      return This.NonVirtualMethod () + "Overridden";
    }

    [Override]
    public string NonVirtualProperty
    {
      get { return This.NonVirtualProperty + "Overridden"; }
    }

    [Override]
    public event EventHandler NonVirtualEvent;
  }
}
