using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutClassIDColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutClassIDColumn : TestDomainBase
  {
    protected ClassWithoutClassIDColumn ()
    {
    }

    protected ClassWithoutClassIDColumn (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}
