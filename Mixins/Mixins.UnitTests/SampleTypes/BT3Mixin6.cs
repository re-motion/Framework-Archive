using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [Extends(typeof(BaseType3))]
  public class BT3Mixin6<TThis, TBase> : Mixin<TThis, TBase>
      where TThis : class, IBaseType31, IBaseType32, IBaseType33, IBT3Mixin4
      where TBase : class, IBaseType34, IBT3Mixin4
  {
  }
}
