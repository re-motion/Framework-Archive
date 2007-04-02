using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  public class Person : TestDomainBase
  {
    // types

    // static members and constants

    public static new Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Person ()
    {
    }

    public Person (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Person (ClientTransaction clientTransaction, ObjectID objectID)
      : base(clientTransaction, objectID)
    {
    }

    protected Person (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    [String (IsNullable = false, MaximumLength = 100)]
    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    [DBBidirectionalRelation ("ContactPerson")]
    public Partner AssociatedPartnerCompany
    {
      get { return (Partner) GetRelatedObject ("AssociatedPartnerCompany"); }
      set { SetRelatedObject ("AssociatedPartnerCompany", value); }
    }
  }
}
