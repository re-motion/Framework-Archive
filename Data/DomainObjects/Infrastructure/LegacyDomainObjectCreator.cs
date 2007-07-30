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
  internal class LegacyDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static LegacyDomainObjectCreator Instance = new LegacyDomainObjectCreator ();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      return (DomainObject) TypesafeActivator.CreateInstance (dataContainer.DomainObjectType, bindingFlags).With (dataContainer);
    }

    public IFuncInvoker<T> GetTypesafeConstructorInvoker<T> ()
       where T : DomainObject
    {
      return new FuncInvoker<T> (
          delegate (Type delegateType)
          {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return ConstructorWrapper.CreateDelegate (typeof (T), delegateType, bindingFlags, null, CallingConventions.Any, null);
          });
    }
  }
}
