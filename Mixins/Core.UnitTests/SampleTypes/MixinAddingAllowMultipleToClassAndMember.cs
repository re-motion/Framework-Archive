namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Multi]
  public class MixinAddingAllowMultipleToClassAndMember
  {
    [OverrideTarget]
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