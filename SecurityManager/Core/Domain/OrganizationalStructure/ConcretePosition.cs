using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.ConcretePosition")]
  public class ConcretePosition : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new ConcretePosition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (ConcretePosition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new ConcretePosition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (ConcretePosition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public ConcretePosition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected ConcretePosition (DataContainer dataContainer)
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

    public GroupType GroupType
    {
      get { return (GroupType) GetRelatedObject ("GroupType"); }
      set { SetRelatedObject ("GroupType", value); }
    }

    public Position Position
    {
      get { return (Position) GetRelatedObject ("Position"); }
      set { SetRelatedObject ("Position", value); }
    }

    public override string DisplayName
    {
      get { return Name; }
    }
  }
}
