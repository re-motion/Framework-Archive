using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [CopyCustomAttributes (typeof (MixinIndirectlyAddingInheritedAttributeFromSelf))]
  [AttributeWithParameters (0)]
  public class MixinIndirectlyAddingInheritedAttributeFromSelf : Mixin<object>
  {
  }
}
