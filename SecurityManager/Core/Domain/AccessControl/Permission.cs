using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class Permission : AccessControlObject
  {
    // types

    // static members and constants

    public static new Permission GetObject (ObjectID id)
    {
      return (Permission) DomainObject.GetObject (id);
    }

    public static new Permission GetObject (ObjectID id, bool includeDeleted)
    {
      return (Permission) DomainObject.GetObject (id, includeDeleted);
    }

    public static new Permission GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Permission) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Permission GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Permission) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public Permission (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Permission (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public int Index
    {
      get { return (int) DataContainer["Index"]; }
      set { DataContainer["Index"] = value; }
    }

    public bool BinaryAllowed
    {
      get { return Allowed.IsTrue; }
      set { Allowed = value ? NaBoolean.True : NaBoolean.Null;  }
    }

    public NaBoolean Allowed
    {
      get { return (NaBoolean) DataContainer["Allowed"]; }
      set { DataContainer["Allowed"] = value; }
    }

    public AccessTypeDefinition AccessType
    {
      get { return (Rubicon.SecurityManager.Domain.Metadata.AccessTypeDefinition) GetRelatedObject ("AccessType"); }
      set { SetRelatedObject ("AccessType", value); }
    }

    public AccessControlEntry AccessControlEntry
    {
      get { return (AccessControlEntry) GetRelatedObject ("AccessControlEntry"); }
    }
  }
}
