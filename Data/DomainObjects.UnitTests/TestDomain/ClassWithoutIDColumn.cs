using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutIDColumn")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutIDColumn : TestDomainBase
  {
    protected ClassWithoutIDColumn()
    {
    }

    protected ClassWithoutIDColumn (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}
