using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class StateDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static new StateDefinition GetObject (ObjectID id)
    {
      return (StateDefinition) DomainObject.GetObject (id);
    }

    public static new StateDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (StateDefinition) DomainObject.GetObject (id, includeDeleted);
    }

    public static new StateDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StateDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new StateDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (StateDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public StateDefinition ()
    {
    }

    public StateDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected StateDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public StatePropertyDefinition StateProperty
    {
      get { return (StatePropertyDefinition) GetRelatedObject ("StateProperty"); }
      set { SetRelatedObject ("StateProperty", value); }
    }

  }
}
