using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.MigrationToReflectionBasedMapping.TestDomain
{
  [Instantiable]
  [DBStorageGroup]
  [DBTable]
  public abstract class Order : BindableDomainObject
  {
    // types

    // static members and constants

    public static Order NewObject ()
    {
      return DomainObject.NewObject<Order> ().With ();
    }

    public static Order NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return Order.NewObject ();
      }
    }

    public static new Order GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Order> (id);
    }

    public static new Order GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return Order.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected Order ()
    {
    }

    // methods and properties

    public abstract int Number { get; set; }

    public abstract OrderPriority Priority { get; set; }

    [DBBidirectionalRelation ("Orders")]
    [Mandatory]
    public abstract Customer Customer { get; set; }

    [DBBidirectionalRelation ("Orders")]
    public abstract Official Official { get; set; }

    [DBBidirectionalRelation ("Order")]
    [Mandatory]
    public abstract ObjectList<OrderItem> OrderItems { get; }

  }
}
