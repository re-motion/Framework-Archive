using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  public class Company: TestDomainBase
  {
    // types

    // static members and constants

    public new static Company GetObject (ObjectID id)
    {
      return (Company) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    // New customers cannot be created directly.
    protected Company()
    {
    }

    // New customers cannot be created directly.
    protected Company (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    public Company (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    protected Company (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

    [StorageClassNone]
    internal int NamePropertyOfInvalidType
    {
      set { DataContainer["Name"] = value; }
    }

    [String (IsNullable = false, MaximumLength = 100)]
    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public Ceo Ceo
    {
      get { return (Ceo) GetRelatedObject ("Ceo"); }
      set { SetRelatedObject ("Ceo", value); }
    }

    [DBBidirectionalRelation ("Companies")]
    public IndustrialSector IndustrialSector
    {
      get { return (IndustrialSector) GetRelatedObject ("IndustrialSector"); }
      set { SetRelatedObject ("IndustrialSector", value); }
    }

    [DBBidirectionalRelation ("Company")]
    private ClassWithoutRelatedClassIDColumnAndDerivation ClassWithoutRelatedClassIDColumnAndDerivation
    {
      get { return (ClassWithoutRelatedClassIDColumnAndDerivation) GetRelatedObject ("ClassWithoutRelatedClassIDColumnAndDerivation"); }
      set { SetRelatedObject ("ClassWithoutRelatedClassIDColumnAndDerivation", value); }
    }
  }
}