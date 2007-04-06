using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  public class IndustrialSector : TestDomainBase
  {
    // types

    // static members and constants

    public static new IndustrialSector GetObject (ObjectID id)
    {
      return (IndustrialSector) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public IndustrialSector ()
    {
    }

    public IndustrialSector (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public IndustrialSector (ClientTransaction clientTransaction, ObjectID objectID)
      : base(clientTransaction, objectID)
    {
    }

    protected IndustrialSector (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public ObjectList<Company> Companies
    {
      get { return (ObjectList<Company>) GetRelatedObjects ("Companies"); }
    }
  }
}
