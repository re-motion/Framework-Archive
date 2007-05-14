using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class AbstractClassNotInMapping: DomainObject
  {
    public static AbstractClassNotInMapping NewObject()
    {
      return NewObject<AbstractClassNotInMapping>().With();
    }

    protected AbstractClassNotInMapping()
    {
    }
  }
}