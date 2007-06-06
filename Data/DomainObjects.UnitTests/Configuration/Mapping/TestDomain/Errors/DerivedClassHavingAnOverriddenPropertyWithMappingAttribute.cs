using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class DerivedClassHavingAnOverriddenPropertyWithMappingAttribute: BaseClass
  {
    protected DerivedClassHavingAnOverriddenPropertyWithMappingAttribute ()
    {
    }

    [StorageClassNoneAttribute]
    public override int Int32
    {
      get { return 0; }
      set { }
    }
  }
}