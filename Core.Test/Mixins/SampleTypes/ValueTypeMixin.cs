using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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