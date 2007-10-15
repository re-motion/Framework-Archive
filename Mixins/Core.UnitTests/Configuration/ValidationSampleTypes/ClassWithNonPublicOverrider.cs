using System;
namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ClassWithNonPublicOverrider
  {
    [OverrideMixinMember]
    private string AbstractMethod (int i)
    {
      return null;
    }

    [OverrideMixinMember]
    private string AbstractProperty
    {
      get { return null; }
    }

    [OverrideMixinMember]
    private event Func<string> AbstractEvent
    {
      add { }
      remove { }
    }
  }
}