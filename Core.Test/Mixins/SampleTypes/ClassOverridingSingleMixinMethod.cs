using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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
