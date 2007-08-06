using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public abstract class AbstractBaseType
  {
    public abstract string VirtualMethod ();
    public abstract string VirtualProperty { get; set; }
    public abstract event EventHandler VirtualEvent;

    public virtual string OverridableMethod (int i)
    {
      return "AbstractBaseType.OverridableMethod(" + i + ")";
    }
  }
}
