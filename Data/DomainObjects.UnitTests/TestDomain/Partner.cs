using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [NotAbstract]
  public abstract class Partner : Company
  {
    public static new Partner GetObject (ObjectID id)
    {
      return (Partner) DomainObject.GetObject (id);
    }

    public static Partner Create ()
    {
      return DomainObject.Create<Partner>();
    }

    protected Partner (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("AssociatedPartnerCompany", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Person ContactPerson { get; set; }
  }
}
