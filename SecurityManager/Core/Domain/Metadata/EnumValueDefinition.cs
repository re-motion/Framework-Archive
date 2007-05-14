using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public abstract class EnumValueDefinition : MetadataObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

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

    public int Value
    {
      get { return (int) DataContainer["Value"]; }
      set { DataContainer["Value"] = value; }
    }

  }
}
