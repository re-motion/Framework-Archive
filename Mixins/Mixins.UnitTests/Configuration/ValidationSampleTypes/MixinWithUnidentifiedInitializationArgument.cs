namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  internal class MixinWithUnidentifiedInitializationArgument
  {
    [MixinInitializationMethod]
    public void Initialize (object whatever)
    {
    }
  }
}