using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends(typeof (GenericClassExtendedByMixin<int>))]
  public class MixinExtendingSpecificGenericClass
  {
  }
}