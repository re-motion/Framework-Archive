using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class MixinA : MixinBase
  {
    public int P5
    {
      get { return Properties[typeof (MixinA), "P5"].GetValue<int>(); }
    }
  }
}