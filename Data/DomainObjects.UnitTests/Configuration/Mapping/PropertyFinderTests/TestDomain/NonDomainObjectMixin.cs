using System;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  public class NonDomainObjectMixin : Mixin<DomainObject>
  {
    public int PNo
    {
      get { return 0; }
      set { Dev.Null = value; }
    }
  }
}