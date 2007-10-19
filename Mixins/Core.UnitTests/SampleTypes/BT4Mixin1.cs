using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  // no attributes
  public class BT4Mixin1 : Mixin<BaseType4, IBaseType4>
  {
    [OverrideTarget]
    public string NonVirtualMethod ()
    {
      return This.NonVirtualMethod () + "Overridden";
    }

    [OverrideTarget]
    public string NonVirtualProperty
    {
      get { return This.NonVirtualProperty + "Overridden"; }
    }

    [OverrideTarget]
    public event EventHandler NonVirtualEvent;
  }
}
