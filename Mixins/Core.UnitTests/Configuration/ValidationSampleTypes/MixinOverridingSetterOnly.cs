namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class MixinOverridingSetterOnly
  {
    [Override]
    public virtual int Property
    {
      set { }
    }
  }
}