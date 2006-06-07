using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.Metadata
{
  [Serializable]
  public class StatePropertyReference : MetadataObject
  {
    // types

    // static members and constants

    public static new StatePropertyReference GetObject (ObjectID id)
    {
      return (StatePropertyReference) DomainObject.GetObject (id);
    }

    public static new StatePropertyReference GetObject (ObjectID id, bool includeDeleted)
    {
      return (StatePropertyReference) DomainObject.GetObject (id, includeDeleted);
    }

    public static new StatePropertyReference GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StatePropertyReference) DomainObject.GetObject (id, clientTransaction);
    }

    public static new StatePropertyReference GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (StatePropertyReference) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public StatePropertyReference ()
    {
    }

    public StatePropertyReference (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected StatePropertyReference (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public SecurableClassDefinition Class
    {
      get { return (SecurableClassDefinition) GetRelatedObject ("Class"); }
      set { SetRelatedObject ("Class", value); }
    }

    public StatePropertyDefinition StateProperty
    {
      get { return (StatePropertyDefinition) GetRelatedObject ("StateProperty"); }
      set { SetRelatedObject ("StateProperty", value); }
    }

  }
}
