using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class MixinD : DomainObjectMixin<DomainObject>
  {
    public int P8
    {
      get { return Properties[typeof (MixinD), "P8"].GetValue<int> (); }
    }
  }
}