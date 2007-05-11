using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  public abstract class AbstractWithoutConcreteClass : Company
  {
    public static AbstractWithoutConcreteClass NewObject()
    {
      return NewObject<AbstractWithoutConcreteClass>().With();
    }

    protected AbstractWithoutConcreteClass()
    {
    }

    protected AbstractWithoutConcreteClass (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}