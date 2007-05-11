using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithKeyOfInvalidType")]
  [TestDomain]
  [Instantiable]
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
