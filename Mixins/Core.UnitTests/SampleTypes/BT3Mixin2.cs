using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin2 : Mixin<IBaseType32>
  {
    public new IBaseType32 This
    {
      get { return base.This; }
    }
  }

  [Serializable]
  public class BT3Mixin2B : Mixin<IBaseType32>
  {
    public new IBaseType32 This
    {
      get { return base.This; }
    }
  }
}
