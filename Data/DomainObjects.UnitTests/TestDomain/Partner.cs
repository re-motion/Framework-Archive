using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Partner : Company
  {
    public static new Partner NewObject ()
    {
      return NewObject<Partner>().With();
    }

    protected Partner ()
    {
    }

    protected Partner (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("AssociatedPartnerCompany", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Person ContactPerson { get; set; }
  }
}
