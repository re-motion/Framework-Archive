using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Security;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using System.Text;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class AbstractRoleDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static DomainObjectCollection Find (ClientTransaction transaction, EnumWrapper[] abstractRoles)
    {
      if (abstractRoles.Length == 0)
        return new DomainObjectCollection ();

      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      return transaction.QueryManager.GetCollection (queryBuilder.CreateQuery (abstractRoles));
    }

    public static new AbstractRoleDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AbstractRoleDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AbstractRoleDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AbstractRoleDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public AbstractRoleDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public AbstractRoleDefinition (ClientTransaction clientTransaction, Guid metadataItemID, string name, int value)
      : base (clientTransaction)
    {
      DataContainer["MetadataItemID"] = metadataItemID;
      DataContainer["Name"] = name;
      DataContainer["Value"] = value;
    }

    protected AbstractRoleDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
      set { } // marks property AccessControlEntries as modifiable
    }
  }
}
