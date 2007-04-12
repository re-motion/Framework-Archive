using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Ceo : TestDomainBase
  {
    public static Ceo GetObject (ObjectID id)
    {
      return (Ceo) DomainObject.GetObject (id);
    }
    
    public static Ceo Create ()
    {
      return DomainObject.Create<Ceo> ();
    }

    protected Ceo (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Ceo", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Company Company { get; set; }
  }
}
