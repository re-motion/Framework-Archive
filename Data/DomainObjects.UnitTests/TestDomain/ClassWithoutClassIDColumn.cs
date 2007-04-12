using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutClassIDColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutClassIDColumn : TestDomainBase
  {
    protected ClassWithoutClassIDColumn (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}
