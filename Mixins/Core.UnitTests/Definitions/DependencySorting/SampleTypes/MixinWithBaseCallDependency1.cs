using System;

namespace Rubicon.Mixins.UnitTests.Definitions.DependencySorting.SampleTypes
{
  public class MixinWithBaseCallDependency1 : Mixin<object, IBaseCallDependency1>, IBaseCallDependency2
  {
    
  }
}