using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin3<TThis, TBase> : Mixin<TThis, TBase>
    where TThis : class, IBaseType33
    where TBase : class, IBaseType33
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
