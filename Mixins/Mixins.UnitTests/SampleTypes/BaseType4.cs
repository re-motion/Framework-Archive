using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  // no attributes
  public class BaseType4
  {
    public string NonVirtualMethod ()
    {
      return "BaseType4.NonVirtualMethod";
    }

    public string NonVirtualProperty
    {
      get { return "BaseType4.NonVirtualProperty"; }
    }

    public event EventHandler NonVirtualEvent;
  }
}
