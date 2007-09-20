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
  // Creates new domain object instances via the DPInterceptedDomainObjectFactory.
  // Needed constructors:
  // (any constructor) -- for new objects
  // no constructor for loading required
  internal class FactoryBasedDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static FactoryBasedDomainObjectCreator Instance = new FactoryBasedDomainObjectCreator ();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(dataContainer.DomainObjectType);
      DomainObject instance = (DomainObject) System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject (concreteType);
      factory.PrepareUnconstructedInstance (instance);
      instance.InitializeFromDataContainer (dataContainer);
      return instance;
    }

    public IFuncInvoker<T> GetTypesafeConstructorInvoker<T> ()
       where T : DomainObject
    {
      return GetTypesafeConstructorInvoker<T> (typeof (T));
    }

    public IFuncInvoker<DomainObject> GetTypesafeConstructorInvoker (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      return GetTypesafeConstructorInvoker<DomainObject> (type);
    }

    private IFuncInvoker<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type domainObjectType)
        where TMinimal : DomainObject
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(domainObjectType);
      return factory.GetTypesafeConstructorInvoker<TMinimal> (concreteType);
    }
  }
}
