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
  // (any constructor) -- for new objects
  // no constructor for loading required
  class NewStyleDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static NewStyleDomainObjectCreator Instance = new NewStyleDomainObjectCreator ();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(dataContainer.DomainObjectType);
      DomainObject instance = (DomainObject) System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject (concreteType);
      factory.PrepareUnconstructedInstance (instance);
      instance.InitializeFromDataContainer (dataContainer);
      return instance;
    }

    public IInvokeWith<T> GetTypesafeConstructorInvoker<T> ()
       where T : DomainObject
    {
      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(typeof (T));
      return factory.GetTypesafeConstructorInvoker<T> (concreteType);
    }
  }
}
