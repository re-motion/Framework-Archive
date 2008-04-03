using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [CopyCustomAttributes (typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf))]
  [NonInheritedAttribute]
  public class MixinIndirectlyAddingNonInheritedAttributeFromSelf : Mixin<object>
  {
  }
}
