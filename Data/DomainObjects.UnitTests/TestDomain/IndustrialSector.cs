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
    public virtual string Name
    {
      get
      {
        return CurrentProperty<string>().GetValue();
      }
      set
      {
        CurrentProperty<string>().SetValue (value);
      }
    }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public virtual ObjectList<Company> Companies
    {
      get
      {
        return CurrentProperty<ObjectList<Company>>().GetValue();
      }
    }
  }
}
