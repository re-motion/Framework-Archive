using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [CopyCustomAttributes (typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf))]
  [NonInheritedAttribute]
  public class MixinIndirectlyAddingNonInheritedAttributeFromSelf : Mixin<object>
  {
  }
}
