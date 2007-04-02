using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [MixinFor (typeof (BaseType3))]
  public class BT3Mixin3<TThis, TBase> : Mixin<TThis, TBase>
    where TThis : IBaseType33
    where TBase : IBaseType33
  {
    public new TThis This
    {
      get { return base.This; }
    }

    public new TBase Base
    {
      get { return base.Base; }
    }
  }
}
