using System;

namespace Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
{
  public class MixinWithNonVirtualMethodToBeOverridden
  {
    public string AbstractMethod(int i)
    {
      return "This method is not really abstract.";
    }
  }
}
