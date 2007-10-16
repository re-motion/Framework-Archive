using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class MixinE : DomainObjectMixin<DomainObject>
  {
    public int P9
    {
      get { return Properties[typeof (MixinE), "P9"].GetValue<int> (); }
    }
  }
}