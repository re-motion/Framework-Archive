using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithKeyOfInvalidType")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithKeyOfInvalidType : TestDomainBase
  {
    protected ClassWithKeyOfInvalidType ()
    {
    }

    protected ClassWithKeyOfInvalidType (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}
