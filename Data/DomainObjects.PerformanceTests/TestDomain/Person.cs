using System;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  [NotAbstract]
  [DBTable]
  public abstract class Person: ClientBoundBaseClass
  {
    public static Person NewObject()
    {
      return NewObject<Person>().With();
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
  }
}