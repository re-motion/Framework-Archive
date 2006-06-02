using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
{
  public class OrganizationalStructureObject : BaseSecurityServiceObject
  {
    // types

    // static members and constants

    public static new OrganizationalStructureObject GetObject (ObjectID id)
    {
      return (OrganizationalStructureObject) DomainObject.GetObject (id);
    }

    public static new OrganizationalStructureObject GetObject (ObjectID id, bool includeDeleted)
    {
      return (OrganizationalStructureObject) DomainObject.GetObject (id, includeDeleted);
    }

    public static new OrganizationalStructureObject GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (OrganizationalStructureObject) DomainObject.GetObject (id, clientTransaction);
    }

    public static new OrganizationalStructureObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (OrganizationalStructureObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public OrganizationalStructureObject ()
    {
    }

    public OrganizationalStructureObject (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected OrganizationalStructureObject (DataContainer dataContainer) : base (dataContainer)
    {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}
