using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class GenericBaseClass<T>
  {
    public virtual T VirtualMethod ()
    {
      return default (T);
    }
  }
}