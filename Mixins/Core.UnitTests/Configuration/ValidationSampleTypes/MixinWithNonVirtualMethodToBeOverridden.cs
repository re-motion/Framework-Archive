using System;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class MixinWithNonVirtualMethodToBeOverridden
  {
    public string AbstractMethod(int i)
    {
      return "This method is not really abstract.";
    }
  }
}
