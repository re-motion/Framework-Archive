using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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
