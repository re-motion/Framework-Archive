using System;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  [NotAbstract]
  [DBTable]
  public abstract class File : DomainObject
  {
    public static File NewObject()
    {
      return NewObject<File>().With();
    }

    protected File()
    {
    }

    protected File (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Number { get; set; }

    [DBBidirectionalRelation ("Files")]
    [Mandatory]
    public abstract Client Client { get; set; }
  }
}