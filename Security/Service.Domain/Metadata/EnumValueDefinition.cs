using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.Metadata
{
  [Serializable]
  public class EnumValueDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static new EnumValueDefinition GetObject (ObjectID id)
    {
      return (EnumValueDefinition) DomainObject.GetObject (id);
    }

    public static new EnumValueDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (EnumValueDefinition) DomainObject.GetObject (id, includeDeleted);
    }

    public static new EnumValueDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (EnumValueDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new EnumValueDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (EnumValueDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public EnumValueDefinition ()
    {
    }

    public EnumValueDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected EnumValueDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public long Value
    {
      get { return (long) DataContainer["Value"]; }
      set { DataContainer["Value"] = value; }
    }

  }
}
