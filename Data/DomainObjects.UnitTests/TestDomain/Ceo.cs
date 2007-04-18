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
    
    public static Ceo NewObject ()
    {
      return DomainObject.NewObject<Ceo> ().With();
    }

    protected Ceo ()
    {
    }

    protected Ceo (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Ceo", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Company Company { get; set; }
  }
}
