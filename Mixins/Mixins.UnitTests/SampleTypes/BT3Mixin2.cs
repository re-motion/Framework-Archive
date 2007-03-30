using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  [Mixin (typeof (BaseType3))]
  public class BT3Mixin2 : Mixin<IBaseType3>
  {
    public new IBaseType3 This
    {
      get { return base.This; }
    }

    public new object Base
    {
      get { return base.Base; }
    }
  }
}
