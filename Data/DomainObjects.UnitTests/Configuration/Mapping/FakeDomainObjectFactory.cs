using System;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class FakeDomainObjectFactory : IDomainObjectFactory
  {
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public bool WasCreatedByFactory (Type t)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public IFuncInvoker<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type type)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
