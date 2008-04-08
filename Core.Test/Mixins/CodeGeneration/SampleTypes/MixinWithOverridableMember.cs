using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  public class MixinWithOverridableMember : Mixin<object>
  {
    protected virtual void Foo ()
    {
    }
  }
}