using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinAddingBT1AttributeToMember
  {
    [Override]
    [BT1]
    public string VirtualMethod ()
    {
      return "";
    }
  }

  public class MixinAddingBT1AttributeToMember2 : MixinAddingBT1AttributeToMember
  {
  }
}