using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Utilities;
using Rubicon.Logging;
using System.Reflection;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  // Creates new domain object instances via a Reflection-based constructor call.
  // Needed constructors:
  // MyDomainObject (DataContainer) -- for loading
  // (any constructor) -- for new objects
  class LegacyDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static LegacyDomainObjectCreator Instance = new LegacyDomainObjectCreator ();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      GetDelegateWith<object> constructorDelegateHolder = ConstructorWrapper.GetConstructor (dataContainer.DomainObjectType,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      return (DomainObject) constructorDelegateHolder.With<DataContainer> () (dataContainer);
    }

    public IInvokeWith<T> GetTypesafeConstructorInvoker<T> ()
    {
      return new InvokeWith<T>(ConstructorWrapper.GetConstructor<T> (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
    }
  }
}
