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
    public static AbstractRoleDefinition NewObject ()
    {
      return NewObject<AbstractRoleDefinition> ().With ();
    }

    public static AbstractRoleDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AbstractRoleDefinition> ().With (metadataItemID, name, value);
    }

    public static DomainObjectCollection Find (EnumWrapper[] abstractRoles)
    {
      if (abstractRoles.Length == 0)
        return new DomainObjectCollection ();

      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (queryBuilder.CreateQuery (abstractRoles));
    }

    public static DomainObjectCollection FindAll ()
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.Metadata.AbstractRoleDefinition.FindAll");
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
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
