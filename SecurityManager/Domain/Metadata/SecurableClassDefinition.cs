using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class SecurableClassDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static new SecurableClassDefinition GetObject (ObjectID id)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id, includeDeleted);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new SecurableClassDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (SecurableClassDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public SecurableClassDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected SecurableClassDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public SecurableClassDefinition BaseClass
    {
      get { return (SecurableClassDefinition) GetRelatedObject ("BaseClass"); }
      set { SetRelatedObject ("BaseClass", value); }
    }

    public DomainObjectCollection DerivedClasses
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("DerivedClasses"); }
      set { } // marks property DerivedClasses as modifiable
    }

    public DomainObjectCollection AccessTypeReferences
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessTypeReferences"); }
      set { } // marks property AccessTypeReferences as modifiable
    }

    public DomainObjectCollection StatePropertyReferences
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("StatePropertyReferences"); }
      set { } // marks property StatePropertyReferences as modifiable
    }

    public DomainObjectCollection StateCombinations
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("StateCombinations"); }
      set { } // marks property StateCombinations as modifiable
    }

    public DomainObjectCollection AccessControlLists
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlLists"); }
      set { } // marks property AccessControlLists as modifiable
    }

    public void AddAccessType (AccessTypeDefinition accessType)
    {
      AccessTypeReference reference = new AccessTypeReference (this.ClientTransaction);
      reference.AccessType = accessType;

      AccessTypeReferences.Add (reference);
    }

    public void AddStateProperty (StatePropertyDefinition stateProperty)
    {
      StatePropertyReference reference = new StatePropertyReference (this.ClientTransaction);
      reference.StateProperty = stateProperty;

      StatePropertyReferences.Add (reference);
    }
  }
}
