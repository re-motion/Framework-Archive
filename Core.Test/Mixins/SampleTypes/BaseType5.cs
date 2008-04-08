using System;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  // no attributes
  public class BaseType5
  {
    public string Method ()
    {
      return "BaseType4.NonVirtualMethod";
    }

    public int Property
    {
      set { }
    }

    public event Action<string> Event;
  }
}
