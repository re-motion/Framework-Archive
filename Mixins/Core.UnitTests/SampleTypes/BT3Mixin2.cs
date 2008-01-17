using System;
using Rubicon.Mixins.Definitions;

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

    public new MixinDefinition Configuration
    {
      get { return base.Configuration; }
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
