using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.Metadata
{
  [Serializable]
  public class AbstractRoleDefinition : EnumValueDefinitionWithIdentity
  {
    // types

    // static members and constants

    public static new AbstractRoleDefinition GetObject (ObjectID id)
    {
      return (AbstractRoleDefinition) DomainObject.GetObject (id);
    }

    public static new AbstractRoleDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (AbstractRoleDefinition) DomainObject.GetObject (id, includeDeleted);
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

    public AbstractRoleDefinition ()
    {
    }

    public AbstractRoleDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AbstractRoleDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

  }
}
