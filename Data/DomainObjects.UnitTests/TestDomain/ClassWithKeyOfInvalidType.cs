using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithKeyOfInvalidType")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithKeyOfInvalidType : TestDomainBase
  {
    protected ClassWithKeyOfInvalidType (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}
