using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class Partner : Company
{
  // types

  // static members and constants

  public static new Partner GetObject (ObjectID id)
  {
    return (Partner) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public Partner ()
  {
  }

  protected Partner (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public Person ContactPerson
  {
    get { return (Person) GetRelatedObject ("ContactPerson"); }
    set { SetRelatedObject ("ContactPerson", value); }
  }

  private ClassWithoutRelatedClassIDColumn ClassWithoutRelatedClassIDColumn
  {
    get 
    { 
      return (ClassWithoutRelatedClassIDColumn) GetRelatedObject ("ClassWithoutRelatedClassIDColumn"); 
    }
    set 
    { 
      SetRelatedObject ("ClassWithoutRelatedClassIDColumn", value); 
    }
  }
}
}
