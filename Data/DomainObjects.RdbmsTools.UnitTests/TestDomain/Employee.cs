using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class Employee : DomainObject
  {
    public static Employee NewObject ()
    {
      return NewObject<Employee> ().With();
    }

    protected Employee ()
    {
    }

    protected Employee (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Supervisor")]
    public abstract ObjectList<Employee> Subordinates { get; }

    [DBBidirectionalRelation ("Subordinates")]
    public abstract Employee Supervisor { get; set; }
  }
}
