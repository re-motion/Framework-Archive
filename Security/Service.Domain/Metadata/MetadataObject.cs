using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Service.Domain.Metadata
{
  public class MetadataObject : BaseSecurityServiceObject
  {
    // types

    // static members and constants

    public static new MetadataObject GetObject (ObjectID id)
    {
      return (MetadataObject) DomainObject.GetObject (id);
    }

    public static new MetadataObject GetObject (ObjectID id, bool includeDeleted)
    {
      return (MetadataObject) DomainObject.GetObject (id, includeDeleted);
    }

    public static new MetadataObject GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (MetadataObject) DomainObject.GetObject (id, clientTransaction);
    }

    public static new MetadataObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (MetadataObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public MetadataObject ()
    {
    }

    public MetadataObject (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected MetadataObject (DataContainer dataContainer) : base (dataContainer)
    {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}
