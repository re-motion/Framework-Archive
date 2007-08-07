using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class GenericTargetClass<T>
  {
    public virtual T VirtualMethod ()
    {
      return default (T);
    }
  }
}