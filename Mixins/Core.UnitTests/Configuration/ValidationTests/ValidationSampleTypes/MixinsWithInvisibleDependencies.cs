using System;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes
{
  interface IInvisible { }

  class MixinWithInvisibleThisDependency : Mixin<IInvisible>
  {
  }

  class MixinWithInvisibleBaseDependency : Mixin<object, IInvisible>
  {
  }
}
