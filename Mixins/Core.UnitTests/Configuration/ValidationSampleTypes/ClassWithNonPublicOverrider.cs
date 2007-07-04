using System;
namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ClassWithNonPublicOverrider
  {
    [Override]
    protected string AbstractMethod (int i)
    {
      return null;
    }

    [Override]
    protected string AbstractProperty
    {
      get { return null; }
    }

    [Override]
    protected event Func<string> AbstractEvent
    {
      add { }
      remove { }
    }
  }
}