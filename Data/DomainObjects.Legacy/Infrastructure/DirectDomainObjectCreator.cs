using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.Infrastructure
{
  // Creates new domain object instances via a Reflection-based constructor call.
  // Needed constructors:
  // MyDomainObject (DataContainer) -- for loading
  // (any constructor) -- for new objects
  internal class DirectDomainObjectCreator : IDomainObjectCreator
  {
    public static readonly DirectDomainObjectCreator Instance = new DirectDomainObjectCreator();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      return (DomainObject) TypesafeActivator.CreateInstance (dataContainer.DomainObjectType, bindingFlags).With (dataContainer);
    }

    public IFuncInvoker<T> GetTypesafeConstructorInvoker<T> ()
        where T: DomainObject
    {
      return TypesafeActivator.CreateInstance<T> (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public IFuncInvoker<DomainObject> GetTypesafeConstructorInvoker (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("domainObjectType", domainObjectType, typeof (DomainObject));
      return TypesafeActivator.CreateInstance<DomainObject> (domainObjectType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
  }
}