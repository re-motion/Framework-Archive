using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public interface IMixinWithEmptyInterface { }
  public class MixinRequiringEmptyInterface : Mixin<object, IMixinWithEmptyInterface> { }
}