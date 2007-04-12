using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutTimestampColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutTimestampColumn : TestDomainBase
  {
    protected ClassWithoutTimestampColumn (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}
