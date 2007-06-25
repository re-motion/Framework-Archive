using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
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
}