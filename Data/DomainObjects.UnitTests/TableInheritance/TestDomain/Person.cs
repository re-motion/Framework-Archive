using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Person")]
  [DBTable ("TableInheritance_Person")]
  [Instantiable]
  public abstract class Person: DomainBase
  {
    public new static Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    public static Person NewObject ()
    {
      return NewObject<Person> ().With ();
    }

    protected Person()
    {
    }

    protected Person (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    public abstract DateTime DateOfBirth { get; set; }

    [DBBidirectionalRelation ("Person")]
    public abstract Address Address { get; }

    [BinaryProperty]
    public abstract byte[] Photo { get; set; }
  }
}