using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class MixinB : MixinA
  {
    public int P6
    {
      get { return Properties[typeof (MixinB), "P6"].GetValue<int> (); }
    }
  }
}