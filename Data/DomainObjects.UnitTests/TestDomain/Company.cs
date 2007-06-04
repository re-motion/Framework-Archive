using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Company : TestDomainBase
  {
    public static Company NewObject ()
    {
      return NewObject<Company> ().With();
    }

    public new static Company GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Company> (id);
    }

    protected Company ()
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
    public virtual IndustrialSector IndustrialSector
    {
      get { return CurrentProperty<IndustrialSector>().GetValue(); }
      set { CurrentProperty<IndustrialSector> ().SetValue (value); }
    }

    [DBBidirectionalRelation ("Company")]
    private ClassWithoutRelatedClassIDColumnAndDerivation ClassWithoutRelatedClassIDColumnAndDerivation
    {
      get { return (ClassWithoutRelatedClassIDColumnAndDerivation) GetRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation"); }
      set { SetRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation", value); }
    }
  }
}