using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.MigrationToReflectionBasedMapping.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class Customer : Company
  {
    // types

    public enum CustomerType
    {
      DummyEntry = 0
    }

    // static members and constants

    public static Customer NewObject ()
    {
      return DomainObject.NewObject<Customer> ().With ();
    }

    public static Customer NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Customer.NewObject ();
      }
    }

    public static new Customer GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Customer> (id);
    }

    public static new Customer GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Customer.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected Customer ()
    {
    }

    // methods and properties

    [DBColumn ("CustomerType")]
    public abstract Customer.CustomerType Type { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches")]
    public abstract string PropertyWithIdenticalNameInDifferentInheritanceBranches { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "Number ASC")]
    public abstract OrderCollection Orders { get; }

    [Mandatory]
    public abstract Official PrimaryOfficial { get; set; }

  }
}
