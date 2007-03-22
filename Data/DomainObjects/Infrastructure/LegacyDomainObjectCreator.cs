using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Utilities;
using Rubicon.Logging;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  // Creates new domain object instances via a Reflection-based constructor call.
  // Needed constructors:
  // MyDomainObject () -- for the current transaction
  // MyDomainObject (ClientTransaction) -- for a given transaction
  // MyDomainObject (DataContainer) -- for loading
  class LegacyDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static LegacyDomainObjectCreator Instance = new LegacyDomainObjectCreator ();
    private static ILog s_log = LogManager.GetLogger (typeof (LegacyDomainObjectCreator));

    public DomainObject CreateWithCurrentTransaction (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return Create (type, new object[] { });
    }

    public DomainObject CreateWithTransaction (Type type, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      return Create (type, new object[] { clientTransaction });
    }

    public DomainObject CreateWithDataContainer (DataContainer dataContainer, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return Create (dataContainer.DomainObjectType, new object[] { dataContainer });
    }

    private DomainObject Create (Type type, object[] constructorArgs)
    {
      try
      {
        return (DomainObject) ReflectionUtility.CreateObject (type, constructorArgs);
      }
      catch (ArgumentException ex)
      {
        throw new MissingMethodException (string.Format("The given type {0} does not implement the required legacy constructor taking a " +
            "ClientTransaction only.", type.FullName), ex);
      }
      catch (TargetInvocationException ex)
      {
        s_log.Error ("TargetInvocationException in constructor call, inner exception is unwrapped and rethrown.", ex);
        throw ex.InnerException;
      }
    }
  }
}
