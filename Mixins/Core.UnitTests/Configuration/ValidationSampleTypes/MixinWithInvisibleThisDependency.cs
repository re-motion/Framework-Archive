using System;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  interface IInvisible { }

  class MixinWithInvisibleThisDependency : Mixin<IInvisible>
  {
  }
}
