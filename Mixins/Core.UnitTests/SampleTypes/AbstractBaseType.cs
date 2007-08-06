using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (BT1Mixin1))]
  public abstract class AbstractBaseType
  {
    public abstract string VirtualMethod ();
    public abstract string VirtualProperty { get; set; }
    public abstract event EventHandler VirtualEvent;
  }
}
