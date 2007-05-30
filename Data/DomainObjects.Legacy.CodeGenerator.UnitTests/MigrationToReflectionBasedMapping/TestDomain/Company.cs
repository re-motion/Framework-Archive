using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.MigrationToReflectionBasedMapping.TestDomain
{
  [Serializable]
  [DBStorageGroup]
  public abstract class Company : BindableDomainObject
  {
    // types

    // static members and constants

    public static new Company GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Company> (id);
    }

    public static new Company GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Company.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected Company ()
    {
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string PhoneNumber { get; set; }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public abstract Ceo Ceo { get; set; }

    [DBBidirectionalRelation ("Company", ContainsForeignKey = true)]
    public abstract Address Address { get; set; }

  }
}
