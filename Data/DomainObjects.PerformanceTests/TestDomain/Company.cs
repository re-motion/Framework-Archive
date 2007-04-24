using System;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  [NotAbstract]
  [DBTable]
  public abstract class Company: ClientBoundBaseClass
  {
    public static Company NewObject()
    {
      return NewObject<Company>().With();
    }

    protected Company()
    {
    }

    protected Company (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}