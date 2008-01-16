using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes
{
  public interface IEmptyInterface
  { }

  public class MixinWithUnsatisfiedEmptyThisDependency : Mixin<IEmptyInterface>
  {
  }

  public class MixinWithUnsatisfiedEmptyBaseDependency : Mixin<object, IEmptyInterface>
  {
  }

  public interface IEmptyAggregateInterface : IEmptyInterface
  {
  }

  public class MixinWithUnsatisfiedEmptyAggregateThisDependency : Mixin<IEmptyAggregateInterface>
  {
  }

  public class MixinWithUnsatisfiedEmptyAggregateBaseDependency : Mixin<object, IEmptyAggregateInterface>
  {
  }
}
