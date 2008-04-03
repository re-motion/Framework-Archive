using System;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
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
      Assertion.IsTrue (dataContainer.ClassDefinition is ReflectionBasedClassDefinition);

      dataContainer.ClassDefinition.ValidateCurrentMixinConfiguration ();

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(dataContainer.DomainObjectType);
      DomainObject instance = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
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
      
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType);
      classDefinition.ValidateCurrentMixinConfiguration ();

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(domainObjectType);
      return factory.GetTypesafeConstructorInvoker<TMinimal> (concreteType);
    }
  }
}
