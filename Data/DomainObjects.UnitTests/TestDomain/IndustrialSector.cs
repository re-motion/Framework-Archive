using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class IndustrialSector : TestDomainBase
  {
    public static IndustrialSector NewObject ()
    {
      return DomainObject.NewObject<IndustrialSector> ().With ();
    }

    protected IndustrialSector ()
    {
    }

    protected IndustrialSector (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public abstract ObjectList<Company> Companies { get; }
  }
}
