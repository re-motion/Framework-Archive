using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class MixinC : DomainObjectMixin<DomainObject>
  {
    public int P7
    {
      get { return Properties[typeof (MixinC), "P7"].GetValue<int>(); }
    }
  }
}