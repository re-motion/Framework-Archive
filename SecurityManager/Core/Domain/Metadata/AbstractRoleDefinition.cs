using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class AbstractRoleDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static new AbstractRoleDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AbstractRoleDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AbstractRoleDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AbstractRoleDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection Find (EnumWrapper[] abstractRoles, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      if (abstractRoles.Length == 0)
        return new DomainObjectCollection ();

      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      return clientTransaction.QueryManager.GetCollection (queryBuilder.CreateQuery (abstractRoles));
    }

    public static DomainObjectCollection FindAll (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.Metadata.AbstractRoleDefinition.FindAll");
      return clientTransaction.QueryManager.GetCollection (query);
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

    private DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
    }
  }
}
