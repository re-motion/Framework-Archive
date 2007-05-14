using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttribute")]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassHavingClassIDAttribute : DomainObject
  {
    protected ClassHavingClassIDAttribute ()
    {
    }
  }
}