using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public struct ValueTypeMixin
  {
    [OverrideTarget]
    public string VirtualMethod ()
    {
      return "ValueTypeMixin.VirtualMethod";
    }
  }
}