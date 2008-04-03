using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(MixinWithSingleAbstractMethod))]
  [Serializable]
  public class ClassOverridingSingleMixinMethod
  {
    [OverrideMixin]
    public string AbstractMethod(int i)
    {
      return "ClassOverridingSingleMixinMethod.AbstractMethod-" + i;
    }

    public virtual string OverridableMethod (int i)
    {
      return "ClassOverridingSingleMixinMethod-" + i;
    }
  }
}
