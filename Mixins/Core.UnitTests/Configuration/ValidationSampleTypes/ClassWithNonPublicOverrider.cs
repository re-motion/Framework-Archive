using System;
namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ClassWithNonPublicOverrider
  {
    [Override]
    private string AbstractMethod (int i)
    {
      return null;
    }

    [Override]
    private string AbstractProperty
    {
      get { return null; }
    }

    [Override]
    private event Func<string> AbstractEvent
    {
      add { }
      remove { }
    }
  }
}