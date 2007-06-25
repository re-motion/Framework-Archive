using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IBT5MixinC1
  {
  }

  public class BT5MixinC1 : Mixin<IBT5MixinC2, IBT5MixinC2>, IBT5MixinC1
  {
  }

  public interface IBT5MixinC2
  {
  }

  public class BT5MixinC2 : Mixin<IBT5MixinC1, IBT5MixinC1>, IBT5MixinC2
  {
  }
}
