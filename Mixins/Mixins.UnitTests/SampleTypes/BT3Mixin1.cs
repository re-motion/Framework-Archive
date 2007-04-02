using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [MixinFor (typeof (BaseType3))]
  public class BT3Mixin1 : Mixin<IBaseType31, IBaseType31>
  {
    public new IBaseType31 This
    {
      get { return base.This; }
    }

    public new IBaseType31 Base
    {
      get { return base.Base; }
    }
  }
}
