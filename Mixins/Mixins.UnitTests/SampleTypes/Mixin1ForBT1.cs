using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IMixin1ForBT1
  {
  }

  [Mixin (typeof (BaseType1))]
  public class Mixin1ForBT1 : IMixin1ForBT1
  {
    [Override]
    public string VirtualMethod ()
    {
      return "Mixin1ForBT1.VirtualMethod";
    }
  }
}
