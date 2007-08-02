namespace Rubicon.Mixins.UnitTests.SampleTypes
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

  [Multi]
  public class MixinAddingAllowMultipleToClassAndMember2 : MixinAddingAllowMultipleToClassAndMember
  {
  }
}