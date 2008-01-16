namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes
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