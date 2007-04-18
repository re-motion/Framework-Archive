using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttribute")]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassHavingClassIDAttribute : DomainObject
  {
    protected ClassHavingClassIDAttribute ()
    {
    }

    protected ClassHavingClassIDAttribute (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}