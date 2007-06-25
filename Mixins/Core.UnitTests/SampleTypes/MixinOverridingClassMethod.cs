using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Serializable]
  public class MixinOverridingClassMethod : Mixin<object>, IMixinOverridingClassMethod
  {
    [Override]
    public string OverridableMethod (int i)
    {
      return "MixinOverridingClassMethod.OverridableMethod-" + i;
    }

    public virtual string AbstractMethod (int i)
    {
      return "MixinOverridingClassMethod.AbstractMethod-" + i;
    }
  }

  public interface IMixinOverridingClassMethod
  {
    string AbstractMethod (int i);
  }
}
