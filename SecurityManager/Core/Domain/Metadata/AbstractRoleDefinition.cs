using System;
using System.ComponentModel;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class AbstractRoleDefinition : EnumValueDefinition
  {
    public static AbstractRoleDefinition NewObject (ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return NewObject<AbstractRoleDefinition> ().With ();
      }
    }

    public static AbstractRoleDefinition NewObject (ClientTransaction clientTransaction, Guid metadataItemID, string name, int value)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return NewObject<AbstractRoleDefinition> ().With (metadataItemID, name, value);
      }
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

    protected AbstractRoleDefinition ()
    {
    }

    protected AbstractRoleDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }

    [DBBidirectionalRelation ("SpecificAbstractRole")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }
  }
}
