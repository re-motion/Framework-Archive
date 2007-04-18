using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutTimestampColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutTimestampColumn: TestDomainBase
  {
    protected ClassWithoutTimestampColumn()
    {
    }

    protected ClassWithoutTimestampColumn (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}