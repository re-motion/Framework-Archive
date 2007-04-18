using System;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  // Assists the DomainObject factory methods in creating domain objects.
  interface IDomainObjectCreator
  {
    DomainObject CreateWithDataContainer (DataContainer dataContainer);
    IInvokeWith<T> GetTypesafeConstructorInvoker<T> ();
  }
}
