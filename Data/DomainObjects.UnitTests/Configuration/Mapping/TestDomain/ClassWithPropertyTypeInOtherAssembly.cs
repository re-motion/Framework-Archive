using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain
{
  public abstract class ClassWithPropertyTypeInOtherAssembly: DomainObject
  {
    protected ClassWithPropertyTypeInOtherAssembly ()
    {
    }

    protected ClassWithPropertyTypeInOtherAssembly (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract ClassInOtherAssembly Property { get; set; }
  }
}