using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [TestDomain]
  public abstract class ReflectionBasedMappingTestDomainBase : DomainObject
  {
    protected ReflectionBasedMappingTestDomainBase (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }
  }
}
