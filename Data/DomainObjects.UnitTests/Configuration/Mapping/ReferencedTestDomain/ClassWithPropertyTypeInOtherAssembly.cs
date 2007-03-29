using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain
{
  public abstract class ClassInOtherAssembly: DomainObject
  {
    protected ClassInOtherAssembly (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }
  }
}