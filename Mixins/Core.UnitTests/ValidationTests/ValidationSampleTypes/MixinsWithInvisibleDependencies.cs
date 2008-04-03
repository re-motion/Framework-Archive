using System;

namespace Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
{
  interface IInvisible { }

  class MixinWithInvisibleThisDependency : Mixin<IInvisible>
  {
  }

  class MixinWithInvisibleBaseDependency : Mixin<object, IInvisible>
  {
  }
}
