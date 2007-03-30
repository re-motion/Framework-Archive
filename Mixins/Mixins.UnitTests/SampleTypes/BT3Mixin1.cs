using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [Mixin (typeof (BaseType3))]
  public class BT3Mixin1 : Mixin<IBaseType3, IBaseType3>
  {
    public new IBaseType3 This
    {
      get { return base.This; }
    }

    public new IBaseType3 Base
    {
      get { return base.Base; }
    }
  }
}
