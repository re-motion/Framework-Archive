using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Company : TestDomainBase
  {
    public new static Company GetObject (ObjectID id)
    {
      return (Company) DomainObject.GetObject (id);
    }

    public static Company NewObject ()
    {
      return DomainObject.NewObject<Company> ().With();
    }

    protected Company ()
    {
    }

    protected Company (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [StorageClassNone]
    internal int NamePropertyOfInvalidType
    {
      set { SetPropertyValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name", value); }
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public abstract Ceo Ceo { get; set; }

    [DBBidirectionalRelation ("Companies")]
    public abstract IndustrialSector IndustrialSector { get; set; }

    [DBBidirectionalRelation ("Company")]
    private ClassWithoutRelatedClassIDColumnAndDerivation ClassWithoutRelatedClassIDColumnAndDerivation
    {
      get { return (ClassWithoutRelatedClassIDColumnAndDerivation) GetRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation"); }
      set { SetRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation", value); }
    }
  }
}