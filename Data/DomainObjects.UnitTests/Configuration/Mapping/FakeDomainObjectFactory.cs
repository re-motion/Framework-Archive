using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class FakeDomainObjectFactory : IDomainObjectFactory
  {
    public T Create<T> () where T : DomainObject
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public T Create<T> (ClientTransaction clientTransaction) where T : DomainObject
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public object Create (Type type, ClientTransaction clientTransaction)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public object Create (Type type, params object[] args)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public object Create (Type type)
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
