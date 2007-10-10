namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IMixinWithEmptyInterface { }
  public class MixinRequiringEmptyInterface : Mixin<object, IMixinWithEmptyInterface> { }
}