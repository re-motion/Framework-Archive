using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinAddingBT1AttributeToMember
  {
    [OverrideTarget]
    [BT1]
    public string VirtualMethod ()
    {
      return "";
    }
  }

  [AcceptsAlphabeticOrdering]
  public class MixinAddingBT1AttributeToMember2 : MixinAddingBT1AttributeToMember
  {
  }
}