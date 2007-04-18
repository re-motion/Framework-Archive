using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain
{
  public abstract class ClassInOtherAssembly: DomainObject
  {
    protected ClassInOtherAssembly ()
    {
    }

    protected ClassInOtherAssembly (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}