using System;

namespace Mixins.UnitTests.SampleTypes
{
  [MixinFor(typeof(IBaseType2))]
  [Serializable]
  public class BT2Mixin1
  {
  }
}
