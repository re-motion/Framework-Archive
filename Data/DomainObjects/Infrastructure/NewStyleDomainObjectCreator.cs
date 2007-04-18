using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Reflection;
using Rubicon.Utilities;
using System.Reflection;
using Rubicon.Logging;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  // Creates new domain object instances via the DomainObjectFactory.
  // Needed constructors:
  // MyDomainObject (DataContainer) -- for loading
  // (any constructor) -- for new objects
  class NewStyleDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static NewStyleDomainObjectCreator Instance = new NewStyleDomainObjectCreator ();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(dataContainer.DomainObjectType);
      return factory.GetTypesafeConstructorInvoker<DomainObject> (concreteType).With (dataContainer);
    }

    public IInvokeWith<T> GetTypesafeConstructorInvoker<T> ()
    {
      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(typeof (T));
      return factory.GetTypesafeConstructorInvoker<T> (concreteType);
    }
  }
}
