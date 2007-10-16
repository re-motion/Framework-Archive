using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class MixinBase : DomainObjectMixin<DomainObject>
  {
    public int P0a
    {
      get { return Properties[typeof (MixinBase), "P0a"].GetValue<int> (); }
    }
  }
}