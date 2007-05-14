using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class IndustrialSector : TestDomainBase
  {
    public static IndustrialSector NewObject ()
    {
      return NewObject<IndustrialSector> ().With ();
    }

    protected IndustrialSector ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public abstract ObjectList<Company> Companies { get; }
  }
}
