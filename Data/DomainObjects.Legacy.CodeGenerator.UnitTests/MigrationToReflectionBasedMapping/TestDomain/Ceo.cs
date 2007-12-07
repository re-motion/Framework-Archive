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
  public abstract class Ceo : BindableDomainObject
  {
    // types

    // static members and constants

    public static Ceo NewObject ()
    {
      return DomainObject.NewObject<Ceo> ().With ();
    }

    public static Ceo NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return Ceo.NewObject ();
      }
    }

    public static new Ceo GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Ceo> (id);
    }

    public static new Ceo GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return Ceo.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected Ceo ()
    {
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Ceo", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Company Company { get; set; }

  }
}
