using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(AbstractMixin))]
  [Serializable]
  public class ClassOverridingMixinMethod
  {
    [Override]
    public string AbstractMethod(int i)
    {
      return "ClassOverridingMixinMethod.AbstractMethod-" + i;
    }

    public virtual string OverridableMethod (int i)
    {
      return "ClassOverridingMixinMethod-" + i;
    }
  }
}
