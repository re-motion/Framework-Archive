using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  [Extends(typeof (GenericClassExtendedByMixin<int>))]
  public class MixinExtendingSpecificGenericClass
  {
  }
}