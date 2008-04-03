using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [CopyCustomAttributes (typeof (AttributeSource))]
  public class MixinIndirectlyAddingNonInheritedAttribute : Mixin<object>
  {
    [NonInheritedAttribute]
    public class AttributeSource
    {
    }
  }
}
