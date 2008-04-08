using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class ClassOverridingMixinEvent
  {
    [OverrideMixin]
    public virtual event EventHandler Event;
  }
}
