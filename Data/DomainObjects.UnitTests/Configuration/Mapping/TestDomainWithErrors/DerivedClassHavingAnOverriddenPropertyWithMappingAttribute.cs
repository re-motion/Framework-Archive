using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class DerivedClassHavingAnOverriddenPropertyWithMappingAttribute: BaseClass
  {
    protected DerivedClassHavingAnOverriddenPropertyWithMappingAttribute ()
    {
    }

    protected DerivedClassHavingAnOverriddenPropertyWithMappingAttribute (DataContainer dataContainer)
        : base (dataContainer)
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