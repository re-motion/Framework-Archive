using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public class BT1Attribute : Attribute { }

  [Serializable]
  [BT1Attribute]
  public class BaseType1
  {
    public int I;

    public virtual string VirtualMethod ()
    {
      return "BaseType1.VirtualMethod";
    }

    public virtual string VirtualMethod (string text)
    {
      return "BaseType1.VirtualMethod(" + text + ")";
    }
  }
}
