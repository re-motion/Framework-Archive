namespace Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
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