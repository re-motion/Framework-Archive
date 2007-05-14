using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  public class ClassNotInMapping : DomainObject
  {
    protected ClassNotInMapping ()
    {
    }
  }
}
