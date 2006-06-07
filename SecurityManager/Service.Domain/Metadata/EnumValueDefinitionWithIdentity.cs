using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.Metadata
{
  [Serializable]
  public class EnumValueDefinitionWithIdentity : EnumValueDefinition
  {
    // types

    // static members and constants

    public static new EnumValueDefinitionWithIdentity GetObject (ObjectID id)
    {
      return (EnumValueDefinitionWithIdentity) DomainObject.GetObject (id);
    }

    public static new EnumValueDefinitionWithIdentity GetObject (ObjectID id, bool includeDeleted)
    {
      return (EnumValueDefinitionWithIdentity) DomainObject.GetObject (id, includeDeleted);
    }

    public static new EnumValueDefinitionWithIdentity GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (EnumValueDefinitionWithIdentity) DomainObject.GetObject (id, clientTransaction);
    }

    public static new EnumValueDefinitionWithIdentity GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (EnumValueDefinitionWithIdentity) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public EnumValueDefinitionWithIdentity ()
    {
    }

    public EnumValueDefinitionWithIdentity (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected EnumValueDefinitionWithIdentity (DataContainer dataContainer)
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

  }
}
