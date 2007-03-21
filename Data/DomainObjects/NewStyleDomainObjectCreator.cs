using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Utilities;
using System.Reflection;
using Rubicon.Logging;

namespace Rubicon.Data.DomainObjects
{
  class NewStyleDomainObjectCreator : IDomainObjectCreator
  {
    public readonly static NewStyleDomainObjectCreator Instance = new NewStyleDomainObjectCreator ();
    private static ILog s_log = LogManager.GetLogger (typeof (NewStyleDomainObjectCreator));

    public DomainObject CreateWithCurrentTransaction (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return Create (type, new object[] { ClientTransaction.Current, null });
    }

    public DomainObject CreateWithTransaction (Type type, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      return Create (type, new object[] { clientTransaction, null });
    }

    public DomainObject CreateWithDataContainer (DataContainer dataContainer, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      DomainObject obj = Create (dataContainer.DomainObjectType, new object[] { null, objectID });
      obj.DataContainer = dataContainer;
      return obj;
    }

    private DomainObject Create (Type type, object[] constructorArgs)
    {
      try
      {
        return (DomainObject) DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.Create (type, constructorArgs);
      }
      catch (TargetInvocationException ex)
      {
        s_log.Error ("TargetInvocationException in constructor call, inner exception is unwrapped and rethrown.", ex);
        throw ex.InnerException;
      }
    }
  }
}
