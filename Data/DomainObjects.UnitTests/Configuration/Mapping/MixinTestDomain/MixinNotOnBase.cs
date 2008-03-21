using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  public class MixinNotOnBase : DomainObjectMixin<DomainObject>
  {
    public int MixinProperty
    {
      get { return Properties[typeof (MixinNotOnBase), "MixinProperty"].GetValue<int> (); }
    }
  }
}