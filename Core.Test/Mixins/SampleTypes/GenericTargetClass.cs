using System;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public class GenericTargetClass<T>
  {
    public virtual T VirtualMethod ()
    {
      return default (T);
    }
  }
}