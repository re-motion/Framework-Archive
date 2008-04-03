using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
{
  public class ClassOverridingMixinEvent
  {
    [OverrideMixin]
    public virtual event EventHandler Event;
  }
}
