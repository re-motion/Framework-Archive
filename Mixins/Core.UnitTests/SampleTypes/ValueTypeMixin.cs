using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
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