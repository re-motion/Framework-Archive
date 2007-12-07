using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.MigrationToReflectionBasedMapping.TestDomain
{
  [Instantiable]
  [DBStorageGroup]
  [DBTable]
  public abstract class OrderItem : BindableDomainObject
  {
    // types

    // static members and constants

    public static OrderItem NewObject ()
    {
      return DomainObject.NewObject<OrderItem> ().With ();
    }

    public static OrderItem NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return OrderItem.NewObject ();
      }
    }

    public static new OrderItem GetObject (ObjectID id)
    {
      return DomainObject.GetObject<OrderItem> (id);
    }

    public static new OrderItem GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return OrderItem.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected OrderItem ()
    {
    }

    // methods and properties

    public abstract int Position { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Product { get; set; }

    [DBBidirectionalRelation ("OrderItems")]
    [Mandatory]
    public abstract Order Order { get; set; }

  }
}
