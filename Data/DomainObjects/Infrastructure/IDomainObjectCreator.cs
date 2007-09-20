using System;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  // Assists the DomainObject factory methods in creating domain objects.
  interface IDomainObjectCreator
  {
    DomainObject CreateWithDataContainer (DataContainer dataContainer);
    IFuncInvoker<T> GetTypesafeConstructorInvoker<T> () where T : DomainObject;
    IFuncInvoker<DomainObject> GetTypesafeConstructorInvoker (Type domainObjectType);
  }
}
