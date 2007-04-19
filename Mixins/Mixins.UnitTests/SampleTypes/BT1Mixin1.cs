using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT1Mixin1
  {
    string IntroducedMethod ();
  }

  [MixinFor (typeof (BaseType1))]
  [Serializable]
  public class BT1Mixin1 : IBT1Mixin1
  {
    [Override]
    public string VirtualMethod ()
    {
      return "BT1Mixin1.VirtualMethod";
    }

    public string IntroducedMethod ()
    {
      return "BT1Mixin1.IntroducedMethod";
    }
  }
}
