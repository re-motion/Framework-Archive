using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class AccessControlObject : BaseSecurityServiceObject
  {
    // types

    // static members and constants

    public static new AccessControlObject GetObject (ObjectID id)
    {
      return (AccessControlObject) DomainObject.GetObject (id);
    }

    public static new AccessControlObject GetObject (ObjectID id, bool includeDeleted)
    {
      return (AccessControlObject) DomainObject.GetObject (id, includeDeleted);
    }

    public static new AccessControlObject GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AccessControlObject) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AccessControlObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AccessControlObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public AccessControlObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AccessControlObject (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}
