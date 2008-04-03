using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Extends(typeof (GenericClassExtendedByMixin<int>))]
  public class MixinExtendingSpecificGenericClass
  {
  }
}