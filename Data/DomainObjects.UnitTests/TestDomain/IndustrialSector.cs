using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class IndustrialSector : TestDomainBase
  {
    public static new IndustrialSector GetObject (ObjectID id)
    {
      return (IndustrialSector) DomainObject.GetObject (id);
    }

    public static IndustrialSector Create ()
    {
      return DomainObject.Create<IndustrialSector> ();
    }

    protected IndustrialSector (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public virtual ObjectList<Company> Companies { get { return (ObjectList<Company>) GetRelatedObjects(); } }
  }
}
