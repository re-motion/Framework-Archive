using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType3))]
  [Serializable]
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

    public new MixinDefinition Configuration
    {
      get { return base.Configuration; }
    }
  }

  [Serializable]
  public class BT3Mixin1B : Mixin<IBaseType31, IBaseType31>
  {
    public new IBaseType31 This
    {
      get { return base.This; }
    }

    public new IBaseType31 Base
    {
      get { return base.Base; }
    }

    public new MixinDefinition Configuration
    {
      get { return base.Configuration; }
    }
  }
}
