using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Computer : TestDomainBase
  {
    public new static Computer GetObject (ObjectID id)
    {
      return (Computer) DomainObject.GetObject (id);
    }

    public static Computer Create ()
    {
      return DomainObject.Create<Computer> ();
    }

    protected Computer (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public abstract string SerialNumber { get; set; }

    [DBBidirectionalRelation ("Computer", ContainsForeignKey = true)]
    public abstract Employee Employee { get; set; }
  }
}