using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
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

  public Person (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Person (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Name
  {
    get { return (string) DataContainer["Name"]; }
    set { DataContainer["Name"] = value; }
  }

  public Partner AssociatedPartnerCompany
  {
    get { return (Partner) GetRelatedObject ("AssociatedPartnerCompany"); }
    set { SetRelatedObject ("AssociatedPartnerCompany", value); }
  }
}
}
