using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutIDColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutIDColumn : TestDomainBase
  {
    protected ClassWithoutIDColumn (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}
