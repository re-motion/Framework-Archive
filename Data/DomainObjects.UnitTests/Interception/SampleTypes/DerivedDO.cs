using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [Instantiable]
  public class DerivedDO : DOWithVirtualProperties
  {
    public virtual int VirtualPropertyOnDerivedClass
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
  }
}