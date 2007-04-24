namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  [Multi]
  public class MixinAddingAllowMultipleToClassAndMember
  {
    [Override]
    [Multi]
    public void Foo ()
    {
    }
  }
}