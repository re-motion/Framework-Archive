using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class MixinOverridingSetterOnly
  {
    [OverrideTarget]
    public virtual int Property
    {
      set { }
    }
  }
}