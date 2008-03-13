using System;

namespace Rubicon.Mixins.UnitTests.Configuration.Definitions.DependencySorting.SampleTypes
{
  public class MixinWithBaseCallDependency1 : Mixin<object, IBaseCallDependency1>, IBaseCallDependency2
  {
    
  }
}