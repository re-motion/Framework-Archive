using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutTimestampColumn")]
  [TestDomain]
  [Instantiable]
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