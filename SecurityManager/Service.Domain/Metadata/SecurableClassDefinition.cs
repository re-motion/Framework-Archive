using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.Metadata
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

    public SecurableClassDefinition ()
    {
    }

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

    public Guid MetadataItemID
    {
      get { return (Guid) DataContainer["MetadataItemID"]; }
      set { DataContainer["MetadataItemID"] = value; }
    }

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public SecurableClassDefinition BaseClass
    {
      get { return (SecurableClassDefinition) GetRelatedObject ("BaseClass"); }
      set { SetRelatedObject ("BaseClass", value); }
    }

    public Rubicon.Data.DomainObjects.DomainObjectCollection DerivedClasses
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("DerivedClasses"); }
      set { } // marks property DerivedClasses as modifiable
    }

    public Rubicon.Data.DomainObjects.DomainObjectCollection AccessTypeReferences
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("AccessTypeReferences"); }
      set { } // marks property AccessTypeReferences as modifiable
    }

    public Rubicon.Data.DomainObjects.DomainObjectCollection StatePropertyReferences
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("StatePropertyReferences"); }
      set { } // marks property StatePropertyReferences as modifiable
    }

  }
}
