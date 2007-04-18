using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class AbstractClassNotInMapping: DomainObject
  {
    public static AbstractClassNotInMapping NewObject()
    {
      return DomainObject.NewObject<AbstractClassNotInMapping>().With();
    }

    protected AbstractClassNotInMapping()
    {
    }

    protected AbstractClassNotInMapping (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}