using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [MixinFor (typeof (BaseType1))]
  [Serializable]
  public class BT1Mixin2
  {
    [Override]
    public string VirtualMethod ()
    {
      return "Mixin2ForBT1.VirtualMethod";
    }
  }
}
