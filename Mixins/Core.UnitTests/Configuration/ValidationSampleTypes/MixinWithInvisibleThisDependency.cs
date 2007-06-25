using System;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  interface IInvisible { }

  class MixinWithInvisibleThisDependency : Mixin<IInvisible>
  {
  }
}
