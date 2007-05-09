using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(AbstractMixin))]
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
