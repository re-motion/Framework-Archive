using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [MixinFor (typeof (BaseType3))]
  public class BT3Mixin4 : BT3Mixin3<BaseType3, IBaseType34>
  {
  }
}
