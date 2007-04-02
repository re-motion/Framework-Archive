using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [MixinFor (typeof (BaseType3))]
  public class BT3Mixin2 : Mixin<IBaseType32>
  {
    public new IBaseType32 This
    {
      get { return base.This; }
    }

    public new object Base
    {
      get { return base.Base; }
    }
  }
}
