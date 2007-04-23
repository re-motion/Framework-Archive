using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  // no attributes
  public class BT4Mixin1 : Mixin<BaseType4, BaseType4>
  {
    [Override]
    public string NonVirtualMethod ()
    {
      return Base.NonVirtualMethod () + "Overridden";
    }

    [Override]
    public string NonVirtualProperty
    {
      get { return Base.NonVirtualProperty + "Overridden"; }
    }

    [Override]
    public event EventHandler NonVirtualEvent;
  }
}
