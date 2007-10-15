using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(MixinWithSingleAbstractMethod))]
  [Serializable]
  public class ClassOverridingSingleMixinMethod
  {
    [OverrideMixinMember]
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
