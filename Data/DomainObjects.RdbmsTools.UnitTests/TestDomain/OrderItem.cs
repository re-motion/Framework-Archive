using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class OrderItem : DomainObject
  {
    public static OrderItem NewObject ()
    {
      return NewObject<OrderItem> ().With ();
    }

    protected OrderItem ()
    {
    }

    protected OrderItem (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract int Position { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Product { get; set; }

    [DBBidirectionalRelation ("OrderItems")]
    [Mandatory]
    public abstract Order Order { get; set; }
  }
}
