using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class FakeDomainObjectFactory : IDomainObjectFactory
  {
    public object Create (Type type, params object[] args)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public bool WasCreatedByFactory (object o)
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
