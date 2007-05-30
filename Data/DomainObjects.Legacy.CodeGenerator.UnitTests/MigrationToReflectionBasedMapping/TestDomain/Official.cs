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
  public abstract class Official : BindableDomainObject
  {
    // types

    // static members and constants

    public static Official NewObject ()
    {
      return DomainObject.NewObject<Official> ().With ();
    }

    public static Official NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Official.NewObject ();
      }
    }

    public static new Official GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Official> (id);
    }

    public static new Official GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Official.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected Official ()
    {
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract OrderPriority ResponsibleForOrderPriority { get; set; }

    public abstract Customer.CustomerType ResponsibleForCustomerType { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract OrderCollection Orders { get; }

  }
}
