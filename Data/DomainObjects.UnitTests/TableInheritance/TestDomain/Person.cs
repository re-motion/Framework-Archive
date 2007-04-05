using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Person")]
  [DBTable (Name = "TableInheritance_Person")]
  [NotAbstract]
  public abstract class Person: DomainBase
  {
    public new static Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    public Person (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    protected Person (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [String (IsNullable = false, MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [String (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    public abstract DateTime DateOfBirth { get; set; }

    [DBBidirectionalRelation ("Person")]
    public abstract Address Address { get; }

    [Binary]
    public abstract byte[] Photo { get; set; }
  }
}